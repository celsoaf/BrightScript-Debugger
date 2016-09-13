using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;

namespace BrightScript.Language.Errors
{
    internal class ErrorListItem : ErrorTask
    {
        public ErrorListItem(SnapshotSpan errorSpan)
        {
            this.ErrorSpan = errorSpan;
        }

        public SnapshotSpan ErrorSpan { get; }
    }
}