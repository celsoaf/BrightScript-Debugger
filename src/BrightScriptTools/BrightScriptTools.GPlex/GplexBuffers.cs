// ==============================================================
// <auto-generated>
// This code automatically produced from an embedded resource.
// Do not edit this file, or it will become incompatible with 
// the specification from which it was generated.
// </auto-generated>
// ==============================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace BrightScriptTools.GPlex
{
// Code copied from GPLEX embedded resource
    [Serializable]
    public class BufferException : Exception
    {
        public BufferException() { }
        public BufferException(string message) : base(message) { }
        public BufferException(string message, Exception innerException)
            : base(message, innerException) { }
        protected BufferException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    public abstract class ScanBuff
    {
        private string fileNm;

        public const int EndOfFile = -1;
        public const int UnicodeReplacementChar = 0xFFFD;

        public bool IsFile { get { return (fileNm != null); } }
        public string FileName { get { return fileNm; } set { fileNm = value; } }

        public abstract int Pos { get; set; }
        public abstract int Read();
        public virtual void Mark() { }

        public abstract string GetString(int begin, int limit);

        public static ScanBuff GetBuffer(string source)
        {
            return new StringBuffer(source);
        }

        public static ScanBuff GetBuffer(IList<string> source)
        {
            return new LineBuffer(source);
        }

#if (!NOFILES)
        public static ScanBuff GetBuffer(Stream source)
        {
            return new BuildBuffer(source);
        }

#if (!BYTEMODE)
        public static ScanBuff GetBuffer(Stream source, int fallbackCodePage)
        {
            return new BuildBuffer(source, fallbackCodePage);
        }
#endif // !BYTEMODE
#endif // !NOFILES
    }

    #region Buffer classes

    // ==============================================================
    // =====  Definitions for various ScanBuff derived classes   ====
    // ==============================================================
    // ===============         String input          ================
    // ==============================================================

    /// <summary>
    /// This class reads characters from a single string as
    /// required, for example, by Visual Studio language services
    /// </summary>
    sealed class StringBuffer : ScanBuff
    {
        string str;        // input buffer
        int bPos;          // current position in buffer
        int sLen;

        public StringBuffer(string source)
        {
            this.str = source;
            this.sLen = source.Length;
            this.FileName = null;
        }

        public override int Read()
        {
            if (bPos < sLen) return str[bPos++];
            else if (bPos == sLen) { bPos++; return '\n'; }   // one strike, see new line
            else { bPos++; return EndOfFile; }                // two strikes and you're out!
        }

        public override string GetString(int begin, int limit)
        {
            //  "limit" can be greater than sLen with the BABEL
            //  option set.  Read returns a "virtual" EOL if
            //  an attempt is made to read past the end of the
            //  string buffer.  Without the guard any attempt 
            //  to fetch yytext for a token that includes the 
            //  EOL will throw an index exception.
            if (limit > sLen) limit = sLen;
            if (limit <= begin) return "";
            else return str.Substring(begin, limit - begin);
        }

        public override int Pos
        {
            get { return bPos; }
            set { bPos = value; }
        }

        public override string ToString() { return "StringBuffer"; }
    }

    // ==============================================================
    //  The LineBuff class contributed by Nigel Horspool, 
    //  nigelh@cs.uvic.cs
    // ==============================================================

    sealed class LineBuffer : ScanBuff
    {
        IList<string> line;    // list of source lines from a file
        int numLines;          // number of strings in line list
        string curLine;        // current line in that list
        int cLine;             // index of current line in the list
        int curLen;            // length of current line
        int curLineStart;      // position of line start in whole file
        int curLineEnd;        // position of line end in whole file
        int maxPos;            // max position ever visited in whole file
        int cPos;              // ordinal number of code in source

        // Constructed from a list of strings, one per source line.
        // The lines have had trailing '\n' characters removed.
        public LineBuffer(IList<string> lineList)
        {
            line = lineList;
            numLines = line.Count;
            cPos = curLineStart = 0;
            curLine = (numLines > 0 ? line[0] : "");
            maxPos = curLineEnd = curLen = curLine.Length;
            cLine = 1;
            FileName = null;
        }

        public override int Read()
        {
            if (cPos < curLineEnd)
                return curLine[cPos++ - curLineStart];
            if (cPos++ == curLineEnd)
                return '\n';
            if (cLine >= numLines)
                return EndOfFile;
            curLine = line[cLine];
            curLen = curLine.Length;
            curLineStart = curLineEnd + 1;
            curLineEnd = curLineStart + curLen;
            if (curLineEnd > maxPos)
                maxPos = curLineEnd;
            cLine++;
            return curLen > 0 ? curLine[0] : '\n';
        }

        // To speed up searches for the line containing a position
        private int cachedPosition;
        private int cachedIxdex;
        private int cachedLineStart;

        // Given a position pos within the entire source, the results are
        //   ix     -- the index of the containing line
        //   lstart -- the position of the first character on that line
        private void findIndex(int pos, out int ix, out int lstart)
        {
            if (pos >= cachedPosition)
            {
                ix = cachedIxdex; lstart = cachedLineStart;
            }
            else
            {
                ix = lstart = 0;
            }
            while (ix < numLines)
            {
                int len = line[ix].Length + 1;
                if (pos < lstart + len) break;
                lstart += len;
                ix++;
            }
            cachedPosition = pos;
            cachedIxdex = ix;
            cachedLineStart = lstart;
        }

        public override string GetString(int begin, int limit)
        {
            if (begin >= maxPos || limit <= begin) return "";
            int endIx, begIx, endLineStart, begLineStart;
            findIndex(begin, out begIx, out begLineStart);
            int begCol = begin - begLineStart;
            findIndex(limit, out endIx, out endLineStart);
            int endCol = limit - endLineStart;
            string s = line[begIx];
            if (begIx == endIx)
            {
                // the usual case, substring all on one line
                return (endCol <= s.Length) ?
                    s.Substring(begCol, endCol - begCol)
                    : s.Substring(begCol) + "\n";
            }
            // the string spans multiple lines, yuk!
            StringBuilder sb = new StringBuilder();
            if (begCol < s.Length)
                sb.Append(s.Substring(begCol));
            for (; ; )
            {
                sb.Append("\n");
                s = line[++begIx];
                if (begIx >= endIx) break;
                sb.Append(s);
            }
            if (endCol <= s.Length)
            {
                sb.Append(s.Substring(0, endCol));
            }
            else
            {
                sb.Append(s);
                sb.Append("\n");
            }
            return sb.ToString();
        }

        public override int Pos
        {
            get { return cPos; }
            set
            {
                cPos = value;
                findIndex(cPos, out cLine, out curLineStart);
                // cLine should be the *next* line after curLine.
                curLine = (cLine < numLines ? line[cLine++] : "");
                curLineEnd = curLineStart + curLine.Length;
            }
        }

        public override string ToString() { return "LineBuffer"; }
    }

#if (!NOFILES)
    // ==============================================================
    // =====     class BuildBuff : for unicode text files    ========
    // ==============================================================

    class BuildBuffer : ScanBuff
    {
        // Double buffer for char stream.
        class BufferElement
        {
            StringBuilder bldr = new StringBuilder();
            StringBuilder next = new StringBuilder();
            int minIx;
            int maxIx;
            int brkIx;
            bool appendToNext;

            internal BufferElement() { }

            internal int MaxIndex { get { return maxIx; } }
            // internal int MinIndex { get { return minIx; } }

            internal char this[int index]
            {
                get
                {
                    if (index < minIx || index >= maxIx)
                        throw new BufferException("Index was outside data buffer");
                    else if (index < brkIx)
                        return bldr[index - minIx];
                    else
                        return next[index - brkIx];
                }
            }

            internal void Append(char[] block, int count)
            {
                maxIx += count;
                if (appendToNext)
                    this.next.Append(block, 0, count);
                else
                {
                    this.bldr.Append(block, 0, count);
                    brkIx = maxIx;
                    appendToNext = true;
                }
            }

            internal string GetString(int start, int limit)
            {
                if (limit <= start)
                    return "";
                if (start >= minIx && limit <= maxIx)
                    if (limit < brkIx) // String entirely in bldr builder
                        return bldr.ToString(start - minIx, limit - start);
                    else if (start >= brkIx) // String entirely in next builder
                        return next.ToString(start - brkIx, limit - start);
                    else // Must do a string-concatenation
                        return
                            bldr.ToString(start - minIx, brkIx - start) +
                            next.ToString(0, limit - brkIx);
                else
                    throw new BufferException("String was outside data buffer");
            }

            internal void Mark(int limit)
            {
                if (limit > brkIx + 16) // Rotate blocks
                {
                    StringBuilder temp = bldr;
                    bldr = next;
                    next = temp;
                    next.Length = 0;
                    minIx = brkIx;
                    brkIx = maxIx;
                }
            }
        }

        BufferElement data = new BufferElement();

        int bPos;            // Postion index in the StringBuilder
        BlockReader NextBlk; // Delegate that serves char-arrays;

        private string EncodingName
        {
            get
            {
                StreamReader rdr = NextBlk.Target as StreamReader;
                return (rdr == null ? "raw-bytes" : rdr.CurrentEncoding.BodyName);
            }
        }

        public BuildBuffer(Stream stream)
        {
            FileStream fStrm = (stream as FileStream);
            if (fStrm != null) FileName = fStrm.Name;
            NextBlk = BlockReaderFactory.Raw(stream);
        }

#if (!BYTEMODE)
        public BuildBuffer(Stream stream, int fallbackCodePage)
        {
            FileStream fStrm = (stream as FileStream);
            if (fStrm != null) FileName = fStrm.Name;
            NextBlk = BlockReaderFactory.Get(stream, fallbackCodePage);
        }
#endif

        /// <summary>
        /// Marks a conservative lower bound for the buffer,
        /// allowing space to be reclaimed.  If an application 
        /// needs to call GetString at arbitrary past locations 
        /// in the input stream, Mark() is not called.
        /// </summary>
        public override void Mark() { data.Mark(bPos - 2); }

        public override int Pos
        {
            get { return bPos; }
            set { bPos = value; }
        }


        /// <summary>
        /// Read returns the ordinal number of the next char, or 
        /// EOF (-1) for an end of stream.  Note that the next
        /// code point may require *two* calls of Read().
        /// </summary>
        /// <returns></returns>
        public override int Read()
        {
            //
            //  Characters at positions 
            //  [data.offset, data.offset + data.bldr.Length)
            //  are available in data.bldr.
            //
            if (bPos < data.MaxIndex)
            {
                // ch0 cannot be EOF
                return (int)data[bPos++];
            }
            else // Read from underlying stream
            {
                // Experimental code, blocks of page size
                char[] chrs = new char[4096];
                int count = NextBlk(chrs, 0, 4096);
                if (count == 0)
                    return EndOfFile;
                else
                {
                    data.Append(chrs, count);
                    return (int)data[bPos++];
                }
            }
        }

        public override string GetString(int begin, int limit)
        {
            return data.GetString(begin, limit);
        }

        public override string ToString()
        {
            return "StringBuilder buffer, encoding: " + this.EncodingName;
        }
    }

    // =============== End ScanBuff-derived classes ==================

    public delegate int BlockReader(char[] block, int index, int number);

    // A delegate factory, serving up a delegate that
    // reads a block of characters from the underlying
    // encoded stream, via a StreamReader object.
    //
    public static class BlockReaderFactory
    {
        public static BlockReader Raw(Stream stream)
        {
            return delegate(char[] block, int index, int number)
            {
                byte[] b = new byte[number];
                int count = stream.Read(b, 0, number);
                int i = 0;
                int j = index;
                for (; i < count; i++, j++)
                    block[j] = (char)b[i];
                return count;
            };
        }

#if (!BYTEMODE)
        public static BlockReader Get(Stream stream, int fallbackCodePage)
        {
            Encoding encoding;
            int preamble = Preamble(stream);

            if (preamble != 0)  // There is a valid BOM here!
                encoding = Encoding.GetEncoding(preamble);
            else if (fallbackCodePage == -1) // Fallback is "raw" bytes
                return Raw(stream);
            else if (fallbackCodePage != -2) // Anything but "guess"
                encoding = Encoding.GetEncoding(fallbackCodePage);
            else // This is the "guess" option
            {
                int guess = new Guesser(stream).GuessCodePage();
                stream.Seek(0, SeekOrigin.Begin);
                if (guess == -1) // ==> this is a 7-bit file
                    encoding = Encoding.ASCII;
                else if (guess == 65001)
                    encoding = Encoding.UTF8;
                else             // ==> use the machine default
                    encoding = Encoding.Default;
            }
            StreamReader reader = new StreamReader(stream, encoding);
            return reader.Read;
        }

        static int Preamble(Stream stream)
        {
            int b0 = stream.ReadByte();
            int b1 = stream.ReadByte();

            if (b0 == 0xfe && b1 == 0xff)
                return 1201; // UTF16BE
            if (b0 == 0xff && b1 == 0xfe)
                return 1200; // UTF16LE

            int b2 = stream.ReadByte();
            if (b0 == 0xef && b1 == 0xbb && b2 == 0xbf)
                return 65001; // UTF8
            //
            // There is no unicode preamble, so we
            // return denoter for the machine default.
            //
            stream.Seek(0, SeekOrigin.Begin);
            return 0;
        }
#endif // !BYTEMODE
    }
#endif // !NOFILES
    #endregion Buffer classes

    // ==============================================================
    // ============      class CodePageHandling         =============
    // ==============================================================
#if (!NOFILES)
    public static class CodePageHandling
    {
        public static int GetCodePage(string option)
        {
            string command = option.ToUpperInvariant();
            if (command.StartsWith("CodePage:", StringComparison.OrdinalIgnoreCase))
                command = command.Substring(9);
            try
            {
                if (command.Equals("RAW"))
                    return -1;
                else if (command.Equals("GUESS"))
                    return -2;
                else if (command.Equals("DEFAULT"))
                    return 0;
                else if (char.IsDigit(command[0]))
                    return int.Parse(command, CultureInfo.InvariantCulture);
                else
                {
                    Encoding enc = Encoding.GetEncoding(command);
                    return enc.CodePage;
                }
            }
            catch (FormatException)
            {
                Console.Error.WriteLine(
                    "Invalid format \"{0}\", using machine default", option);
            }
            catch (ArgumentException)
            {
                Console.Error.WriteLine(
                    "Unknown code page \"{0}\", using machine default", option);
            }
            return 0;
        }
    }
#region guesser
#if (!BYTEMODE)
    // ==============================================================
    // ============          Encoding Guesser           =============
    // ==============================================================

    /// <summary>
    /// This class provides a simple finite state automaton that
    /// scans the file looking for (1) valid UTF-8 byte patterns,
    /// (2) bytes >= 0x80 which are not part of a UTF-8 sequence.
    /// The method then guesses whether it is UTF-8 or maybe some 
    /// local machine default encoding.  This works well for the
    /// various Latin encodings.
    /// </summary>
    internal class Guesser
    {
        ScanBuff buffer;

        public int GuessCodePage() { return Scan(); }

        const int maxAccept = 10;
        const int initial = 0;
        const int eofNum = 0;
        const int goStart = -1;
        const int INITIAL = 0;
        const int EndToken = 0;

        #region user code
        /* 
         *  Reads the bytes of a file to determine if it is 
         *  UTF-8 or a single-byte code page file.
         */
        public long utfX;
        public long uppr;
        #endregion user code

        int state;
        int currentStart = startState[0];
        int code;

        #region ScannerTables
        static int[] startState = new int[] { 11, 0 };

        #region CharacterMap
        static sbyte[] map = new sbyte[256] {
/*     '\0' */ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
/*   '\x10' */ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
/*   '\x20' */ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
/*      '0' */ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
/*      '@' */ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
/*      'P' */ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
/*      '`' */ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
/*      'p' */ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
/*   '\x80' */ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 
/*   '\x90' */ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 
/*   '\xA0' */ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 
/*   '\xB0' */ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 
/*   '\xC0' */ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 
/*   '\xD0' */ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 
/*   '\xE0' */ 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 
/*   '\xF0' */ 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5 };
        #endregion

        static sbyte[][] nextState = new sbyte[][] {
            new sbyte[] {0, 0, 0, 0, 0, 0},
            new sbyte[] {-1, -1, 10, -1, -1, -1},
            new sbyte[] {-1, -1, -1, -1, -1, -1},
            new sbyte[] {-1, -1, 8, -1, -1, -1},
            new sbyte[] {-1, -1, 5, -1, -1, -1},
            new sbyte[] {-1, -1, 6, -1, -1, -1},
            new sbyte[] {-1, -1, 7, -1, -1, -1},
            null,
            new sbyte[] {-1, -1, 9, -1, -1, -1},
            null,
            null,
            new sbyte[] {-1, 1, 2, 3, 4, 2}
        };


        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        // Reason for suppression: cannot have self-reference in array initializer.
        static Guesser()
        {
            nextState[7] = nextState[2];
            nextState[9] = nextState[2];
            nextState[10] = nextState[2];
        }

        int NextState()
        {
            if (code == ScanBuff.EndOfFile)
                return eofNum;
            else
                return nextState[state][map[code]];
        }
        #endregion

        public Guesser(System.IO.Stream file) { SetSource(file); }

        public void SetSource(System.IO.Stream source)
        {
            this.buffer = new BuildBuffer(source);
            code = buffer.Read();
        }

        int Scan()
        {
            for (; ; )
            {
                int next;
                state = currentStart;
                while ((next = NextState()) == goStart)
                    code = buffer.Read();

                state = next;
                code = buffer.Read();

                while ((next = NextState()) > eofNum)
                {
                    state = next;
                    code = buffer.Read();
                }
                if (state <= maxAccept)
                {
                    #region ActionSwitch
#pragma warning disable 162
                    switch (state)
                    {
                        case eofNum:
                            switch (currentStart)
                            {
                                case 11:
                                    if (utfX == 0 && uppr == 0) return -1; /* raw ascii */
                                    else if (uppr * 10 > utfX) return 0;   /* default code page */
                                    else return 65001;                     /* UTF-8 encoding */
                                    break;
                            }
                            return EndToken;
                        case 1: // Recognized '{Upper128}',	Shortest string "\xC0"
                        case 2: // Recognized '{Upper128}',	Shortest string "\x80"
                        case 3: // Recognized '{Upper128}',	Shortest string "\xE0"
                        case 4: // Recognized '{Upper128}',	Shortest string "\xF0"
                            uppr++;
                            break;
                        case 5: // Recognized '{Utf8pfx4}{Utf8cont}',	Shortest string "\xF0\x80"
                            uppr += 2;
                            break;
                        case 6: // Recognized '{Utf8pfx4}{Utf8cont}{2}',	Shortest string "\xF0\x80\x80"
                            uppr += 3;
                            break;
                        case 7: // Recognized '{Utf8pfx4}{Utf8cont}{3}',	Shortest string "\xF0\x80\x80\x80"
                            utfX += 3;
                            break;
                        case 8: // Recognized '{Utf8pfx3}{Utf8cont}',	Shortest string "\xE0\x80"
                            uppr += 2;
                            break;
                        case 9: // Recognized '{Utf8pfx3}{Utf8cont}{2}',	Shortest string "\xE0\x80\x80"
                            utfX += 2;
                            break;
                        case 10: // Recognized '{Utf8pfx2}{Utf8cont}',	Shortest string "\xC0\x80"
                            utfX++;
                            break;
                        default:
                            break;
                    }
#pragma warning restore 162
                    #endregion
                }
            }
        }
    } // end class Guesser
    
#endif // !BYTEMODE
#endregion
#endif // !NOFILES

// End of code copied from embedded resource
}
