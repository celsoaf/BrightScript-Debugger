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
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace BrightScript.Language
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

    public class BrightScriptTokenTag : ITag 
    {
        public BrightScriptTokenTypes type { get; private set; }

        public BrightScriptTokenTag(BrightScriptTokenTypes type)
        {
            this.type = type;
        }
    }

    internal sealed class BrightScriptTokenTagger : ITagger<BrightScriptTokenTag>
    {

        ITextBuffer _buffer;
        IDictionary<string, BrightScriptTokenTypes> _bsTypes;

        internal BrightScriptTokenTagger(ITextBuffer buffer)
        {
            _buffer = buffer;
            _bsTypes = new Dictionary<string, BrightScriptTokenTypes>();
            _bsTypes["ook!"] = BrightScriptTokenTypes.OokExclamation;
            _bsTypes["ook."] = BrightScriptTokenTypes.OokPeriod;
            _bsTypes["ook?"] = BrightScriptTokenTypes.OokQuestion;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged
        {
            add { }
            remove { }
        }

        public IEnumerable<ITagSpan<BrightScriptTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {

            foreach (SnapshotSpan curSpan in spans)
            {
                ITextSnapshotLine containingLine = curSpan.Start.GetContainingLine();
                int curLoc = containingLine.Start.Position;
                string[] tokens = containingLine.GetText().ToLower().Split(' ');

                foreach (string bsToken in tokens)
                {
                    if (_bsTypes.ContainsKey(bsToken))
                    {
                        var tokenSpan = new SnapshotSpan(curSpan.Snapshot, new Span(curLoc, bsToken.Length));
                        if( tokenSpan.IntersectsWith(curSpan) ) 
                            yield return new TagSpan<BrightScriptTokenTag>(tokenSpan, 
                                                                  new BrightScriptTokenTag(_bsTypes[bsToken]));
                    }

                    //add an extra char location because of the space
                    curLoc += bsToken.Length + 1;
                }
            }
            
        }
    }
}
