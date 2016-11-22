﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using BrightScript.Debugger.Engine;
using BrightScript.Debugger.Enums;
using Microsoft.VisualStudio.Debugger.Interop;

namespace BrightScript.Debugger.Core.CommandFactories
{
    public enum PrintValues
    {
        NoValues = 0,
        AllValues = 1,
        SimpleValues = 2,
    }

    [Flags]
    public enum ExceptionBreakpointState
    {
        None = 0,
        BreakUserHandled = 0x1,
        BreakThrown = 0x2
    }

    public abstract class MICommandFactory
    {
        protected Debugger _debugger;

        public abstract string Name { get; }

        public static MICommandFactory GetInstance(Debugger debugger)
        {
            MICommandFactory commandFactory;

            commandFactory = new RokuMICommandFactory();
            commandFactory._debugger = debugger;
            commandFactory.Radix = 10;
            return commandFactory;
        }
        #region Stack Manipulation

        public virtual void DefineCurrentThread(int threadId)
        {
        }

        public virtual async Task<Results> ThreadInfo()
        {
            Results threadsinfo = await _debugger.CmdAsync("-thread-info", ResultClass.None);
            return threadsinfo;
        }

        public async Task<Results> StackInfoDepth(int threadId, int maxDepth = 1000, ResultClass resultClass = ResultClass.done)
        {
            string command = string.Format(@"-stack-info-depth {0}", maxDepth);
            Results results = await ThreadCmdAsync(command, resultClass, threadId);

            return results;
        }

        public async Task<TupleValue[]> StackListFrames(int threadId, uint lowFrameLevel, uint highFrameLevel = 1000)
        {
            string command = DebuggerCommandEnum.bt.ToString();
            Results results = await ThreadCmdAsync(command, ResultClass.done, threadId);

            ListValue list = results.Find<ListValue>("stack");
            if (list is ResultListValue)
            {
                // Populated stacks are converted to ResultListValue type. Return all instances of "frame={...}".
                return ((ResultListValue)list).FindAll<TupleValue>("frame");
            }
            else if (list is ValueListValue)
            {
                // Empty stacks are converted to ValueListValue type. Just return an empty stack.
                return new TupleValue[0];
            }
            else
            {
                throw new MIResultFormatException("stack", results);
            }
        }

        public async Task<Results> StackInfoFrame()
        {
            string command = DebuggerCommandEnum.bt.ToString();
            Results results = await _debugger.CmdAsync(command, ResultClass.done);

            return results;
        }

        /// <summary>
        /// Get locals for the give frame
        /// </summary>
        /// <param name="printValues">0 for no values, 1 for all values, 2 for simple values</param>
        /// <param name="threadId"></param>
        /// <param name="frameLevel"></param>
        /// <returns></returns>
        public async Task<ResultValue> StackListLocals(PrintValues printValues, int threadId, uint frameLevel)
        {
            string cmd = DebuggerCommandEnum.var.ToString();

            Results localsResults = await ThreadFrameCmdAsync(cmd, ResultClass.done, threadId, frameLevel);
            return localsResults.Find("locals");
        }

        /// <summary>
        /// Get Stack arguments for mulitples frames
        /// </summary>
        /// <param name="printValues"></param>
        /// <param name="threadId"></param>
        /// <param name="lowFrameLevel"></param>
        /// <param name="hiFrameLevel"></param>
        /// <returns>This returns an array of results of frames, which contains a level and an args array. </returns>
        public virtual async Task<TupleValue[]> StackListArguments(PrintValues printValues, int threadId, uint lowFrameLevel, uint hiFrameLevel)
        {
            string cmd = DebuggerCommandEnum.var.ToString();
            Results argumentsResults = await ThreadCmdAsync(cmd, ResultClass.done, threadId);

            return argumentsResults.Find<ListValue>("stack-args").IsEmpty()
                ? new TupleValue[0]
                : argumentsResults.Find<ResultListValue>("stack-args").FindAll<TupleValue>("frame");
        }

        /// <summary>
        /// Get Stack arguments for a single frame
        /// </summary>
        /// <param name="printValues"></param>
        /// <param name="threadId"></param>
        /// <param name="frameLevel"></param>
        /// <returns>This returns an array of results for args, which have a name and a value, etc.</returns>
        public async Task<ListValue> StackListArguments(PrintValues printValues, int threadId, uint frameLevel)
        {
            TupleValue[] frameResults = await StackListArguments(printValues, threadId, frameLevel, frameLevel);

            Debug.Assert(frameResults.Length == 1);

            return frameResults[0].Find<ListValue>("args");
        }

        /// <summary>
        /// Get variables for the given frame
        /// </summary>
        /// <param name="printValues"></param>
        /// <param name="threadId"></param>
        /// <param name="frameLevel"></param>
        /// <returns>Returns an array of results for variables</returns>
        public async Task<ValueListValue> StackListVariables(PrintValues printValues, int threadId, uint frameLevel)
        {
            string cmd = DebuggerCommandEnum.var.ToString();

            Results variablesResults = await ThreadFrameCmdAsync(cmd, ResultClass.done, threadId, frameLevel);
            return variablesResults.Find<ValueListValue>("variables");
        }

        #endregion

        #region Program Execution

        public async Task ExecStep(int threadId, ResultClass resultClass = ResultClass.running)
        {
            string command = DebuggerCommandEnum.s.ToString();
            await ThreadCmdAsync(command, resultClass, threadId);
        }

        public async Task ExecNext(int threadId, ResultClass resultClass = ResultClass.running)
        {
            string command = DebuggerCommandEnum.over.ToString();
            await ThreadCmdAsync(command, resultClass, threadId);
        }

        public async Task ExecFinish(int threadId, ResultClass resultClass = ResultClass.running)
        {
            string command = DebuggerCommandEnum.@out.ToString();
            await ThreadCmdAsync(command, resultClass, threadId);
        }

        public async Task ExecStepInstruction(int threadId, ResultClass resultClass = ResultClass.running)
        {
            string command = DebuggerCommandEnum.s.ToString();
            await ThreadCmdAsync(command, resultClass, threadId);
        }

        public async Task ExecNextInstruction(int threadId, ResultClass resultClass = ResultClass.running)
        {
            string command = DebuggerCommandEnum.s.ToString();
            await ThreadCmdAsync(command, resultClass, threadId);
        }

        /// <summary>
        /// Tells GDB to spawn a target process previous setup with -file-exec-and-symbols or similar
        /// </summary>
        public async Task ExecRun()
        {
            string command = DebuggerCommandEnum.c.ToString();
            await _debugger.CmdAsync(command, ResultClass.running);
        }

        /// <summary>
        /// Continues running the target process
        /// </summary>
        public async Task ExecContinue()
        {
            string command = DebuggerCommandEnum.c.ToString();
            await _debugger.CmdAsync(command, ResultClass.running);
        }

        #endregion

        #region Data Manipulation

        public async Task<string[]> DataListRegisterNames()
        {
            string cmd = "-data-list-register-names";
            Results results = await _debugger.CmdAsync(cmd, ResultClass.done);
            return results.Find<ValueListValue>("register-names").AsStrings;
        }

        public async Task<TupleValue[]> DataListRegisterValues(int threadId)
        {
            string command = "-data-list-register-values x";
            Results results = await ThreadCmdAsync(command, ResultClass.done, threadId);
            return results.Find<ValueListValue>("register-values").AsArray<TupleValue>();
        }

        public async Task<string> DataEvaluateExpression(string expr, int threadId, uint frame)
        {
            string command = "-data-evaluate-expression \"" + expr + "\"";
            Results results = await ThreadFrameCmdAsync(command, ResultClass.None, threadId, frame);
            return results.FindString("value");
        }

        public virtual async Task<bool> SetRadix(uint radix)
        {
            if (radix != 10 && radix != 16)
            {
                return false;
            }
            string command = "-gdb-set output-radix " + radix;
            Results results = await _debugger.CmdAsync(command, ResultClass.None);
            Radix = radix;
            return results.ResultClass == ResultClass.done;
        }

        public virtual Task<bool> SetJustMyCode(bool enabled)
        {
            // Default implementation of SetJustMyCode does nothing as only a few engines support this feature.
            // We will override this for debuggers that support Just My Code.
            return Task.FromResult<bool>(true);
        }

        public uint Radix { get; protected set; }


        #endregion

        #region Variable Objects

        public virtual async Task<Results> VarCreate(string expression, int threadId, uint frameLevel, enum_EVALFLAGS dwFlags, ResultClass resultClass = ResultClass.done)
        {
            string command = string.Format("-var-create - * \"{0}\"", expression);
            Results results = await ThreadFrameCmdAsync(command, resultClass, threadId, frameLevel);

            return results;
        }

        public async Task<Results> VarSetFormat(string variableName, string format, ResultClass resultClass = ResultClass.done)
        {
            string command = string.Format(@"-var-set-format {0} {1}", variableName, format);
            Results results = await _debugger.CmdAsync(command, resultClass);

            return results;
        }

        public virtual async Task<Results> VarListChildren(string variableReference, enum_DEBUGPROP_INFO_FLAGS dwFlags, ResultClass resultClass = ResultClass.done)
        {
            // Limit the number of children expanded to 1000 in case memory is uninitialized
            string command = string.Format("-var-list-children --simple-values \"{0}\" 0 1000", variableReference);
            Results results = await _debugger.CmdAsync(command, resultClass);

            return results;
        }

        public async Task<Results> VarEvaluateExpression(string variableName, ResultClass resultClass = ResultClass.done)
        {
            string command = string.Format(@"-var-evaluate-expression {0}", variableName);
            Results results = await _debugger.CmdAsync(command, resultClass);

            return results;
        }

        public async Task<string> VarAssign(string variableName, string expression)
        {
            string command = string.Format("-var-assign {0} \"{1}\"", variableName, expression);
            Results results = await _debugger.CmdAsync(command, ResultClass.done);
            return results.FindString("value");
        }

        public async Task<string> VarShowAttributes(string variableName)
        {
            string command = string.Format("-var-show-attributes {0}", variableName);
            Results results = await _debugger.CmdAsync(command, ResultClass.done);

            string attribute = string.Empty;

            // The docs say that this should be 'status' but Android version of Gdb-mi uses 'attr'
            if (results.Contains("attr"))
            {
                attribute = results.FindString("attr");
            }
            else
            {
                attribute = results.FindString("status");
            }

            return attribute;
        }

        public async Task VarDelete(string variableName)
        {
            string command = string.Format("-var-delete {0}", variableName);
            await _debugger.CmdAsync(command, ResultClass.None);
        }

        public async Task<string> VarInfoPathExpression(string variableName)
        {
            string command = string.Format("-var-info-path-expression {0}", variableName);
            Results results = await _debugger.CmdAsync(command, ResultClass.done);
            return results.FindString("path_expr");
        }

        public virtual async Task Terminate()
        {
            string command = DebuggerCommandEnum.exit.ToString();
            await _debugger.CmdAsync(command, ResultClass.None);
        }

        #endregion

        #region Breakpoints

        private StringBuilder BuildBreakInsert(string condition)
        {
            StringBuilder cmd = new StringBuilder("-break-insert -f ");
            if (condition != null)
            {
                cmd.Append("-c \"");
                cmd.Append(condition);
                cmd.Append("\" ");
            }
            return cmd;
        }

        public virtual async Task<Results> BreakInsert(string filename, uint line, string condition, ResultClass resultClass = ResultClass.done)
        {
            StringBuilder cmd = BuildBreakInsert(condition);
            cmd.Append(filename);
            cmd.Append(":");
            cmd.Append(line.ToString());
            return await _debugger.CmdAsync(cmd.ToString(), resultClass);
        }

        public virtual async Task<Results> BreakInsert(string functionName, string condition, ResultClass resultClass = ResultClass.done)
        {
            StringBuilder cmd = BuildBreakInsert(condition);
            // TODO: Add support of break function type filename:function locations
            cmd.Append(functionName);
            return await _debugger.CmdAsync(cmd.ToString(), resultClass);
        }

        public virtual async Task<TupleValue> BreakInfo(string bkptno)
        {
            Results bindResult = await _debugger.CmdAsync("-break-info " + bkptno, ResultClass.None);
            if (bindResult.ResultClass != ResultClass.done)
            {
                return null;
            }
            var breakpointTable = bindResult.Find<TupleValue>("BreakpointTable").Find<ResultListValue>("body").FindAll<TupleValue>("bkpt");
            return breakpointTable[0];
        }

        public virtual async Task BreakEnable(bool enabled, string bkptno)
        {
            if (enabled)
            {
                await _debugger.CmdAsync("-break-enable " + bkptno, ResultClass.done);
            }
            else
            {
                await _debugger.CmdAsync("-break-disable " + bkptno, ResultClass.done);
            }
        }

        public virtual async Task BreakDelete(string bkptno)
        {
            await _debugger.CmdAsync("-break-delete " + bkptno, ResultClass.done);
        }

        public virtual async Task BreakCondition(string bkptno, string expr)
        {
            if (string.IsNullOrWhiteSpace(expr))
            {
                expr = string.Empty;
            }
            string command = string.Format("-break-condition {0} {1}", bkptno, expr);
            await _debugger.CmdAsync(command, ResultClass.done);
        }

        public virtual IEnumerable<Guid> GetSupportedExceptionCategories()
        {
            return new Guid[0];
        }

        /// <summary>
        /// Adds a breakpoint which will be triggered when an exception is thrown and/or goes user-unhandled
        /// </summary>
        /// <param name="exceptionCategory">AD7 category for the execption</param>
        /// <param name="exceptionNames">[Optional] names of the exceptions to set a breakpoint on. If null, this sets an breakpoint for all 
        /// exceptions in the category. Note that this clear all previous exception breakpoints set in this category.</param>
        /// <param name="exceptionBreakpointState">Indicates when the exception breakpoint should fire</param>
        /// <returns>Task containing the exception breakpoint id's for the various set exceptions</returns>
        public virtual Task<IEnumerable<ulong>> SetExceptionBreakpoints(Guid exceptionCategory, /*OPTIONAL*/ IEnumerable<string> exceptionNames, ExceptionBreakpointState exceptionBreakpointState)
        {
            // NOTES: 
            // GDB /MI has no support for exceptions. Though they do have it through the non-MI through a 'catch' command. Example:
            //   catch throw MyException
            //   Catchpoint 3 (throw)
            //   =breakpoint-created,bkpt={number="3",type="breakpoint",disp="keep",enabled="y",addr="0xa1b5f830",what="exception throw",catch-type="throw",thread-groups=["i1"],regexp="MyException",times="0"}
            // Documentation: http://www.sourceware.org/gdb/onlinedocs/gdb/Set-Catchpoints.html#Set-Catchpoints
            // 
            // LLDB-MI has no support for exceptions. Though they do have it through the non-MI breakpoint command. Example:
            //   break set -F std::range_error
            // And they do have it in their API:
            //   SBTarget::BreakpointCreateForException
            throw new NotImplementedException();
        }

        public virtual Task RemoveExceptionBreakpoint(Guid exceptionCategory, IEnumerable<ulong> exceptionBreakpoints)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Decode properties from an exception-received event.
        /// </summary>
        /// <param name="miExceptionResult">Results object for the exception-received event</param>
        /// <param name="exceptionCategory">AD7 Exception Category to return</param>
        /// <param name="state">Exception state</param>
        public virtual void DecodeExceptionReceivedProperties(Results miExceptionResult, out Guid? exceptionCategory, out ExceptionBreakpointState state)
        {
            exceptionCategory = null;
            state = ExceptionBreakpointState.None;
        }

        #endregion

        #region Helpers

        #endregion

        #region Other

        abstract protected Task<Results> ThreadFrameCmdAsync(string command, ResultClass expectedResultClass, int threadId, uint frameLevel);
        abstract protected Task<Results> ThreadCmdAsync(string command, ResultClass expectedResultClass, int threadId);

        abstract public bool SupportsStopOnDynamicLibLoad();

        /// <summary>
        /// True if the underlying debugger can format frames itself
        /// </summary>
        public virtual bool SupportsFrameFormatting
        {
            get { return false; }
        }

        public virtual bool IsAsyncBreakSignal(Results results)
        {
            return (results.TryFindString("reason") == "signal-received" && results.TryFindString("signal-name") == "SIGINT");
        }

        public Results IsModuleLoad(string cmd)
        {
            Results results = null;
            if (cmd.StartsWith("library-loaded,", StringComparison.Ordinal))
            {
                MIResults res = new MIResults();
                results = res.ParseResultList(cmd.Substring(15));
            }
            return results;
        }

        abstract public bool AllowCommandsWhileRunning();

        public virtual bool CanDetach()
        {
            return true;
        }

        abstract public Task<List<ulong>> StartAddressesForLine(string file, uint line);

        /// <summary>
        /// Sets the gdb 'target-async' option to 'on'.
        /// </summary>
        /// <returns>[Required] Task to track when this is complete</returns>
        abstract public Task EnableTargetAsyncOption();

        #endregion
    }
}