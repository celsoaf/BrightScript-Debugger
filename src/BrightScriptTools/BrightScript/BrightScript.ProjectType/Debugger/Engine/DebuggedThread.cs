using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BrightScript.Debugger.AD7;
using BrightScript.Debugger.Enums;
using BrightScript.Debugger.Models;

namespace BrightScript.Debugger.Engine
{
    public class DebuggedThread
    {
        public DebuggedThread(int id, AD7Engine engine)
        {
            Id = id;
            Name = "";
            TargetId = (uint)id;
            AD7Thread ad7Thread = new AD7Thread(engine, this);
            Client = ad7Thread;
        }

        public int Id { get; private set; }
        public uint TargetId { get; set; }
        public Object Client { get; private set; }      // really AD7Thread
        public bool Alive { get; set; }
        public bool Default { get; set; }
        public string Name { get; set; }
    }

    public class ThreadCache
    {
        private List<DebuggedThread> _threadList;
        private Dictionary<int, List<ThreadContext>> _stackFrames;
        private Dictionary<int, ThreadContext> _topContext;    // can retrieve the top frame without walking the stack
        private Dictionary<int, List<VariableModel>> _variables;
        private bool _stateChange;             // indicates that a thread has been created/destroyed since last thread-info
        private bool _full;                    // indicates whether the cache has already been filled via -thread-info
        private ISampleEngineCallback _callback;
        private DebuggedProcess _debugger;
        private List<DebuggedThread> _deadThreads;
        private List<DebuggedThread> _newThreads;

        internal ThreadCache(ISampleEngineCallback callback, DebuggedProcess debugger)
        {
            _threadList = new List<DebuggedThread>();
            _stackFrames = new Dictionary<int, List<ThreadContext>>();
            _topContext = new Dictionary<int, ThreadContext>();
            _variables = new Dictionary<int, List<VariableModel>>();
            _stateChange = true;
            _callback = callback;
            _debugger = debugger;
            _full = false;
            debugger.RunModeEvent += SendThreadEvents;
        }

        internal async Task<DebuggedThread[]> GetThreads()
        {
            //if (_stateChange) // if new threads 
            //{
            //    await CollectThreadsInfo(0);
            //}
            lock (_threadList)
            {
                return _threadList.ToArray();
            }
        }

        internal async Task<DebuggedThread> GetThread(int id)
        {
            DebuggedThread[] threads = await GetThreads();
            foreach (var t in threads)
            {
                if (t.Id == id)
                {
                    return t;
                }
            }
            return null;
        }

        internal async Task<List<ThreadContext>> StackFrames(DebuggedThread thread)
        {
            lock (_threadList)
            {
                if (!_threadList.Contains(thread))
                {
                    return null;    // thread must be dead
                }
                if (_stackFrames.ContainsKey(thread.Id))
                {
                    return _stackFrames[thread.Id];
                }
            }
            List<ThreadContext> stack = null;
            try
            {
                stack = await WalkStack(thread);
            }
            catch (UnexpectedMIResultException)
            {
                _debugger.Logger.WriteLine("Stack walk failed on thread: " + thread.TargetId);
                _stateChange = true;   // thread may have been deleted. Force a resync
            }
            lock (_threadList)
            {
                _stackFrames[thread.Id] = stack;
                _topContext[thread.Id] = (stack != null && stack.Count > 0) ? stack[0] : null;
                return _stackFrames[thread.Id];
            }
        }

        internal async Task<ThreadContext> GetThreadContext(DebuggedThread thread)
        {
            lock (_threadList)
            {
                if (_topContext.ContainsKey(thread.Id))
                {
                    return _topContext[thread.Id];
                }
                if (_full)
                {
                    return null;    // no context available for this thread
                }
            }
            return await CollectThreadsInfo(thread.Id);
        }

        internal void MarkDirty()
        {
            lock (_threadList)
            {
                _topContext.Clear();
                _stackFrames.Clear();
                _full = false;
            }
        }

        internal void ThreadEvent(int id, bool deleted)
        {
            lock (_threadList)
            {
                var thread = _threadList.Find(t => t.Id == id);
                if ((thread != null) == deleted)
                {
                    _stateChange = true;
                }
            }
        }

        private async Task<List<ThreadContext>> WalkStack(DebuggedThread thread)
        {
            List<ThreadContext> stack = null;
            TupleValue[] frameinfo = await _debugger.MICommandFactory.StackListFrames(thread.Id, 0, 1000);
            if (frameinfo == null)
            {
                _debugger.Logger.WriteLine("Failed to get frame info");
            }
            else
            {
                stack = new List<ThreadContext>();
                foreach (var frame in frameinfo)
                {
                    stack.Add(CreateContext(frame));
                }
            }
            return stack;
        }

        private ThreadContext CreateContext(TupleValue frame)
        {
            ulong? pc = frame.TryFindAddr("addr");
            MITextPosition textPosition = MITextPosition.TryParse(frame);
            string func = frame.TryFindString("func");
            uint level = frame.FindUint("level");
            string from = frame.TryFindString("from");

            return new ThreadContext(pc, textPosition, func, level, from);
        }

        private async Task<ThreadContext> CollectThreadsInfo(int cxtThreadId)
        {
            await _debugger.CmdAsync(DebuggerCommandEnum.bt.ToString(), ResultClass.None);
            //await _debugger.CmdAsync(DebuggerCommandEnum.var.ToString(), ResultClass.None);

            return await GetThreadContext(FindThread(cxtThreadId));
        }

        internal void SendThreadEvents(object sender, EventArgs e)
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
                    _callback.OnThreadStart(newt);
                }
            if (deadThreads != null)
                foreach (var dead in deadThreads)
                {
                    // Send the destroy event outside the lock
                    _callback.OnThreadExit(dead, 0);
                }
        }

        public DebuggedThread FindThread(int id, out bool bNew)
        {
            DebuggedThread newthread;
            bNew = false;
            var thread = _threadList.Find(t => t.Id == id);
            if (thread != null)
                return thread;
            // thread not found, so create it, and return it
            newthread = new DebuggedThread(id, _debugger.Engine);
            newthread.Default = false;
            _threadList.Add(newthread);
            bNew = true;
            return newthread;
        }

        public DebuggedThread FindThread(int id)
        {
            bool bnew;
            var thread = FindThread(id, out bnew);

            if (bnew)
            {
                if (_newThreads == null)
                {
                    _newThreads = new List<DebuggedThread>();
                }
                _newThreads.Add(thread);
            }

            return thread;
        }

        internal void SetThreadStack(int id, Results results)
        {
            var bt = results.Find("stack") as ResultListValue;

            var stack = new List<ThreadContext>();
            foreach (var frame in bt.FindAll<TupleValue>("frame"))
            {
                stack.Add(CreateContext(frame));
            }
            _stackFrames[id] = stack;
            _topContext[id] = stack.FirstOrDefault();
        }

        internal void SetVariables(int id, List<VariableModel> variables)
        {
            _variables[id] = variables;
        }

        internal List<VariableModel> GetVariables(int id)
        {
            if (_variables.ContainsKey(id))
            {
                return _variables[id];
            }

            return null;
        }
    }
}