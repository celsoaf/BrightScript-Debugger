using System;
using BrightScript.Debugger.AD7;

namespace BrightScript.Debugger.Models
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
}