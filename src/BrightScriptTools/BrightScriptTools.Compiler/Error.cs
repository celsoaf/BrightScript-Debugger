using System;
using BrightScriptTools.GPlex.Parser;

namespace BrightScriptTools.Compiler
{
    public class Error : IComparable<Error>
    {
        internal const int minErr = 50;
        internal const int minWrn = 100;

        internal int code;
        public bool IsWarn { get; private set; }
        public string Message { get; private set; }
        public LexSpan Span { get; private set; }


        internal Error(int code, string msg, LexSpan spn, bool wrn)
        {
            this.code = code;
            IsWarn = wrn;
            Message = msg;
            Span = spn;
        }

        public int CompareTo(Error r)
        {
            if (Span.startLine < r.Span.startLine) return -1;
            else if (Span.startLine > r.Span.startLine) return 1;
            else if (Span.startColumn < r.Span.startColumn) return -1;
            else if (Span.startColumn > r.Span.startColumn) return 1;
            else return 0;
        }

    }
}