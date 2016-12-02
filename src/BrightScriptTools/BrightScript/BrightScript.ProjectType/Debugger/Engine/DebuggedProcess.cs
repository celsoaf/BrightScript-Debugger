using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BrightScript.Debugger.AD7;
using BrightScript.Debugger.Commands;
using BrightScript.Debugger.Interfaces;
using BrightScript.Debugger.Models;
using BrightScript.Debugger.Transport;
using Microsoft.VisualStudio.Debugger.Interop;

namespace BrightScript.Debugger.Engine
{
    internal class DebuggedProcess : IDebuggedProcess, ITransportCallback
    {
        private readonly string _ip;
        private readonly int _port;
        private readonly IEngineCallback _engineCallback;
        private bool _connected;
        private ITransport _transport;

        public DebuggedProcess(string ip, int port, IEngineCallback engineCallback, IWorkerThread workerThread, AD7Engine engine)
        {
            _ip = ip;
            _port = port;
            _engineCallback = engineCallback;
            WorkerThread = workerThread;
            Engine = engine;

            CommandFactory = new CommandFactory(this);

            // we do NOT have real Win32 process IDs, so we use a guid
            AD_PROCESS_ID pid = new AD_PROCESS_ID();
            pid.ProcessIdType = (int)enum_AD_PROCESS_ID.AD_PROCESS_ID_GUID;
            pid.guidProcessId = Guid.NewGuid();
            this.Id = pid;
        }

        public AD_PROCESS_ID Id { get; }
        public AD7Engine Engine { get; }
        public ICommandFactory CommandFactory { get; }
        public IWorkerThread WorkerThread { get; }
        public IThreadCache ThreadCache { get; }

        public async Task Execute(DebuggedThread thread)
        {
            throw new NotImplementedException();
        }

        public async Task<List<SimpleVariableInformation>> GetParameterInfoOnly(AD7Thread thread, ThreadContext ctx)
        {
            throw new NotImplementedException();
        }

        public async Task<List<VariableInformation>> GetLocalsAndParameters(AD7Thread thread, ThreadContext ctx)
        {
            throw new NotImplementedException();
        }

        public void OnPostedOperationError(object sender, Exception e)
        {
            throw new NotImplementedException();
        }

        public async Task Initialize()
        {
            
        }

        public async Task ResumeFromLaunch()
        {
            Connect();
            _connected = true;
        }

        private void Connect()
        {
            _transport = new TcpTransport();
            _transport.Init(_ip, _port, this);
        }

        public async Task CmdDetach()
        {
            throw new NotImplementedException();
        }

        public async Task<List<ulong>> StartAddressesForLine(string file, uint line)
        {
            throw new NotImplementedException();
        }

        public async Task Step(int threadId, enum_STEPKIND kind, enum_STEPUNIT unit)
        {
            throw new NotImplementedException();
        }

        public void Terminate()
        {
            _transport.Close();
        }

        public void Close()
        {
            _transport.Close();
        }

        public void OnStdOutLine(string line)
        {
            throw new NotImplementedException();
        }

        public void OnDebuggerProcessExit(string exitCode)
        {
            throw new NotImplementedException();
        }
    }
}