using BrightScript.Debugger.Engine;
using Microsoft.VisualStudio.Debugger.Interop;

namespace BrightScript.Debugger.Models
{
    public class MITextPosition
    {
        public string FileName { get; private set; }
        public TEXT_POSITION BeginPosition { get; private set; }
        public TEXT_POSITION EndPosition { get; private set; }

        public MITextPosition(string filename, TEXT_POSITION beginPosition, TEXT_POSITION endPosition)
        {
            this.FileName = filename;
            this.BeginPosition = beginPosition;
            this.EndPosition = endPosition;
        }

        public MITextPosition(string filename, uint line)
        {
            this.FileName = filename;
            this.BeginPosition = new Microsoft.VisualStudio.Debugger.Interop.TEXT_POSITION() { dwLine = line - 1 };
            this.EndPosition = this.BeginPosition;
        }

        public string GetFileExtension()
        {
            int lastDotIndex = this.FileName.LastIndexOf('.');
            if (lastDotIndex < 0)
                return string.Empty;
            if (this.FileName.IndexOf('\\', lastDotIndex) >= 0)
                return string.Empty;

            return this.FileName.Substring(lastDotIndex);
        }
    }
}