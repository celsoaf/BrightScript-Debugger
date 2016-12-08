using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrightScript.Debugger.AD7;
using BrightScript.Debugger.Interfaces;
using BrightScript.Debugger.Models;

namespace BrightScript.Debugger.Engine
{
    internal class ThreadCache : IThreadCache
    {
        private readonly IEngineCallback _engineCallback;
        private readonly AD7Engine _engine;
        private readonly ICommandFactory _commandFactory;
        private List<DebuggedThread> _threadList;
        private Dictionary<int, List<ThreadContext>> _stackFrames;
        private Dictionary<int, ThreadContext> _topContext;    // can retrieve the top frame without walking the stack
        private Dictionary<int, List<SimpleVariableInformation>> _variables = new Dictionary<int, List<SimpleVariableInformation>>();

        private List<DebuggedThread> _deadThreads;
        private List<DebuggedThread> _newThreads;

        public ThreadCache(IEngineCallback engineCallback, AD7Engine engine, ICommandFactory commandFactory)
        {
            _engineCallback = engineCallback;
            _engine = engine;
            _commandFactory = commandFactory;

            _threadList = new List<DebuggedThread>();
            _stackFrames = new Dictionary<int, List<ThreadContext>>();
            _topContext = new Dictionary<int, ThreadContext>();
        }

        public async Task<DebuggedThread[]> GetThreads()
        {
            return _threadList.ToArray();
        }

        public DebuggedThread FindThread(int id)
        {
            var thread = _threadList.FirstOrDefault(t => t.Id == id);
            if (thread != null) return thread;

            thread = new DebuggedThread(id, _engine);
            thread.Default = false;
            _threadList.Add(thread);
            if (_newThreads == null)
                _newThreads = new List<DebuggedThread>();
            _newThreads.Add(thread);

            return thread;
        }

        public async Task<ThreadContext> GetThreadContext(DebuggedThread thread)
        {
            lock (_threadList)
            {
                if (_topContext.ContainsKey(thread.Id))
                    return _topContext[thread.Id];
            }

            var frames = await GetStackFrames(thread);

            return frames
                    .OrderBy(s => s.Level)
                    .FirstOrDefault();
        }

        private async Task<List<ThreadContext>> GetStackFrames(DebuggedThread thread)
        {
            var frames = await _commandFactory.GetStackTrace();

            SetStackFrames(thread.Id, frames);
            return frames;
        }

        public async Task<List<ThreadContext>> StackFrames(DebuggedThread thread)
        {
            lock (_threadList)
            {
                if (_stackFrames.ContainsKey(thread.Id))
                    return _stackFrames[thread.Id];
            }

            return await GetStackFrames(thread);
        }

        public void MarkDirty()
        {
            lock (_threadList)
            {
                _topContext.Clear();
                _stackFrames.Clear();
                _variables.Clear();
            }
        }

        public void SendThreadEvents()
        {
            List<DebuggedThread> deadThreads;
            List<DebuggedThread> newThreads;
            lock (_threadList)
            {
                deadThreads = _deadThreads;
                _deadThreads = null;
                newThreads = _newThreads;
                _newThreads = null;
            }
            if (newThreads != null)
                foreach (var newt in newThreads)
                {
                    _engineCallback.OnThreadStart(newt);
                }
            if (deadThreads != null)
                foreach (var dead in deadThreads)
                {
                    // Send the destroy event outside the lock
                    _engineCallback.OnThreadExit(dead, 0);
                }
        }

        public void SetStackFrames(int id, List<ThreadContext> frames)
        {
            lock (_threadList)
            {
                _stackFrames[id] = frames;
                _topContext[id] = frames
                                    .OrderBy(s => s.Level)
                                    .FirstOrDefault();
            }
        }

        public void SetVariables(int id, List<SimpleVariableInformation> variables)
        {
            lock (_threadList)
            {
                _variables[id] = variables;
            }
        }

        public SimpleVariableInformation GetVariable(int id, string name)
        {
            lock (_threadList)
                if (_variables.ContainsKey(id))
                    return _variables[id].FirstOrDefault(v => v.Name == name);

            return null;
        }
    }
}