using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BrightScriptTools.Compiler;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace BrightScript.Language.Errors
{
    internal sealed class ErrorTagger : DisposableObject, ITagger<ErrorTag>
    {
        private ITextBuffer buffer;
        private CancellationTokenSource cancellationTokenSource;

        public ErrorTagger(ITextBuffer buffer)
        {
            this.buffer = buffer;
        }

        public IEnumerable<ITagSpan<ErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
            {
                yield break;
            }

            // It is possible that we could be asked for tags for any text buffer,
            // not necessarily the one text buffer that this tagger knows about.
            if (spans[0].Snapshot.TextBuffer != this.buffer)
            {
                yield break;
            }

            ITextSnapshot textSnapshot = spans[0].Snapshot.TextBuffer.CurrentSnapshot;
            var text = textSnapshot.GetText();

            using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            {
                // parse input args, and open input file
                Scanner scanner = new Scanner(stream);

                Parser parser = new Parser(scanner);
                if (!parser.Parse())
                {
                    
                }
            }
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        protected override void DisposeManagedResources()
        {
            if (this.buffer != null)
            {
                this.buffer.Changed -= this.OnBufferChanged;
            }

            base.DisposeManagedResources();
        }

        private void OnBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            if (this.cancellationTokenSource != null)
            {
                this.cancellationTokenSource.Cancel();
            }

            this.cancellationTokenSource = new CancellationTokenSource();
            this.UpdateErrorsWithDelay(e.After, this.cancellationTokenSource.Token);
        }

        private void UpdateErrorsWithDelay(ITextSnapshot snapshot, CancellationToken token)
        {
            Task.Run(async () =>
            {
                await Task.Delay(Constants.UIUpdateDelay).WithoutCancellation();

                if (token.IsCancellationRequested)
                {
                    return;
                }

                this.TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(snapshot, 0, snapshot.Length)));
            }, token);
        }
    }
}