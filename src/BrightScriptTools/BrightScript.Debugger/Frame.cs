namespace BrightScript.Debugger
{
    internal class Frame
    {
        private string m_func;
        private string m_src;
        private string m_line;

        public Frame(string funcName, string src, string line)
        {
            m_func = funcName;
            m_src = src;
            m_line = line;
        }

        public string GetFunc()
        {
            return m_func;
        }

        public string GetSource()
        {
            return m_src;
        }

        public string GetLine()
        {
            return m_line;
        }
    }
}