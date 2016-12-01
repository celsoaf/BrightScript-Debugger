namespace BrightScript.Debugger.Models
{
    public class RegisterGroup
    {
        public readonly string Name;
        internal int Count { get; set; }

        public RegisterGroup(string name)
        {
            Name = name;
            Count = 0;
        }
    }
}