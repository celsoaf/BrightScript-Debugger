using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrightScriptTools.Compiler
{
    public class Program
    {
        static void Main(string[] args)
        {
            Stream file = File.Open("Main.brs", FileMode.Open, FileAccess.Read);
            // parse input args, and open input file
            Scanner scanner = new Scanner(file);
            Parser parser = new Parser(scanner);
            parser.Parse();
            // and so on ...
        }
    }
}
