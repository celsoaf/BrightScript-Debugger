using BrightScript.ToolWindows.Enums;

namespace BrightScript.ToolWindows.Models
{
    public class EventModel
    {
        public EventModel(EventType eventType, EventKey eventKey, string args = null)
        {
            EventType = eventType;
            EventKey = eventKey;
            Args = args;
        }

        public EventType EventType { get; private set; }
        public EventKey EventKey { get; private set; }
        public string Args { get; private set; }
    }
}