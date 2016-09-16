using BrightScriptTools.GPlex;
using BrightScriptTools.GPlex.Parser;

namespace BrightScriptTools.Compiler
{
    public partial class Parser : ShiftReduceParser<int, LexSpan>
    {
        internal ErrorHandler handler;


        public Parser(AbstractScanner<int, LexSpan> scanner, ErrorHandler handler)
            :base(scanner)
        {
            this.handler = handler;
        }
    }
}