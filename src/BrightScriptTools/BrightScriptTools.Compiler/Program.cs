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
            TestScanner();

            TestParser();
        }

        private static void TestParser()
        {
            using (Stream file = File.Open("Main.brs", FileMode.Open, FileAccess.Read))
            {
                // parse input args, and open input file
                Scanner scanner = new Scanner(file);

                Parser parser = new Parser(scanner);
                parser.Parse();
            }

            Console.ReadLine();
        }

        private static void TestScanner()
        {
            using (Stream file = File.Open("Main.brs", FileMode.Open, FileAccess.Read))
            {
                // parse input args, and open input file
                Scanner scanner = new Scanner(file);
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
