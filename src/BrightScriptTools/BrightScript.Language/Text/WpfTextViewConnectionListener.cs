﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using BrightScript.Language.Shared;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Differencing;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace BrightScript.Language.Text
{
    [Export(typeof(IWpfTextViewConnectionListener))]
    [ContentType(Constants.Language.ContentType)]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    internal class WpfTextViewConnectionListener : IWpfTextViewConnectionListener
    {
        [Import]
        private ISingletons singletons = null;

        private static readonly Dictionary<IWpfTextView, TextView> viewMap = new Dictionary<IWpfTextView, TextView>();

        public void SubjectBuffersConnected(IWpfTextView textView, ConnectionReason reason, Collection<ITextBuffer> subjectBuffers)
        {
            List<ITextBuffer> textBuffers = IgnoreLeftTextBufferInInlineDiffView(textView, subjectBuffers);

            if (textBuffers.Count == 0)
            {
                return;
            }

            if (!viewMap.ContainsKey(textView))
            {
                TextView internalTextView = new TextView(textView, this.singletons);
                viewMap.Add(textView, internalTextView);
                internalTextView.Connect(textBuffers[0]);
            }
        }

        private static List<ITextBuffer> IgnoreLeftTextBufferInInlineDiffView(IWpfTextView textView, Collection<ITextBuffer> subjectBuffers)
        {
            List<ITextBuffer> textBuffers = new List<ITextBuffer>();
            bool isInlineDiffView = textView.Roles.Contains(DifferenceViewerRoles.InlineViewTextViewRole);

            foreach (ITextBuffer subjectBuffer in subjectBuffers)
            {
                if (!(isInlineDiffView && subjectBuffer != textView.TextDataModel.DocumentBuffer))
                {
                    textBuffers.Add(subjectBuffer);
                }
            }

            return textBuffers;
        }

        public void SubjectBuffersDisconnected(IWpfTextView textView, ConnectionReason reason, Collection<ITextBuffer> subjectBuffers)
        {
            List<ITextBuffer> textBuffers = IgnoreLeftTextBufferInInlineDiffView(textView, subjectBuffers);

            if (textBuffers.Count == 0)
            {
                return;
            }

            TextView internalTextView;
            if (viewMap.TryGetValue(textView, out internalTextView))
            {
                internalTextView.Disconnect(textBuffers[0]);
                viewMap.Remove(textView);
            }
        }
    }
}