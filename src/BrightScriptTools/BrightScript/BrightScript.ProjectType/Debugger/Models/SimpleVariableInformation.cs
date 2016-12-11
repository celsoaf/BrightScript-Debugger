using System.Threading.Tasks;
using BrightScript.Debugger.AD7;
using BrightScript.Debugger.Engine;

namespace BrightScript.Debugger.Models
{
    public class SimpleVariableInformation
    {
        public string Name { get; private set; }
        public string Value { get; private set; }
        public string TypeName { get; private set; }
        public bool IsParameter { get; private set; }

        public SimpleVariableInformation(string name, bool isParam = false, string value = null, string type = null)
        {
            Name = name;
            Value = value;
            TypeName = type;
            IsParameter = isParam;
        }

        internal async Task<VariableInformation> CreateMIDebuggerVariable(ThreadContext ctx, AD7Engine engine, AD7Thread thread)
        {
            VariableInformation vi = new VariableInformation(Name, ctx, engine, thread, IsParameter);
            await vi.Eval();
            return vi;
        }
    }
}