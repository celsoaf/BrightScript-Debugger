using BrightScriptTools.GPlex;
using BrightScriptTools.GPlex.Parser;

namespace BrightScriptDebug.Compiler
{
    public partial class Parser : ShiftReduceParser<int, LexSpan>
    {
        public Parser(AbstractScanner<int, LexSpan> scanner)
            :base(scanner)
        {
            
        }
    }
}