using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace BrightScript.Language.Classification
{
    [Export(typeof(ITaggerProvider))]
    [ContentType(Constants.Language.ContentType)]
    [TagType(typeof(ClassificationTag))]
    internal sealed class BrightScriptClassifierProvider : ITaggerProvider
    {

        [Export]
        [Name(Constants.Language.ContentType)]
        [BaseDefinition("code")]
        internal static ContentTypeDefinition BrightScriptContentType = null;

        [Export]
        [FileExtension(Constants.Language.FileExtension)]
        [ContentType(Constants.Language.ContentType)]
        internal static FileExtensionToContentTypeDefinition BrightScriptFileType = null;

        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry = null;

        [Import]
        internal IBufferTagAggregatorFactoryService aggregatorFactory = null;

        [Import]
        private IStandardClassificationService standardClassifications = null;


        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {

            ITagAggregator<BrightScriptTokenTag> bsTagAggregator = 
                aggregatorFactory.CreateTagAggregator<BrightScriptTokenTag>(buffer);

            return new BrightScriptClassifier(buffer, bsTagAggregator, standardClassifications, ClassificationTypeRegistry) as ITagger<T>;
        }
    }
}