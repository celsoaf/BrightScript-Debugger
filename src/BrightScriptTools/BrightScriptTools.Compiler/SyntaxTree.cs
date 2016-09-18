using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using BrightScriptTools.Compiler.AST;

namespace BrightScriptTools.Compiler
{
    public class SyntaxTree
    {
        public SyntaxTree(ChunkNode root, List<Token> tokens, List<StatementNode> statementNodeList, ImmutableList<Error> errorList)
        {
            this.Root = root;
            this.Tokens = tokens;
            this.ErrorList = errorList;
            this.StatementNodeList = statementNodeList;
        }

        public ChunkNode Root { get; }
        public ImmutableList<Error> ErrorList { get; }
        public List<Token> Tokens { get; }
        public List<StatementNode> StatementNodeList { get; }

        public static SyntaxTree Create(TextReader reader)
        {
            return CreateFromString(reader.ReadToEnd());
        }

        // For testing
        public static SyntaxTree Create(string filename)
        {
            return CreateFromSteam(File.Open(filename, FileMode.Open));
        }

        public static SyntaxTree CreateFromString(string program)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(program)))
            {
                return CreateFromSteam(stream);
            }
        }

        public static SyntaxTree CreateFromSteam(Stream stream)
        {
            ErrorHandler handler = new ErrorHandler();
            // parse input args, and open input file
            ASTScanner scanner = new ASTScanner(stream);
            scanner.SetHandler(handler);

            Parser parser = new Parser(scanner, handler);
            parser.Parse();

            var statementNodeList = new List<StatementNode>();
            ChunkNode root = new ChunkNode();
            return new SyntaxTree(root, scanner.GetTokens(), statementNodeList, handler.SortedErrorList().ToImmutableList());
        }

        public SyntaxNode GetNodeAt(int position)
        {
            throw new NotImplementedException();
        }
    }
}