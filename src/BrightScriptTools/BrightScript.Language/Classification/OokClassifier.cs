//***************************************************************************
//
//    Copyright (c) Microsoft Corporation. All rights reserved.
//    This code is licensed under the Visual Studio SDK license terms.
//    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
//    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
//    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
//    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//***************************************************************************

using BrightScript.Language;

namespace OokLanguage
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(ITaggerProvider))]
    [ContentType(BrightScriptConstants.ContentType)]
    [TagType(typeof(ClassificationTag))]
    internal sealed class OokClassifierProvider : ITaggerProvider
    {

        [Export]
        [Name(BrightScriptConstants.ContentType)]
        [BaseDefinition("code")]
        internal static ContentTypeDefinition BrightScriptContentType = null;

        [Export]
        [FileExtension(BrightScriptConstants.FileExtention)]
        [ContentType(BrightScriptConstants.ContentType)]
        internal static FileExtensionToContentTypeDefinition BrightScriptFileType = null;

        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry = null;

        [Import]
        internal IBufferTagAggregatorFactoryService aggregatorFactory = null;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {

            ITagAggregator<BrightScriptTokenTag> bsTagAggregator = 
                                            aggregatorFactory.CreateTagAggregator<BrightScriptTokenTag>(buffer);

            return new OokClassifier(buffer, bsTagAggregator, ClassificationTypeRegistry) as ITagger<T>;
        }
    }

    internal sealed class OokClassifier : ITagger<ClassificationTag>
    {
        ITextBuffer _buffer;
        ITagAggregator<BrightScriptTokenTag> _aggregator;
        IDictionary<BrightScriptTokenTypes, IClassificationType> _bsTypes;

        /// <summary>
        /// Construct the classifier and define search tokens
        /// </summary>
        internal OokClassifier(ITextBuffer buffer, 
                               ITagAggregator<BrightScriptTokenTag> bsTagAggregator, 
                               IClassificationTypeRegistryService typeService)
        {
            _buffer = buffer;
            _aggregator = bsTagAggregator;
            _bsTypes = new Dictionary<BrightScriptTokenTypes, IClassificationType>();
            _bsTypes[BrightScriptTokenTypes.OokExclamation] = typeService.GetClassificationType("ook!");
            _bsTypes[BrightScriptTokenTypes.OokPeriod] = typeService.GetClassificationType("ook.");
            _bsTypes[BrightScriptTokenTypes.OokQuestion] = typeService.GetClassificationType("ook?");
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Search the given span for any instances of classified tags
        /// </summary>
        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            foreach (var tagSpan in _aggregator.GetTags(spans))
            {
                var tagSpans = tagSpan.Span.GetSpans(spans[0].Snapshot);
                yield return 
                    new TagSpan<ClassificationTag>(tagSpans[0], 
                                                   new ClassificationTag(_bsTypes[tagSpan.Tag.type]));
            }
        }
    }
}
