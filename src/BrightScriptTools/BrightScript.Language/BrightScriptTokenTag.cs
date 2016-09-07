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
using System.Diagnostics;
using System.Linq;
using BrightScriptTools.Compiler;
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
        IDictionary<int, BrightScriptTokenTypes> _bsTypes;

        internal BrightScriptTokenTagger(ITextBuffer buffer)
        {
            _buffer = buffer;
            
            _bsTypes = new Dictionary<int, BrightScriptTokenTypes>();

            _bsTypes[(int)TokensColor.cmnt] = BrightScriptTokenTypes.Cmnt;
            _bsTypes[(int)TokensColor.funcs] = BrightScriptTokenTypes.Funcs;
            _bsTypes[(int)TokensColor.ident] = BrightScriptTokenTypes.Ident;
            _bsTypes[(int)TokensColor.number] = BrightScriptTokenTypes.Number;
            _bsTypes[(int)TokensColor.str] = BrightScriptTokenTypes.Str;
            _bsTypes[(int)TokensColor.opr] = BrightScriptTokenTypes.Opr;
            _bsTypes[(int)TokensColor.keyword] = BrightScriptTokenTypes.Keyword;
            _bsTypes[(int)TokensColor.type] = BrightScriptTokenTypes.Typs;
            _bsTypes[(int)TokensColor.literal] = BrightScriptTokenTypes.Literal;
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
                int startPos = containingLine.Start.Position;
                var line = containingLine.GetText();

                var scanner = new ScannerColor();
                scanner.SetSource(line, 0);

                int token;

                while ((token = scanner.yylex()) != (int)TokensColor.EOF)
                {
                    if (_bsTypes.ContainsKey(token))
                    {
                        var tokenSpan = new SnapshotSpan(curSpan.Snapshot, new Span(startPos + scanner.GetPos(), scanner.yyleng));
                        if (tokenSpan.IntersectsWith(curSpan))
                            yield return new TagSpan<BrightScriptTokenTag>(tokenSpan, new BrightScriptTokenTag(_bsTypes[token]));

                    }
                }
            }
            
        }
    }
}
