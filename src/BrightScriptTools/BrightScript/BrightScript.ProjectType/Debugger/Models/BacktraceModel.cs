namespace BrightScript.Debugger.Models
{
    public class BacktraceModel
    {
        public int Position { get; set; }
        public string Function { get; set; }
        public string File { get; set; }
        public int Line { get; set; }
    }
}