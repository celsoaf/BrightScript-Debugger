using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BrightScript.Debugger.AD7;
using BrightScript.Debugger.Core;
using BrightScript.Debugger.Core.CommandFactories;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;

namespace BrightScript.Debugger.Engine
{
    internal class DebuggedProcess : Core.Debugger
    {
        public AD_PROCESS_ID Id { get; private set; }
        public AD7Engine Engine { get; private set; }
        public List<string> VariablesToDelete { get; private set; }
        public List<IVariableInformation> ActiveVariables { get; private set; }

        public SourceLineCache SourceLineCache { get; private set; }
        
        private ISampleEngineCallback _callback;
        private bool _bLastModuleLoadFailed;
        private StringBuilder _pendingMessages;
        private WorkerThread _worker;
        private BreakpointManager _breakpointManager;
        private bool _bEntrypointHit;
        private Core.Debugger.ResultEventArgs _initialBreakArgs;
        private List<string> _libraryLoaded;   // unprocessed library loaded messages
        private uint _loadOrder;
        private ReadOnlyCollection<RegisterDescription> _registers;
        private ReadOnlyCollection<RegisterGroup> _registerGroups;
        private bool _needTerminalReset;

        public DebuggedProcess(TcpLaunchOptions launchOptions, ISampleEngineCallback callback, WorkerThread worker, BreakpointManager bpman, AD7Engine engine)
            : base(launchOptions)
        {
            uint processExitCode = 0;
            _pendingMessages = new StringBuilder(400);
            _worker = worker;
            _breakpointManager = bpman;
            Engine = engine;
            _libraryLoaded = new List<string>();
            _loadOrder = 0;
            MICommandFactory = MICommandFactory.GetInstance(this);

            // we do NOT have real Win32 process IDs, so we use a guid
            AD_PROCESS_ID pid = new AD_PROCESS_ID();
            pid.ProcessIdType = (int)enum_AD_PROCESS_ID.AD_PROCESS_ID_GUID;
            pid.guidProcessId = Guid.NewGuid();
            this.Id = pid;

            SourceLineCache = new SourceLineCache(this);

            _callback = callback;
            //_moduleList = new List<DebuggedModule>();
            ThreadCache = new ThreadCache(callback, this);

            VariablesToDelete = new List<string>();
            this.ActiveVariables = new List<IVariableInformation>();

            OutputStringEvent += delegate (object o, string message)
            {
                // We can get messages before we have started the process
                // but we can't send them on until it is
                if (_connected)
                {
                    _callback.OnOutputString(message);
                }
                else
                {
                    _pendingMessages.Append(message);
                }
            };

            //this.Init(_launchOptions);

            MIDebugCommandDispatcher.AddProcess(this);

            // When the debuggee exits, we need to exit the debugger
            ProcessExitEvent += delegate (object o, EventArgs args)
            {
                // NOTE: Exceptions leaked from this method may cause VS to crash, be careful

                Core.Debugger.ResultEventArgs results = args as Core.Debugger.ResultEventArgs;

                if (results.Results.Contains("exit-code"))
                {
                    // GDB sometimes returns exit codes, which don't fit into uint, like "030000000472".
                    // And we can't throw from here, because it crashes VS.
                    // Full exit code will still usually be reported in the Output window,
                    // but here let's return "uint.MaxValue" just to indicate that something went wrong.
                    if (!uint.TryParse(results.Results.FindString("exit-code"), out processExitCode))
                    {
                        processExitCode = uint.MaxValue;
                    }
                }

                // quit MI Debugger
                _worker.PostOperation(CmdExitAsync);
            };

            // When the debugger exits, we tell AD7 we are done
            DebuggerExitEvent += delegate (object o, EventArgs args)
            {
                // NOTE: Exceptions leaked from this method may cause VS to crash, be careful

                // this is the last AD7 Event we can ever send
                // Also the transport is closed when this returns
                _callback.OnProcessExit(processExitCode);

                Dispose();
            };

            DebuggerAbortedEvent += delegate (object o, /*OPTIONAL*/ string debuggerExitCode)
            {
                // NOTE: Exceptions leaked from this method may cause VS to crash, be careful

                // The MI debugger process unexpectedly exited.
                _worker.PostOperation(() =>
                {
                    // If the MI Debugger exits before we get a resume call, we have no way of sending program destroy. So just let start debugging fail.
                    if (!_connected)
                    {
                        return;
                    }

                    string message;
                    if (string.IsNullOrEmpty(debuggerExitCode))
                        message = string.Format(CultureInfo.CurrentCulture, MICoreResources.Error_MIDebuggerExited_UnknownCode, MICommandFactory.Name);
                    else
                        message = string.Format(CultureInfo.CurrentCulture, MICoreResources.Error_MIDebuggerExited_WithCode, MICommandFactory.Name, debuggerExitCode);

                    _callback.OnError(message);
                    _callback.OnProcessExit(uint.MaxValue);

                    Dispose();
                });
            };

            // When we break we need to gather information
            BreakModeEvent += async delegate (object o, EventArgs args)
            {
                // NOTE: This is an async void method, so make sure exceptions are caught and somehow reported

                Core.Debugger.ResultEventArgs results = args as Core.Debugger.ResultEventArgs;

                if (!this._connected)
                {
                    _initialBreakArgs = results;
                    return;
                }

                try
                {
                    await HandleBreakModeEvent(results);
                }
                catch (Exception e) when (ExceptionHelper.BeforeCatch(e, reportOnlyCorrupting: true))
                {
                    if (this.ProcessState == ProcessState.Exited)
                    {
                        return; // ignore exceptions after the process has exited
                    }

                    string exceptionDescription = EngineUtils.GetExceptionDescription(e);
                    string message = string.Format(CultureInfo.CurrentCulture, MICoreResources.Error_FailedToEnterBreakState, exceptionDescription);
                    _callback.OnError(message);

                    Terminate();
                }
            };

            ErrorEvent += delegate (object o, EventArgs args)
            {
                // NOTE: Exceptions leaked from this method may cause VS to crash, be careful

                Core.Debugger.ResultEventArgs result = (Core.Debugger.ResultEventArgs)args;
                _callback.OnError(result.Results.FindString("msg"));
            };

            ThreadCreatedEvent += delegate (object o, EventArgs args)
            {
                Core.Debugger.ResultEventArgs result = (Core.Debugger.ResultEventArgs)args;
                ThreadCache.ThreadEvent(result.Results.FindInt("id"), /*deleted */false);
            };

            ThreadExitedEvent += delegate (object o, EventArgs args)
            {
                Core.Debugger.ResultEventArgs result = (Core.Debugger.ResultEventArgs)args;
                ThreadCache.ThreadEvent(result.Results.FindInt("id"), /*deleted*/true);
            };

            MessageEvent += (object o, Core.Debugger.ResultEventArgs args) =>
            {
                OutputMessage outputMessage = DecodeOutputEvent(args.Results);
                if (outputMessage != null)
                {
                    _callback.OnOutputMessage(outputMessage);
                }
            };

            BreakChangeEvent += _breakpointManager.BreakpointModified;
        }

        public async Task Initialize(EventWaitHandle waitLoop, CancellationToken token)
        {
            bool success = false;
            int total = 1;

            await this.WaitForConsoleDebuggerInitialize(token);

            try
            {
                this.MICommandFactory.EnableTargetAsyncOption();

                success = true;
            }
            finally
            {
                if (!success)
                {
                    Terminate();
                }
            }

            waitLoop.Set();
            token.ThrowIfCancellationRequested();
        }

        private void Dispose()
        {
            if (_launchOptions.DeviceAppLauncher != null)
            {
                _launchOptions.DeviceAppLauncher.Dispose();
            }
        }

        private async Task HandleBreakModeEvent(Core.Debugger.ResultEventArgs results)
        {
            string reason = results.Results.TryFindString("reason");
            int tid;
            if (!results.Results.Contains("thread-id"))
            {
                Results res = await MICommandFactory.ThreadInfo();
                tid = res.FindInt("id");
            }
            else
            {
                tid = results.Results.FindInt("thread-id");
            }

            // Any existing variable objects at this point are from the last time we were in break mode, and are
            //  therefore invalid.  Dispose them so they're marked for cleanup.
            lock (this.ActiveVariables)
            {
                foreach (IVariableInformation varInfo in this.ActiveVariables)
                {
                    varInfo.Dispose();
                }
                this.ActiveVariables.Clear();
            }

            //ThreadCache.MarkDirty();
            //MICommandFactory.DefineCurrentThread(tid);

            DebuggedThread thread = ThreadCache.FindThread(tid);
            //await ThreadCache.StackFrames(thread);  // prepopulate the break thread in the thread cache
            ThreadCache.SendThreadEvents(this, null);   // make sure that new threads have been pushed to the UI

            //always delete breakpoints pending deletion on break mode
            //the flag tells us if we hit an existing breakpoint pending deletion that we need to continue

            await _breakpointManager.DeleteBreakpointsPendingDeletion();

            // Delete GDB variable objects that have been marked for cleanup
            List<string> variablesToDelete = null;
            lock (VariablesToDelete)
            {
                variablesToDelete = new List<string>(this.VariablesToDelete);
                VariablesToDelete.Clear();
            }

            foreach (var variable in variablesToDelete)
            {
                try
                {
                    await MICommandFactory.VarDelete(variable);
                }
                catch (MIException)
                {
                    //not much to do really, we're leaking MI debugger variables.
                    Debug.Fail("Failed to delete variable: " + variable + ". This is leaking memory in the MI Debugger.");
                }
            }

            if (String.IsNullOrWhiteSpace(reason) && !_bEntrypointHit)
            {
                _bEntrypointHit = true;
                CmdContinueAsync();
                FireDeviceAppLauncherResume();
            }
            else if (reason == "entry-point-hit")
            {
                _bEntrypointHit = true;
                _callback.OnEntryPoint(thread);
            }
            else if (reason == "breakpoint-hit")
            {
                ThreadContext cxt = await ThreadCache.GetThreadContext(thread);

                string bkptno = results.Results.FindString("bkptno");
                ulong addr = cxt.pc ?? 0;

                bool fContinue;
                TupleValue frame = results.Results.TryFind<TupleValue>("frame");
                AD7BoundBreakpoint[] bkpt = _breakpointManager.FindHitBreakpoints(bkptno, addr, frame, out fContinue);
                if (bkpt != null)
                {
                    List<object> bplist = new List<object>();
                    bplist.AddRange(bkpt);
                    _callback.OnBreakpoint(thread, bplist.AsReadOnly());
                }
                else if (!_bEntrypointHit)
                {
                    _bEntrypointHit = true;
                    _callback.OnEntryPoint(thread);
                }
                else if (bkptno == "<EMBEDDED>")
                {
                    _callback.OnBreakpoint(thread, new ReadOnlyCollection<object>(new AD7BoundBreakpoint[] { }));
                }
                else
                {
                    if (fContinue)
                    {
                        //we hit a bp pending deletion
                        //post the CmdContinueAsync operation so it does not happen until we have deleted all the pending deletes
                        CmdContinueAsync();
                    }
                    else
                    {
                        // not one of our breakpoints, so stop with a message
                        _callback.OnException(thread, "Unknown breakpoint", "", 0);
                    }
                }
            }
            else if (reason == "end-stepping-range" || reason == "function-finished")
            {
                _callback.OnStepComplete(thread);
            }
            else if (reason == "signal-received")
            {
                string name = results.Results.TryFindString("signal-name");
                if ((name == "SIG32") || (name == "SIG33"))
                {
                    // we are going to ignore these (Sigma) signals for now
                    CmdContinueAsync();
                }
                else if (MICommandFactory.IsAsyncBreakSignal(results.Results))
                {
                    _callback.OnAsyncBreakComplete(thread);
                }
                else
                {
                    uint code = 0;
                    string sigName = results.Results.TryFindString("signal-name");
                    code = results.Results.Contains("signal") ? results.Results.FindUint("signal") : 0;
                    if (String.IsNullOrEmpty(sigName) && code != 0 && EngineUtils.SignalMap.Instance.ContainsValue(code))
                    {
                        sigName = EngineUtils.SignalMap.Instance.First((p) => p.Value == code).Key;
                    }
                    else if (!String.IsNullOrEmpty(sigName) && code == 0 && EngineUtils.SignalMap.Instance.ContainsKey(sigName))
                    {
                        code = EngineUtils.SignalMap.Instance[sigName];
                    }
                    _callback.OnException(thread, sigName, results.Results.TryFindString("signal-meaning"), code);
                }
            }
            else if (reason == "exception-received")
            {
                string exceptionName = results.Results.TryFindString("exception-name");
                if (string.IsNullOrEmpty(exceptionName))
                    exceptionName = "Exception";

                string description = results.Results.FindString("exception");
                Guid? exceptionCategory;
                ExceptionBreakpointState state;
                MICommandFactory.DecodeExceptionReceivedProperties(results.Results, out exceptionCategory, out state);

                _callback.OnException(thread, exceptionName, description, 0, exceptionCategory, state);
            }
            else
            {
                Debug.Fail("Unknown stopping reason");
                _callback.OnException(thread, "Unknown", "Unknown stopping event", 0);
            }
        }

        internal WorkerThread WorkerThread
        {
            get { return _worker; }
        }
        internal string EscapePath(string path, bool ignoreSpaces = false)
        {
            path = path.Trim();
            path = path.Replace(@"\", @"\\");

            if (!ignoreSpaces && path.IndexOf(' ') != -1)
            {
                path = '"' + path + '"';
            }
            return path;
        }

        internal static string UnixPathToWindowsPath(string unixPath)
        {
            return unixPath.Replace('/', '\\');
        }

        // this is called on any thread, so we need to dispatch the command via
        // the Worker thread, to end up in DispatchCommand
        protected override void ScheduleStdOutProcessing(string line)
        {
            _worker.PostOperation(() => { ProcessStdOutLine(line); });
        }

        protected override void ScheduleResultProcessing(Action func)
        {
            _worker.PostOperation(() => { func(); });
        }

        public async Task Execute(DebuggedThread thread)
        {
            // Should clear stepping state
            if (_worker.IsPollThread())
            {
                CmdContinueAsync();
            }
            else
            {
                _worker.PostOperation(CmdContinueAsync);
            }
        }

        public Task Continue(DebuggedThread thread)
        {
            // Called after Stopping event
            return Execute(thread);
        }

        public async Task Step(int threadId, enum_STEPKIND kind, enum_STEPUNIT unit)
        {
            if ((unit == enum_STEPUNIT.STEP_LINE) || (unit == enum_STEPUNIT.STEP_STATEMENT))
            {
                switch (kind)
                {
                    case enum_STEPKIND.STEP_INTO:
                        await MICommandFactory.ExecStep(threadId);
                        break;
                    case enum_STEPKIND.STEP_OVER:
                        await MICommandFactory.ExecNext(threadId);
                        break;
                    case enum_STEPKIND.STEP_OUT:
                        await MICommandFactory.ExecFinish(threadId);
                        break;
                    default:
                        throw new NotImplementedException();
                }

                ThreadCache.MarkDirty();
            }
            else if (unit == enum_STEPUNIT.STEP_INSTRUCTION)
            {
                switch (kind)
                {
                    case enum_STEPKIND.STEP_INTO:
                        await MICommandFactory.ExecStepInstruction(threadId);
                        break;
                    case enum_STEPKIND.STEP_OVER:
                        await MICommandFactory.ExecNextInstruction(threadId);
                        break;
                    case enum_STEPKIND.STEP_OUT:
                        await MICommandFactory.ExecFinish(threadId);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        //public DebuggedModule ResolveAddress(ulong addr)
        //{
        //    lock (_moduleList)
        //    {
        //        return _moduleList.Find((m) => m.AddressInModule(addr));
        //    }
        //}

        public void Close()
        {
            if (_launchOptions.DeviceAppLauncher != null)
            {
                _launchOptions.DeviceAppLauncher.Terminate();
            }
            CloseQuietly();
        }

        public async Task ResumeFromLaunch()
        {
            _connected = true;

            // Send any strings we got before the process came up
            if (_pendingMessages?.Length != 0)
            {
                _callback.OnOutputString(_pendingMessages.ToString());
                _pendingMessages = null;
            }

            if (_initialBreakArgs != null)
            {
                _libraryLoaded.Clear();
                await HandleBreakModeEvent(_initialBreakArgs);
                _initialBreakArgs = null;
            }
            else
            {
                bool attach = false;
                int attachPid = 0;

                if (!attach)
                {
                    switch (_launchOptions.LaunchCompleteCommand)
                    {
                        case LaunchCompleteCommand.ExecRun:
                            MICommandFactory.ExecRun();
                            break;
                        case LaunchCompleteCommand.ExecContinue:
                            MICommandFactory.ExecContinue();
                            break;
                        case LaunchCompleteCommand.None:
                            break;
                        default:
                            Debug.Fail("Not implemented enum code for LaunchCompleteCommand??");
                            throw new NotImplementedException();
                    }
                }

                FireDeviceAppLauncherResume();
            }
        }

        private void FireDeviceAppLauncherResume()
        {
            Init(_launchOptions);
        }

        /// <summary>
        /// Generates results that represent an emulated MI stopped record.
        /// </summary>
        private async Task<Results> GenerateStoppedRecordResults()
        {
            Results threadInfo = await this.MICommandFactory.ThreadInfo();

            // Get the current thread identifier
            string currentThreadId = threadInfo.FindString("current-thread-id");

            // Get list of all threads in the process
            ValueListValue threads = threadInfo.Find<ValueListValue>("threads");

            // Find the thread that is the current thread, which should exist since there is a current thread id value
            TupleValue currentThread = threads.AsArray<TupleValue>().FirstOrDefault(tv => currentThreadId.Equals(tv.FindString("id"), StringComparison.Ordinal));
            Debug.Assert(null != currentThread, String.Concat("Unable to find thread with ID ", currentThreadId, "."));
            if (null == currentThread)
                throw new UnexpectedMIResultException(this.MICommandFactory.Name, "-thread-info", null);

            // Get the frame of the current thread
            TupleValue currentFrame = currentThread.Find<TupleValue>("frame");

            // Collect the addr, func, and args fields from the current frame as they are required.
            // Collect the file, fullname, and line fileds if they are available. They may be missing if the frame is for
            // a binary that does not have symbols.
            TupleValue newFrame = currentFrame.Subset(
                new string[] { "addr", "func", "args" },
                new string[] { "file", "fullname", "line" });

            // Create result that emulates a signal received from the debuggee with the frame and thread information
            List<NamedResultValue> values = new List<NamedResultValue>();
            values.Add(new NamedResultValue("reason", new ConstValue("signal-received")));
            values.Add(new NamedResultValue("frame", newFrame));
            values.Add(new NamedResultValue("thread-id", new ConstValue(currentThreadId)));
            return new Results(ResultClass.done, values);
        }

        public override void Terminate()
        {
            base.Terminate();
            this.CmdTerminate();

            // Pretend to kill the process, which will tear down the MI Debugger
            //TODO: Something better than this.
            if (_launchOptions.DeviceAppLauncher != null)
            {
                _launchOptions.DeviceAppLauncher.Terminate();
            }
            ScheduleStdOutProcessing(@"*stopped,reason=""exited"",exit-code=""42""");
        }

        public void Detach() { }
        //public DebuggedModule[] GetModules()
        //{
        //    lock (_moduleList)
        //    {
        //        return _moduleList.ToArray();
        //    }
        //}

        //public DebuggedModule FindModule(string id)
        //{
        //    lock (_moduleList)
        //    {
        //        return _moduleList.Find((m) => m.Id == id);
        //    }
        //}

        public bool GetSourceInformation(uint addr, ref string m_documentName, ref string m_functionName, ref uint m_lineNum, ref uint m_numParameters, ref uint m_numLocals)
        {
            return false;
        }

        public uint[] GetAddressesForSourceLocation(string moduleName, string documentName, uint dwStartLine, uint dwStartCol)
        {
            uint[] addrs = new uint[1];
            addrs[0] = 0xDEADF00D;
            return addrs;
        }

        public void SetBreakpoint(uint address, Object client)
        {
            throw new NotImplementedException();
        }

        internal void OnPostedOperationError(object sender, Exception e)
        {
            if (this.ProcessState == ProcessState.Exited)
            {
                return; // ignore exceptions after the process has exited
            }

            string exceptionMessage = e.Message.TrimEnd(' ', '\t', '.', '\r', '\n');
            string userMessage = string.Format(CultureInfo.CurrentCulture, MICoreResources.Error_ExceptionInOperation, exceptionMessage);
            _callback.OnError(userMessage);
        }

        //This method gets the locals and parameters and creates an MI debugger variable for each one so that we can manipulate them (and expand children, etc.)
        //NOTE: Eval is called
        internal async Task<List<VariableInformation>> GetLocalsAndParameters(AD7Thread thread, ThreadContext ctx)
        {
            List<VariableInformation> variables = new List<VariableInformation>();

            ValueListValue localsAndParameters = await MICommandFactory.StackListVariables(PrintValues.NoValues, thread.Id, ctx.Level);

            foreach (var localOrParamResult in localsAndParameters.Content)
            {
                string name = localOrParamResult.FindString("name");
                bool isParam = localOrParamResult.TryFindString("arg") == "1";
                SimpleVariableInformation simpleInfo = new SimpleVariableInformation(name, isParam);
                VariableInformation vi = await simpleInfo.CreateMIDebuggerVariable(ctx, Engine, thread);
                variables.Add(vi);
            }

            return variables;
        }

        //This method gets the value/type info for the method parameters without creating an MI debugger varialbe for them. For use in the callstack window
        //NOTE: eval is not called
        public async Task<List<SimpleVariableInformation>> GetParameterInfoOnly(AD7Thread thread, ThreadContext ctx)
        {
            List<SimpleVariableInformation> parameters = new List<SimpleVariableInformation>();

            ValueListValue localAndParameters = await MICommandFactory.StackListVariables(PrintValues.SimpleValues, thread.Id, ctx.Level);

            foreach (var results in localAndParameters.Content.Where(r => r.TryFindString("arg") == "1"))
            {
                parameters.Add(new SimpleVariableInformation(results.FindString("name"), /*isParam*/ true, results.FindString("value"), results.FindString("type")));
            }

            return parameters;
        }

        //This method gets the value/type info for the method parameters of all frames without creating an mi debugger varialbe for them. For use in the callstack window
        //NOTE: eval is not called
        public async Task<List<ArgumentList>> GetParameterInfoOnly(AD7Thread thread, bool values, bool types, uint low, uint high)
        {
            //var frames = await MICommandFactory.StackListArguments(values || types ? PrintValues.SimpleValues : PrintValues.NoValues, thread.Id, low, high);
            var variables = ThreadCache.GetVariables(thread.Id);
            List<ArgumentList> parameters = new List<ArgumentList>();

            int level = 0;
            List<SimpleVariableInformation> args = new List<SimpleVariableInformation>();
            if (variables != null)
            {
                foreach (var vm in variables)
                {
                    args.Add(new SimpleVariableInformation(vm.Ident, true, vm.Value));
                }
            }
            parameters.Add(new ArgumentList(level, args));

            return parameters;
        }

        private OutputMessage DecodeOutputEvent(Results results)
        {
            // NOTE: the message event is an MI Extension from clrdbg, though we could use in it the future for other debuggers
            string text = results.TryFindString("text");
            if (string.IsNullOrEmpty(text))
            {
                Debug.Fail("Bogus message event. Missing 'text' property.");
                return null;
            }

            string sendTo = results.TryFindString("send-to");
            if (string.IsNullOrEmpty(sendTo))
            {
                Debug.Fail("Bogus message event, missing 'send-to' property");
                return null;
            }

            enum_MESSAGETYPE messageType;
            switch (sendTo)
            {
                case "message-box":
                    messageType = enum_MESSAGETYPE.MT_MESSAGEBOX;
                    break;

                case "output-window":
                    messageType = enum_MESSAGETYPE.MT_OUTPUTSTRING;
                    break;

                default:
                    Debug.Fail("Bogus message event. Unexpected 'send-to' property. Ignoring.");
                    return null;
            }

            OutputMessage.Severity severity = OutputMessage.Severity.Warning;
            switch (results.TryFindString("severity"))
            {
                case "error":
                    severity = OutputMessage.Severity.Error;
                    break;

                case "warning":
                    severity = OutputMessage.Severity.Warning;
                    break;
            }

            switch (results.TryFindString("source"))
            {
                case "target-exception":
                    messageType |= enum_MESSAGETYPE.MT_REASON_EXCEPTION;
                    break;
                case "jmc-prompt":
                    messageType |= (enum_MESSAGETYPE)enum_MESSAGETYPE90.MT_REASON_JMC_PROMPT;
                    break;
                case "step-filter":
                    messageType |= (enum_MESSAGETYPE)enum_MESSAGETYPE90.MT_REASON_STEP_FILTER;
                    break;
                case "fatal-error":
                    messageType |= (enum_MESSAGETYPE)enum_MESSAGETYPE120.MT_FATAL_ERROR;
                    break;
            }

            uint errorCode = results.TryFindUint("error-code") ?? 0;
            return new OutputMessage(text, messageType, severity, errorCode);
        }

        private static RegisterGroup GetGroupForRegister(List<RegisterGroup> registerGroups, string name, EngineUtils.RegisterNameMap nameMap)
        {
            string grpName = nameMap.GetGroupName(name);
            RegisterGroup grp = registerGroups.FirstOrDefault((g) => { return g.Name == grpName; });
            if (grp == null)
            {
                grp = new RegisterGroup(grpName);
                registerGroups.Add(grp);
            }
            return grp;
        }

        public async Task<Tuple<int, string>[]> GetRegisters(int threadId, uint level)
        {
            TupleValue[] values = await MICommandFactory.DataListRegisterValues(threadId);
            Tuple<int, string>[] regValues = new Tuple<int, string>[values.Length];
            for (int i = 0; i < values.Length; ++i)
            {
                int index = values[i].FindInt("number");
                string regContent = values[i].FindString("value");
                regValues[i] = new Tuple<int, string>(index, regContent);
            }
            return regValues;
        }

        public async Task DisableBreakpointsForFuncEvalAsync()
        {
            await _breakpointManager.DisableBreakpointsForFuncEvalAsync();
        }

        public async Task EnableBreakpointsAfterFuncEvalAsync()
        {
            await _breakpointManager.EnableAfterFuncEvalAsync();
        }

        /// <summary>
        /// Finds the line associated with a start address.
        /// </summary>
        public async Task<uint> LineForStartAddress(string file, ulong startAddress)
        {
            List<ulong> addresses = new List<ulong>();
            SourceLineMap srcLines = await SourceLineCache.GetLinesForFile(file);
            if (srcLines == null || srcLines.Count == 0)
            {
                srcLines = await SourceLineCache.GetLinesForFile(System.IO.Path.GetFileName(file));
            }
            if (srcLines == null || srcLines.Count == 0)
            {
                return 0;
            }

            SourceLine srcLine;
            if (srcLines.TryGetValue(startAddress, out srcLine))
            {
                return srcLine.Line;
            }
            return 0;
        }

        public async Task<List<ulong>> StartAddressesForLine(string file, uint line)
        {
            List<ulong> addresses = new List<ulong>();
            SourceLineMap srcLines = await SourceLineCache.GetLinesForFile(file);
            if (srcLines == null || srcLines.Count == 0)
            {
                srcLines = await SourceLineCache.GetLinesForFile(System.IO.Path.GetFileName(file));
            }
            if (srcLines != null && srcLines.Count > 0)
            {
                bool gotoNextFunc = false;
                foreach (KeyValuePair<ulong, SourceLine> l in srcLines)
                {
                    if (gotoNextFunc)
                    {
                        if (l.Value.Line == 0)
                        {
                            gotoNextFunc = false;
                        }
                    }
                    else if (line == l.Value.Line)
                    {
                        addresses.Add(l.Value.AddrStart);
                        gotoNextFunc = true;
                    }
                }
            }
            if (addresses.Count == 0)
            {
                // ask the underlying debugger for the line info
                addresses = await MICommandFactory.StartAddressesForLine(EscapePath(file), line);
            }
            return addresses;
        }
    }
}