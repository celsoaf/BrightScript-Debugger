using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace BrightScript.Language.Classification
{
    [Export(typeof(ITaggerProvider))]
    [ContentType(BrightScriptConstants.ContentType)]
    [TagType(typeof(BrightScriptTokenTag))]
    internal sealed class BrightScriptTokenTagProvider : ITaggerProvider
    {

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return new BrightScriptTokenTagger(buffer) as ITagger<T>;
        }
    }
}