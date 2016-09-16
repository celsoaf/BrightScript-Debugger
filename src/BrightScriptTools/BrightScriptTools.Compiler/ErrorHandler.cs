using System.Collections.Generic;
using BrightScriptTools.GPlex.Parser;
using TooManyErrorsException = BrightScriptTools.Gppg.GPGen.Parser.TooManyErrorsException;

namespace BrightScriptTools.Compiler
{
    public class ErrorHandler
    {
        const int maxErrors = 50; // Will this be enough for all users?

        private List<Error> _errors = new List<Error>();
        int errNum;
        int wrnNum;

        LexSpan defaultSpan;
        internal LexSpan DefaultSpan
        {
            set { defaultSpan = value; }
            get { return (defaultSpan != null ? defaultSpan : new LexSpan(1, 1, 0, 0, 0, 0, null)); }
        }

        internal bool Errors { get { return errNum > 0; } }

        internal bool Warnings { get { return wrnNum > 0; } }

        public List<Error> SortedErrorList()
        {
            if (_errors.Count > 1) _errors.Sort();
            return _errors;
        }

        internal void AddError(int code, string msg, LexSpan spn)
        {
            if (spn == null)
                spn = DefaultSpan;
            this.AddError(new Error(code, msg, spn, false)); errNum++;
        }

        internal void AddWarning(int code, string msg, LexSpan spn)
        {
            if (spn == null)
                spn = DefaultSpan;
            this.AddError(new Error(code, msg, spn, true)); wrnNum++;
        }

        private void AddError(Error e)
        {
            _errors.Add(e);
            if (_errors.Count > maxErrors)
            {
                _errors.Add(new Error(1, "Too many errors, abandoning", e.Span, false));
                throw new TooManyErrorsException("Too many errors");
            }
        }
    }
}