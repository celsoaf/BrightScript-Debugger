using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrightScriptDebug.Compiler;
using BrightScriptTools.GPlex;

namespace BrightScriptTools.Compiler
{
    public class Program
    {
        static void Main(string[] args)
        {
            //TestScanner(@"TestData\TestColor.brs");

            TestParser(@"TestData\output.txt");

            Console.ReadLine();
        }

        private static void TestParser(string path)
        {
            using (Stream file = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                // parse input args, and open input file
                Scanner scanner = new Scanner(file);

                Parser parser = new Parser(scanner);
                parser.Parse();
            }
        }
    }
}
