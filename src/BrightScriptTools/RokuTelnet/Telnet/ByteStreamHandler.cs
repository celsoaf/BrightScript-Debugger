using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RokuTelnet.Telnet
{
    /// <summary>
    /// Provides core functionality for interacting with the ByteStream.
    /// 
    /// </summary>
    /// 
    /// <summary>
    /// Provides core functionality for interacting with the ByteStream.
    /// 
    /// </summary>
    public class ByteStreamHandler : IByteStreamHandler
    {
        private readonly IByteStream byteStream;
        private readonly CancellationTokenSource internalCancellation;

        private bool IsResponsePending
        {
            get
            {
                return this.byteStream.Available > 0;
            }
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="T:PrimS.Telnet.ByteStreamHandler"/> class.
        /// 
        /// </summary>
        /// <param name="byteStream">The byteStream to handle.</param><param name="internalCancellation">A cancellation token.</param>
        public ByteStreamHandler(IByteStream byteStream, CancellationTokenSource internalCancellation)
        {
            this.byteStream = byteStream;
            this.internalCancellation = internalCancellation;
        }

        private static DateTime ExtendRollingTimeout(TimeSpan timeout)
        {
            return DateTime.Now.Add(TimeSpan.FromMilliseconds(timeout.TotalMilliseconds / 100.0));
        }

        private static async Task<bool> IsWaitForIncrementalResponse(DateTime rollingTimeout)
        {
            bool result = DateTime.Now < rollingTimeout;
            await Task.Delay(1);
            return result;
        }

        private static bool IsWaitForInitialResponse(DateTime endInitialTimeout, StringBuilder sb)
        {
            if (sb.Length == 0)
                return DateTime.Now < endInitialTimeout;
            return false;
        }

        private bool RetrieveAndParseResponse(StringBuilder sb)
        {
            if (!this.IsResponsePending)
                return false;
            int num = this.byteStream.ReadByte();
            switch (num)
            {
                case -1:
                label_6:
                    return true;
                case (int)byte.MaxValue:
                    int inputVerb = this.byteStream.ReadByte();
                    switch (inputVerb)
                    {
                        case 251:
                        case 252:
                        case 253:
                        case 254:
                            this.ReplyToCommand(inputVerb);
                            goto label_6;
                        case (int)byte.MaxValue:
                            sb.Append(inputVerb);
                            goto label_6;
                        default:
                            goto label_6;
                    }
                default:
                    sb.Append((char)num);
                    goto case -1;
            }
        }

        private void ReplyToCommand(int inputVerb)
        {
            int num = this.byteStream.ReadByte();
            if (num == -1)
                return;
            this.byteStream.WriteByte(byte.MaxValue);
            if (num == 3)
                this.byteStream.WriteByte(inputVerb == 253 ? (byte)251 : (byte)253);
            else
                this.byteStream.WriteByte(inputVerb == 253 ? (byte)252 : (byte)254);
            this.byteStream.WriteByte((byte)num);
        }

        /// <summary>
        /// Reads asynchronously from the stream.
        /// 
        /// </summary>
        /// <param name="timeout">The rolling timeout to wait for no further response from stream.</param>
        /// <returns>
        /// Any text read from the stream.
        /// </returns>
        public async Task<string> ReadAsync(TimeSpan timeout)
        {
            string str;
            if (!this.byteStream.Connected || this.internalCancellation.Token.IsCancellationRequested)
            {
                str = string.Empty;
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                this.byteStream.ReceiveTimeout = (int)timeout.TotalMilliseconds;
                DateTime endInitialTimeout = DateTime.Now.Add(timeout);
                DateTime rollingTimeout = ByteStreamHandler.ExtendRollingTimeout(timeout);
                do
                {
                    do
                    {
                        if (this.RetrieveAndParseResponse(sb))
                            rollingTimeout = ByteStreamHandler.ExtendRollingTimeout(timeout);
                        if (this.internalCancellation.Token.IsCancellationRequested)
                            goto label_8;
                    }
                    while (this.IsResponsePending || ByteStreamHandler.IsWaitForInitialResponse(endInitialTimeout, sb));
                }
                while (await ByteStreamHandler.IsWaitForIncrementalResponse(rollingTimeout));
            label_8:
                int num = DateTime.Now >= rollingTimeout ? 1 : 0;
                str = sb.ToString();
            }
            return str;
        }
    }
}