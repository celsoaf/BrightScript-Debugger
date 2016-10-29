using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BrightScript.Debugger.Core;
using BrightScript.Debugger.Engine;

namespace BrightScript.Debugger
{
    public static class MIDebugCommandDispatcher
    {
        private readonly static List<DebuggedProcess> s_processes = new List<DebuggedProcess>();

        public static Task<string> ExecuteCommand(string command)
        {
            DebuggedProcess lastProcess;
            lock (s_processes)
            {
                if (s_processes.Count == 0)
                {
                    throw new InvalidOperationException(MICoreResources.Error_NoMIDebuggerProcess);
                }

                lastProcess = s_processes[s_processes.Count - 1];
            }
            return ExecuteCommand(command, lastProcess);
        }

        internal static Task<string> ExecuteCommand(string command, DebuggedProcess process)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentNullException("command");

            if (process == null)
            {
                throw new InvalidOperationException(MICoreResources.Error_NoMIDebuggerProcess);
            }

            command = command.Trim();

            if (command[0] == '-')
            {
                return ExecuteMiCommand(process, command);
            }
            else
            {
                return process.ConsoleCmdAsync(command);
            }
        }

        private static async Task<string> ExecuteMiCommand(DebuggedProcess lastProcess, string command)
        {
            Results results = await lastProcess.CmdAsync(command, ResultClass.None);
            return results.ToString();
        }

        internal static void AddProcess(DebuggedProcess process)
        {
            process.DebuggerExitEvent += process_DebuggerExitEvent;

            lock (s_processes)
            {
                s_processes.Add(process);
            }
        }

        private static void process_DebuggerExitEvent(object sender, EventArgs e)
        {
            DebuggedProcess debuggedProcess = sender as DebuggedProcess;
            if (debuggedProcess != null)
            {
                debuggedProcess.DebuggerExitEvent -= process_DebuggerExitEvent;
                lock (s_processes)
                {
                    s_processes.Remove(debuggedProcess);
                }
            }
        }
    }
}