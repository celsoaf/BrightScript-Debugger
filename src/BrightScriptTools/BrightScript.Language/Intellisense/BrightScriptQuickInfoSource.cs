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

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using BrightScript.Language.Classification;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace BrightScript.Language.Intellisense
{
    /// <summary>
    /// Factory for quick info sources
    /// </summary>
    [Export(typeof(IQuickInfoSourceProvider))]
    [ContentType(Constants.Language.ContentType)]
    [Name("brightScriptQuickInfo")]
    class BrightScriptQuickInfoSourceProvider : IQuickInfoSourceProvider
    {

        [Import]
        IBufferTagAggregatorFactoryService aggService = null;

        public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            return new BrightScriptQuickInfoSource(textBuffer, aggService.CreateTagAggregator<BrightScriptTokenTag>(textBuffer));
        }
    }

    /// <summary>
    /// Provides QuickInfo information to be displayed in a text buffer
    /// </summary>
    class BrightScriptQuickInfoSource : IQuickInfoSource
    {
        private ITagAggregator<BrightScriptTokenTag> _aggregator;
        private ITextBuffer _buffer;
        private bool _disposed = false;


        public BrightScriptQuickInfoSource(ITextBuffer buffer, ITagAggregator<BrightScriptTokenTag> aggregator)
        {
            _aggregator = aggregator;
            _buffer = buffer;
        }

        /// <summary>
        /// Determine which pieces of Quickinfo content should be displayed
        /// </summary>
        public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> quickInfoContent, out ITrackingSpan applicableToSpan)
        {
            applicableToSpan = null;

            if (_disposed)
                throw new ObjectDisposedException("TestQuickInfoSource");

            var triggerPoint = (SnapshotPoint) session.GetTriggerPoint(_buffer.CurrentSnapshot);

            if (triggerPoint == null)
                return;

            foreach (IMappingTagSpan<BrightScriptTokenTag> curTag in _aggregator.GetTags(new SnapshotSpan(triggerPoint, triggerPoint)))
            {
                if (curTag.Tag.type == TokenTypes.Typs)
                {
                    var tagSpan = curTag.Span.GetSpans(_buffer).First();
                    applicableToSpan = _buffer.CurrentSnapshot.CreateTrackingSpan(tagSpan, SpanTrackingMode.EdgeExclusive);
                    quickInfoContent.Add("BrightScript type");
                }
                else if (curTag.Tag.type == TokenTypes.Cmnt)
                {
                    var tagSpan = curTag.Span.GetSpans(_buffer).First();
                    applicableToSpan = _buffer.CurrentSnapshot.CreateTrackingSpan(tagSpan, SpanTrackingMode.EdgeExclusive);
                    quickInfoContent.Add("BrightScript comment");
                }
                else if (curTag.Tag.type == TokenTypes.Funcs)
                {
                    var tagSpan = curTag.Span.GetSpans(_buffer).First();
                    applicableToSpan = _buffer.CurrentSnapshot.CreateTrackingSpan(tagSpan, SpanTrackingMode.EdgeExclusive);
                    quickInfoContent.Add("BrightScript functions");
                }
                else if (curTag.Tag.type == TokenTypes.Ident)
                {
                    var tagSpan = curTag.Span.GetSpans(_buffer).First();
                    applicableToSpan = _buffer.CurrentSnapshot.CreateTrackingSpan(tagSpan, SpanTrackingMode.EdgeExclusive);
                    quickInfoContent.Add("BrightScript identifier");
                }
                else if (curTag.Tag.type == TokenTypes.Keyword)
                {
                    var tagSpan = curTag.Span.GetSpans(_buffer).First();
                    applicableToSpan = _buffer.CurrentSnapshot.CreateTrackingSpan(tagSpan, SpanTrackingMode.EdgeExclusive);
                    quickInfoContent.Add("BrightScript keyword");
                }
                else if (curTag.Tag.type == TokenTypes.Number)
                {
                    var tagSpan = curTag.Span.GetSpans(_buffer).First();
                    applicableToSpan = _buffer.CurrentSnapshot.CreateTrackingSpan(tagSpan, SpanTrackingMode.EdgeExclusive);
                    quickInfoContent.Add("BrightScript number");
                }
                else if (curTag.Tag.type == TokenTypes.Str)
                {
                    var tagSpan = curTag.Span.GetSpans(_buffer).First();
                    applicableToSpan = _buffer.CurrentSnapshot.CreateTrackingSpan(tagSpan, SpanTrackingMode.EdgeExclusive);
                    quickInfoContent.Add("BrightScript string");
                }
                else if (curTag.Tag.type == TokenTypes.Literal)
                {
                    var tagSpan = curTag.Span.GetSpans(_buffer).First();
                    applicableToSpan = _buffer.CurrentSnapshot.CreateTrackingSpan(tagSpan, SpanTrackingMode.EdgeExclusive);
                    quickInfoContent.Add("BrightScript literal");
                }
            }
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}

