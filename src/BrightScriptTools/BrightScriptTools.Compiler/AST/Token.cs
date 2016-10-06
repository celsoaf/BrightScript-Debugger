using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrightScriptTools.Compiler.AST
{
    public class Token : SyntaxNodeOrToken
    {
        public LexSpan Lex { get; }

        public int FullStart { get; private set; }
        public string Text { get; private set; }

        public List<Trivia> LeadingTrivia { get; } //TODO: change to Immutable List


        private Token(SyntaxKind kind, string text, List<Trivia> trivia, int fullStart, int start)
        {
            this.Kind = kind;
            this.Text = text;
            this.LeadingTrivia = trivia == null ? new List<Trivia>() : trivia;
            this.FullStart = fullStart;
            this.Start = start;
            this.Length = Text.Length;
        }

        public Token(SyntaxKind kind, LexSpan lex, List<Trivia> trivia=null)
            : this(kind, lex.text, trivia, lex.startIndex, lex.startIndex)
        {
            Lex = lex;
        }

        public static Token CreateMissingToken(int position)
        {
            return new Token(SyntaxKind.MissingToken, "", Enumerable.Empty<Trivia>().ToList(), position, position);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Kind.ToString());
            sb.Append("\t");
            sb.Append(Text);

            return sb.ToString();
        }

        public override System.Collections.Immutable.ImmutableList<SyntaxNodeOrToken> Children
        {
            get
            {
                return null;
            }
        }
    }
}