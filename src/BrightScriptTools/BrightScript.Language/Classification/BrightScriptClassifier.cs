﻿//***************************************************************************
//
//    Copyright (c) Microsoft Corporation. All rights reserved.
//    This code is licensed under the Visual Studio SDK license terms.
//    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
//    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
//    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
//    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//***************************************************************************

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace BrightScript.Language.Classification
{
    internal sealed class BrightScriptClassifier : ITagger<ClassificationTag>
    {
        ITextBuffer _buffer;
        ITagAggregator<BrightScriptTokenTag> _aggregator;
        IDictionary<BrightScriptTokenTypes, IClassificationType> _bsTypes;

        /// <summary>
        /// Construct the classifier and define search tokens
        /// </summary>
        internal BrightScriptClassifier(ITextBuffer buffer, 
                               ITagAggregator<BrightScriptTokenTag> bsTagAggregator,
                               IStandardClassificationService standardClassifications,
                               IClassificationTypeRegistryService typeService)
        {
            _buffer = buffer;
            _aggregator = bsTagAggregator;
            _bsTypes = new Dictionary<BrightScriptTokenTypes, IClassificationType>();

            _bsTypes[BrightScriptTokenTypes.Cmnt] = standardClassifications.Comment;
            _bsTypes[BrightScriptTokenTypes.Keyword] = standardClassifications.Keyword;
            _bsTypes[BrightScriptTokenTypes.Literal] = standardClassifications.Literal;
            _bsTypes[BrightScriptTokenTypes.Opr] = standardClassifications.Operator;
            _bsTypes[BrightScriptTokenTypes.Number] = standardClassifications.NumberLiteral;
            _bsTypes[BrightScriptTokenTypes.Str] = standardClassifications.StringLiteral;
            _bsTypes[BrightScriptTokenTypes.Ident] = standardClassifications.Identifier;

            _bsTypes[BrightScriptTokenTypes.Funcs] = typeService.GetClassificationType("Funcs");
            _bsTypes[BrightScriptTokenTypes.Typs] = typeService.GetClassificationType("Typs");
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
