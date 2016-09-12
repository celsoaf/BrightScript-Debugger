using System.Runtime.CompilerServices;
using Microsoft;
using Microsoft.VisualStudio.Text;

namespace BrightScript.Language.Text
{
    /// <summary>
 /// This class will be changed to non-static once I merge this branch with the branch that has the Core class that
 /// holds everything.
 /// </summary>
    internal class SourceTextCache
    {
        internal SourceText Get(ITextSnapshot textSnapshot)
        {
            Requires.NotNull(textSnapshot, nameof(textSnapshot));

            SourceText sourceText = null;
            if (this.sources.TryGetValue(textSnapshot, out sourceText))
            {
                return sourceText;
            }

            sourceText = new SourceText(textSnapshot.GetText());
            this.sources.Add(textSnapshot, sourceText);

            return sourceText;
        }

        private ConditionalWeakTable<ITextSnapshot, SourceText> sources =
            new ConditionalWeakTable<ITextSnapshot, SourceText>();
    }
}