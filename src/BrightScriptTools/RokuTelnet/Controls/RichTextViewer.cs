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
                            doc.Blocks.Add(BuildParagraph(newLine));
                        }
                    }

                    rt.Document = doc;
                    rt.ScrollToEnd();
                }
            }
        }

        private static Paragraph BuildParagraph(string line)
        {
            if (line.StartsWith("\u001b[3"))
            {
                var color = int.Parse(line.Substring(3, 1));
                line = line.Substring(7);
                line = line.Replace("[0m", "");

                var p = new Paragraph(new Run(line));
                p.Foreground = GetColor(color);
            }

            return new Paragraph(new Run(line));
        }

        private static Brush GetColor(int index)
        {
            switch (index)
            {
                case 1:
                    return Brushes.Red;
                case 2:
                    return Brushes.LightGreen;
                case 3:
                    return Brushes.Yellow;
                case 4:
                    return Brushes.DeepSkyBlue;
                case 5:
                    return Brushes.Magenta;
                case 6:
                    return Brushes.Cyan;
                case 7:
                    return Brushes.LightCyan;
            }

            return Brushes.WhiteSmoke;
        }
    }
}