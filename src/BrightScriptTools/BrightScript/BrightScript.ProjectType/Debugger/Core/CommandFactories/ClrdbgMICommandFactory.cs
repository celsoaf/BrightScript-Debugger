﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BrightScript.Debugger.Engine;
using Microsoft.VisualStudio.Debugger.Interop;

namespace BrightScript.Debugger.Core.CommandFactories
{
    internal class ClrdbgMICommandFactory : MICommandFactory
    {
        private readonly static Guid s_exceptionCategory_CLR = new Guid("449EC4CC-30D2-4032-9256-EE18EB41B62B");
        private readonly static Guid s_exceptionCategory_MDA = new Guid("6ECE07A9-0EDE-45C4-8296-818D8FC401D4");
        private readonly static ReadOnlyCollection<Guid> s_exceptionCategories = new ReadOnlyCollection<Guid>(new Guid[] { s_exceptionCategory_CLR, s_exceptionCategory_MDA });

        public override string Name
        {
            get { return "CLRDBG"; }
        }

        public override bool SupportsStopOnDynamicLibLoad()
        {
            return false;
        }

        // CLRDBG supports frame formatting itself
        override public bool SupportsFrameFormatting
        {
            get { return true; }
        }

        public override bool AllowCommandsWhileRunning()
        {
            return true;
        }

        public override bool CanDetach()
        {
            // clrdbg doesn't support detach yet
            return false;
        }

        public override async Task<bool> SetJustMyCode(bool enabled)
        {
            string command = "-gdb-set just-my-code " + (enabled ? "1" : "0");
            Results results = await _debugger.CmdAsync(command, ResultClass.None);
            return results.ResultClass == ResultClass.done;
        }

        public override Task<TupleValue[]> StackListArguments(PrintValues printValues, int threadId, uint lowFrameLevel, uint hiFrameLevel)
        {
            // CLRDBG supports stack frame formatting, so this should not be used
            throw new NotImplementedException();
        }

        protected override async Task<Results> ThreadFrameCmdAsync(string command, ResultClass expectedResultClass, int threadId, uint frameLevel)
        {
            string threadFrameCommand = string.Format(@"{0} --thread {1} --frame {2}", command, threadId, frameLevel);

            return await _debugger.CmdAsync(threadFrameCommand, expectedResultClass);
        }

        protected override async Task<Results> ThreadCmdAsync(string command, ResultClass expectedResultClass, int threadId)
        {
            string threadCommand = string.Format(@"{0} --thread {1}", command, threadId);

            return await _debugger.CmdAsync(threadCommand, expectedResultClass);
        }
        public override Task<List<ulong>> StartAddressesForLine(string file, uint line)
        {
            return Task.FromResult<List<ulong>>(null);
        }

        public override Task EnableTargetAsyncOption()
        {
            // clrdbg is always in target-async mode
            return Task.FromResult((object)null);
        }

        public override IEnumerable<Guid> GetSupportedExceptionCategories()
        {
            return s_exceptionCategories;
        }

        public override async Task<IEnumerable<ulong>> SetExceptionBreakpoints(Guid exceptionCategory, /*OPTIONAL*/ IEnumerable<string> exceptionNames, ExceptionBreakpointState exceptionBreakpointState)
        {
            List<string> commandTokens = new List<string>();
            commandTokens.Add("-break-exception-insert");

            if (exceptionCategory == s_exceptionCategory_MDA)
            {
                commandTokens.Add("--mda");
            }
            else if (exceptionCategory != s_exceptionCategory_CLR)
            {
                throw new ArgumentOutOfRangeException("exceptionCategory");
            }

            if (exceptionBreakpointState.HasFlag(ExceptionBreakpointState.BreakThrown))
            {
                if (exceptionBreakpointState.HasFlag(ExceptionBreakpointState.BreakUserHandled))
                    commandTokens.Add("throw+user-unhandled");
                else
                    commandTokens.Add("throw");
            }
            else
            {
                if (exceptionBreakpointState.HasFlag(ExceptionBreakpointState.BreakUserHandled))
                    commandTokens.Add("user-unhandled");
                else
                    commandTokens.Add("unhandled");
            }

            if (exceptionNames == null)
                commandTokens.Add("*");
            else
                commandTokens.AddRange(exceptionNames);

            string command = string.Join(" ", commandTokens);

            Results results = await _debugger.CmdAsync(command, ResultClass.done);
            ResultValue bkpt;
            if (results.TryFind("bkpt", out bkpt))
            {
                if (bkpt is ValueListValue)
                {
                    ValueListValue list = bkpt as ValueListValue;
                    return list.Content.Select((x) => x.FindAddr("number"));
                }
                else
                {
                    return new ulong[1] { bkpt.FindAddr("number") };
                }
            }
            else
            {
                return new ulong[0];
            }
        }

        public override Task RemoveExceptionBreakpoint(Guid exceptionCategory, IEnumerable<ulong> exceptionBreakpointIds)
        {
            string breakpointIds = string.Join(" ", exceptionBreakpointIds.Select(x => x.ToString(CultureInfo.InvariantCulture)));

            string command = "-break-exception-delete " + breakpointIds;
            return _debugger.CmdAsync(command, ResultClass.done);
        }

        public override void DecodeExceptionReceivedProperties(Results miExceptionResult, out Guid? exceptionCategory, out ExceptionBreakpointState state)
        {
            string category = miExceptionResult.FindString("exception-category");
            if (category == "mda")
            {
                exceptionCategory = s_exceptionCategory_MDA;
            }
            else
            {
                Debug.Assert(category == "clr");
                exceptionCategory = s_exceptionCategory_CLR;
            }

            string stage = miExceptionResult.FindString("exception-stage");
            switch (stage)
            {
                case "throw":
                    state = ExceptionBreakpointState.BreakThrown;
                    break;

                case "user-unhandled":
                    state = ExceptionBreakpointState.BreakUserHandled;
                    break;

                case "unhandled":
                    state = ExceptionBreakpointState.None;
                    break;

                default:
                    Debug.Fail("Unknown exception-stage value");
                    state = ExceptionBreakpointState.None;
                    break;
            }
        }

        override public async Task Terminate()
        {
            string command = "-exec-abort";
            await _debugger.CmdAsync(command, ResultClass.done);
        }

        public override async Task<Results> VarCreate(string expression, int threadId, uint frameLevel, enum_EVALFLAGS dwFlags, ResultClass resultClass = ResultClass.done)
        {
            string command = string.Format("-var-create - * \"{0}\" --evalFlags {1}", expression, (uint)dwFlags);
            Results results = await ThreadFrameCmdAsync(command, resultClass, threadId, frameLevel);

            return results;
        }

        public override async Task<Results> VarListChildren(string variableReference, enum_DEBUGPROP_INFO_FLAGS dwFlags, ResultClass resultClass = ResultClass.done)
        {
            // Limit the number of children expanded to 1000 in case memory is uninitialized
            string command = string.Format("-var-list-children --simple-values \"{0}\" --propertyInfoFlags {1} 0 1000", variableReference, (uint)dwFlags);
            Results results = await _debugger.CmdAsync(command, resultClass);

            return results;
        }
    }
}