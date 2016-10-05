using System.IO;
using BrightScriptTools.GPlex;

namespace BrightScriptTools.Compiler
{
    public class LexSpan : IMerge<LexSpan>
    {
        public int startLine;       // start line of span
        public int startColumn;     // start column of span
        public int endLine;         // end line of span
        public int endColumn;       // end column of span
        public int startIndex;      // start position in the buffer
        public int endIndex;        // end position in the buffer
        public string text;
        public int token;
        internal ScanBuff buffer;     // reference to the buffer

        public int Length { get { return endIndex - startIndex; } }

        public LexSpan() { }
        public LexSpan(int sl, int sc, int el, int ec, int sp, int ep, ScanBuff bf)
        { startLine = sl; startColumn = sc; endLine = el; endColumn = ec; startIndex = sp; endIndex = ep; buffer = bf; }

        /// <summary>
        /// This method implements the IMerge interface
        /// </summary>
        /// <param name="end">The last span to be merged</param>
        /// <returns>A span from the start of 'this' to the end of 'end'</returns>
        public LexSpan Merge(LexSpan end)
        {
            return new LexSpan(startLine, startColumn, end.endLine, end.endColumn, startIndex, end.endIndex, buffer);
        }

        /// <summary>
        /// Get a short span from the first line of this span.
        /// </summary>
        /// <param name="idx">Starting index</param>
        /// <param name="len">Length of span</param>
        /// <returns></returns>
        internal GPlex.Parser.LexSpan FirstLineSubSpan(int idx, int len)
        {
            //if (this.endLine != this.startLine) 
            //    throw new Exception("Cannot index into multiline span");

            return new GPlex.Parser.LexSpan(
                this.startLine, this.startColumn + idx, this.startLine, this.startColumn + idx + len,
                this.startIndex, this.endIndex, this.buffer);
        }

        internal bool IsInitialized { get { return buffer != null; } }

        internal void StreamDump(TextWriter sWtr)
        {
            // int indent = sCol;
            int savePos = buffer.Pos;
            string str = buffer.GetString(startIndex, endIndex);
            //for (int i = 0; i < indent; i++)
            //    sWtr.Write(' ');
            sWtr.WriteLine(str);
            buffer.Pos = savePos;
            sWtr.Flush();
        }

        //internal void ConsoleDump()
        //{
        //    int savePos = buffer.Pos;
        //    string str = buffer.GetString(startIndex, endIndex);
        //    Console.WriteLine(str);
        //    buffer.Pos = savePos; 
        //}

        public override string ToString()
        {
            return buffer.GetString(startIndex, endIndex);
        }
    }
}