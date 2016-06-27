using System.Collections.Generic;
using System.IO;

namespace BrightScriptDebug.Compiler
{
    public class TelnetScanner : Scanner
    {
        private const int TOKEN_LENGHT = 50;

        private readonly List<string> _tokenValues = new List<string>();

        public TelnetScanner(Stream file)
            : base(file)
        {
            
        }

        public override int yylex()
        {
            _tokenValues.Add(yytext);

            if (_tokenValues.Count > TOKEN_LENGHT)
                _tokenValues.RemoveAt(0);

            return base.yylex(); ;
        }

        public string GetToken(int index)
        {
            return _tokenValues[_tokenValues.Count - (index)];
        }
    }
}