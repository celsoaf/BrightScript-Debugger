using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace RokuTelnet.Controls
{
    public class RichTextViewer : RichTextBox
    {
        public const string RichTextPropertyName = "RichText";

        public static readonly DependencyProperty RichTextProperty =
            DependencyProperty.Register(RichTextPropertyName,
                                        typeof(string),
                                        typeof(RichTextBox),
                                        new PropertyMetadata(
                                            new PropertyChangedCallback
                                                (RichTextPropertyChanged)));

        public RichTextViewer()
        {
            IsReadOnly = true;
            Background = new SolidColorBrush { Opacity = 0 };
            BorderThickness = new Thickness(0);

            Loaded += (s, e) => ScrollToEnd();
        }

        public string RichText
        {
            get { return (string)GetValue(RichTextProperty); }
            set { SetValue(RichTextProperty, value); }
        }

        private static void RichTextPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {

            var rt = dependencyObject as RichTextViewer;
            if (rt != null)
            {
                var doc = new FlowDocument();
                var value = dependencyPropertyChangedEventArgs.NewValue as string;
                if (value != null)
                {
                    using (StringReader reader = new StringReader(value))
                    {
                        string newLine;
                        while ((newLine = reader.ReadLine()) != null)
                        {
                            doc.Blocks.Add(BuildParagraph1(newLine));
                        }
                    }

                    rt.Document = doc;
                    rt.ScrollToEnd();
                }
            }
        }

        private static Paragraph BuildParagraph(string line)
        {
            line = line.Replace("[0m", "");
            if (line.StartsWith("\u001b[3"))
            {
                var color = line.Substring(3, 1);
                line = line.Substring(7);

                var p = new Paragraph(new Run(line));
                p.Foreground = GetColor(color);

                return p;
            }

            return new Paragraph(new Run(line));
        }

        private static Paragraph BuildParagraph1(string line)
        {
            line = line.Replace("\u001b]0;", "$");

            var p = new Paragraph();

            do
            {
                var idx = line.IndexOf("\u001b[");
                if (idx == -1)
                {
                    p.Inlines.Add(new Run(line));
                    line = string.Empty;
                }
                else if (idx == 0)
                {
                    if (line.Length > 3)
                    {
                        var color = line.Substring(3, 1);
                        line = line.Substring(line.IndexOf("m") + 1);
                        idx = line.IndexOf("\u001b[");
                        if (idx != -1)
                        {
                            var s = line.Substring(0, idx);
                            line = line.Substring(idx);
                            p.Inlines.Add(new Run(s) {Foreground = GetColor(color)});
                        }
                        else
                        {
                            p.Inlines.Add(new Run(line) {Foreground = GetColor(color)});
                            line = string.Empty;
                        }
                    }
                    else
                    {
                        line = string.Empty;
                    }
                }
                else
                {
                    var s = line.Substring(0, idx);
                    line = line.Substring(idx);
                    p.Inlines.Add(new Run(s));
                }
            } while (!string.IsNullOrEmpty(line));

            return p;
        }

        private static Brush GetColor(string index)
        {
            switch (index)
            {
                case "1":
                    return Brushes.Red;
                case "2":
                    return Brushes.LightGreen;
                case "3":
                    return Brushes.Yellow;
                case "4":
                    return Brushes.DeepSkyBlue;
                case "5":
                    return Brushes.Magenta;
                case "6":
                    return Brushes.Cyan;
                case "7":
                    return Brushes.LightCyan;
            }

            return Brushes.WhiteSmoke;
        }
    }
}