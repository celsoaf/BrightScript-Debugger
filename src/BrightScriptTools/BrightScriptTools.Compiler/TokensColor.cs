namespace BrightScriptTools.Compiler
{
    public enum TokensColor
    {
        error = 2, EOF = 3, keyword = 4, ident = 5, number = 6,
        str = 7, cmnt = 8, type = 9, funcs = 10, reserved = 11, opr = 12,
        bar = 13, dot = 14, semi = 15, star = 16, lt = 17, gt = 18,
        comma = 19, slash = 20, lBrac = 21, rBrac = 22, lPar = 23, rPar = 24,
        lBrace = 25, rBrace = 26, sub = 27, function = 28, comment = 29, maxParseToken = 30,
        EOL = 31, errTok = 32, repErr = 33
    }
}