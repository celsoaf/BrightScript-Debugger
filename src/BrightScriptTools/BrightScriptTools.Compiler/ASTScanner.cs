using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using BrightScriptTools.Compiler.AST;

namespace BrightScriptTools.Compiler
{
    public class ASTScanner : Scanner
    {
        private readonly List<Token> _tokenList = new List<Token>();
        
        public ASTScanner(Stream file)
            : base(file)
        {
        }

        public override int yylex()
        {
            var token = base.yylex();

            yylloc = GetTokenSpan(token);
            _tokenList.Add(GetToken(token));

            return token;
        }

        public List<Token> GetTokens()
        {
            return _tokenList;
        }
    }
}