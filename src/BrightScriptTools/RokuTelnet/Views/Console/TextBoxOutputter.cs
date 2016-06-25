using System;
using System.IO;
using System.Text;
using System.Windows.Controls;

namespace RokuTelnet.Views.Console
{
    public class TextBoxOutputter : TextWriter
    {
        private StringBuilder textBox = null;

        public event Action TextChange; 

        public TextBoxOutputter(StringBuilder output)
        {
            textBox = output;
        }

        public override void Write(char value)
        {
            base.Write(value);
            textBox.Append(value.ToString());

            TextChange?.Invoke();
        }

        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }
}