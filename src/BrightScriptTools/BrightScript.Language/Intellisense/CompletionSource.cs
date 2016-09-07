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
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

namespace BrightScript.Language.Intellisense
{
    [Export(typeof(ICompletionSourceProvider))]
    [ContentType(Constants.Language.ContentType)]
    [Name("brightScriptCompletion")]
    class BrightScriptCompletionSourceProvider : ICompletionSourceProvider
    {
        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            return new BrightScriptCompletionSource(textBuffer);
        }
    }

    class BrightScriptCompletionSource : ICompletionSource
    {
        private ITextBuffer _buffer;
        private bool _disposed = false;
        
        public BrightScriptCompletionSource(ITextBuffer buffer)
        {
            _buffer = buffer;
        }

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            if (_disposed)
                throw new ObjectDisposedException("BrightScriptCompletionSource");

            List<Completion> completions = new List<Completion>()
            {
                new Completion("boolean"),
                new Completion("integer"),
                new Completion("float"),
                new Completion("double"),
                new Completion("string"),
                new Completion("object"),
                new Completion("interface"),
                new Completion("invalid"),
                new Completion("brsub"),
                new Completion("void")
            };
            
            ITextSnapshot snapshot = _buffer.CurrentSnapshot;
            var triggerPoint = (SnapshotPoint)session.GetTriggerPoint(snapshot);

            if (triggerPoint == null)
                return;

            var line = triggerPoint.GetContainingLine();
            SnapshotPoint start = triggerPoint;

            while (start > line.Start && !char.IsWhiteSpace((start - 1).GetChar()))
            {
                start -= 1;
            }

            var applicableTo = snapshot.CreateTrackingSpan(new SnapshotSpan(start, triggerPoint), SpanTrackingMode.EdgeInclusive);

            completionSets.Add(new CompletionSet("All", "All", applicableTo, completions, Enumerable.Empty<Completion>()));
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}

