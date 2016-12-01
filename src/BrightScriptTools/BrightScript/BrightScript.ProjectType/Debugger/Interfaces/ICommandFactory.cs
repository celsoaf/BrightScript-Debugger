using System.Threading.Tasks;

namespace BrightScript.Debugger.Interfaces
{
    internal interface ICommandFactory
    {
        uint Radix { get; }
        bool SupportsFrameFormatting { get; }

        Task<bool> SetRadix(uint radix);
        Task<bool> SetJustMyCode(bool enabled);
        Task<string> DataEvaluateExpression(string expr, int threadId, uint frame);

        bool CanDetach();
    }
}