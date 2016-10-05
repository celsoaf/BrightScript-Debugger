using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrightScriptTools.GPlex;

namespace BrightScriptTools.Compiler
{
    public class Program
    {
        static void Main(string[] args)
        {
            //TestScanner(@"TestData\TestColor.brs");

            TestParser(@"TestData\TestParser.brs");

            Console.ReadLine();
        }

        private static void TestParser(string path)
        {
            using (Stream file = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                ErrorHandler handler = new ErrorHandler();
                // parse input args, and open input file
                Scanner scanner = new ASTScanner(file);
                scanner.SetHandler(handler);

                Parser parser = new Parser(scanner, handler);
                if (!parser.Parse())
                {
                    handler.SortedErrorList().ToList().ForEach(e =>
                    {
                        Console.WriteLine(e.Message);
                    });
                }
                else
                {
                    var ast = parser.GetASTRoot();
                }
            }
        }

        private static void TestScanner(string path)
        {
            using (Stream file = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                // parse input args, and open input file
                var scanner = new ScannerColor(file);
                Tokens token;
                do
                {
                    token = (Tokens) scanner.yylex();
                    var text = scanner.yytext;
                    Console.WriteLine("text {0} token {1}", text, token);
                } while (token != Tokens.EOF);
            }
        }
    }
}
