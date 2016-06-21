using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace RokuTelnet.Telnet
{
    /// <summary>
    /// Basic Telnet client.
    /// 
    /// </summary>
    public class Client : BaseClient
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="T:PrimS.Telnet.Client"/> class.
        /// 
        /// </summary>
        /// <param name="hostname">The hostname to connect to.</param><param name="port">The port to connect to.</param><param name="token">The cancellation token.</param>
        public Client(string hostname, int port, CancellationToken token)
            : this((IByteStream)new TcpByteStream(hostname, port), token)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="T:PrimS.Telnet.Client"/> class.
        /// 
        /// </summary>
        /// <param name="byteStream">The stream served by the host connected to.</param><param name="token">The cancellation token.</param>
        public Client(IByteStream byteStream, CancellationToken token)
            : this(byteStream, token, new TimeSpan(0, 0, 30))
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="T:PrimS.Telnet.Client"/> class.
        /// 
        /// </summary>
        /// <param name="byteStream">The stream served by the host connected to.</param><param name="token">The cancellation token.</param><param name="timeout">The timeout to wait for initial successful connection to <cref>byteStream</cref>.</param>
        public Client(IByteStream byteStream, CancellationToken token, TimeSpan timeout)
            : base(byteStream, token)
        {
            Guard.AgainstNullArgument<IByteStream>("byteStream", byteStream);
            DateTime dateTime = DateTime.Now.Add(timeout);
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            while (!this.ByteStream.Connected && dateTime > DateTime.Now)
                autoResetEvent.WaitOne(2);
            if (!this.ByteStream.Connected)
                throw new InvalidOperationException("Unable to connect to the host.");
        }

        /// <summary>
        /// Tries to login asynchronously.
        /// 
        /// </summary>
        /// <param name="username">The username.</param><param name="password">The password.</param><param name="loginTimeOutMs">The login time out ms.</param>
        /// <returns>
        /// True if successful.
        /// </returns>
        public async Task<bool> TryLoginAsync(string username, string password, int loginTimeOutMs)
        {
            bool flag;
            try
            {
                if (await this.IsTerminatedWith(loginTimeOutMs, ":"))
                {
                    await this.WriteLine(username);
                    if (await this.IsTerminatedWith(loginTimeOutMs, ":"))
                        await this.WriteLine(password);
                    flag = await this.IsTerminatedWith(loginTimeOutMs, ">");
                    goto label_10;
                }
            }
            catch (Exception ex)
            {
            }
            flag = false;
        label_10:
            return flag;
        }

        /// <summary>
        /// Writes the line to the server.
        /// 
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>
        /// An awaitable Task.
        /// </returns>
        public async Task WriteLine(string command)
        {
            await this.Write(string.Format("{0}\n", (object)command));
        }

        /// <summary>
        /// Writes the specified command to the server.
        /// 
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>
        /// Any text read from the stream.
        /// </returns>
        public async Task Write(string command)
        {
            if (this.ByteStream.Connected && !this.InternalCancellation.Token.IsCancellationRequested)
            {
                await this.SendRateLimit.WaitAsync(this.InternalCancellation.Token);
                await this.ByteStream.WriteAsync(command, this.InternalCancellation.Token);
                this.SendRateLimit.Release();
            }
        }

        /// <summary>
        /// Reads asynchronously from the stream, terminating as soon as the <paramref name="terminator"/> is located.
        /// 
        /// </summary>
        /// <param name="terminator">The terminator.</param>
        /// <returns>
        /// Any text read from the stream.
        /// </returns>
        public async Task<string> TerminatedReadAsync(string terminator)
        {
            return await this.TerminatedReadAsync(terminator, TimeSpan.FromMilliseconds(100.0));
        }

        /// <summary>
        /// Reads asynchronously from the stream, terminating as soon as the <paramref name="terminator"/> is located.
        /// 
        /// </summary>
        /// <param name="terminator">The terminator.</param><param name="timeout">The timeout.</param>
        /// <returns>
        /// Any text read from the stream.
        /// </returns>
        public async Task<string> TerminatedReadAsync(string terminator, TimeSpan timeout)
        {
            return await this.TerminatedReadAsync(terminator, timeout, 1);
        }

        /// <summary>
        /// Reads asynchronously from the stream, terminating as soon as the <paramref name="terminator"/> is located.
        /// 
        /// </summary>
        /// <param name="terminator">The terminator.</param><param name="timeout">The timeout.</param>
        /// <returns>
        /// Any text read from the stream.
        /// </returns>
        public async Task<string> TerminatedReadAsync(Regex regex, TimeSpan timeout)
        {
            return await this.TerminatedReadAsync(regex, timeout, 1);
        }

        /// <summary>
        /// Reads asynchronously from the stream, terminating as soon as the <paramref name="terminator"/> is located.
        /// 
        /// </summary>
        /// <param name="terminator">The terminator.</param><param name="timeout">The maximum time to wait.</param><param name="millisecondSpin">The millisecond spin between each read from the stream.</param>
        /// <returns>
        /// Any text read from the stream.
        /// </returns>
        public async Task<string> TerminatedReadAsync(string terminator, TimeSpan timeout, int millisecondSpin)
        {
            DateTime endTimeout = DateTime.Now.Add(timeout);
            string s;
            string str1;
            string str2;
            for (s = string.Empty; !BaseClient.IsTerminatorLocated(terminator, s) && endTimeout >= DateTime.Now; s = str2 + str1)
            {
                str2 = s;
                str1 = await this.ReadAsync(TimeSpan.FromMilliseconds((double)millisecondSpin));
            }
            BaseClient.IsTerminatorLocated(terminator, s);
            return s;
        }

        /// <summary>
        /// Reads asynchronously from the stream, terminating as soon as the <paramref name="regex"/> is matched.
        /// 
        /// </summary>
        /// <param name="regex">The regex to match.</param><param name="timeout">The maximum time to wait.</param><param name="millisecondSpin">The millisecond spin between each read from the stream.</param>
        /// <returns>
        /// Any text read from the stream.
        /// </returns>
        public async Task<string> TerminatedReadAsync(Regex regex, TimeSpan timeout, int millisecondSpin)
        {
            DateTime endTimeout = DateTime.Now.Add(timeout);
            string s;
            string str1;
            string str2;
            for (s = string.Empty; !BaseClient.IsRegexLocated(regex, s) && endTimeout >= DateTime.Now; s = str2 + str1)
            {
                str2 = s;
                str1 = await this.ReadAsync(TimeSpan.FromMilliseconds(1.0));
            }
            BaseClient.IsRegexLocated(regex, s);
            return s;
        }

        /// <summary>
        /// Reads asynchronously from the stream.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// Any text read from the stream.
        /// </returns>
        public async Task<string> ReadAsync()
        {
            return await this.ReadAsync(TimeSpan.FromMilliseconds(100.0));
        }

        /// <summary>
        /// Reads asynchronously from the stream.
        /// 
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <returns>
        /// Any text read from the stream.
        /// </returns>
        public async Task<string> ReadAsync(TimeSpan timeout)
        {
            ByteStreamHandler handler = new ByteStreamHandler(this.ByteStream, this.InternalCancellation);
            return await handler.ReadAsync(timeout);
        }

        private async Task<bool> IsTerminatedWith(int loginTimeOutMs, string terminator)
        {
            return (await this.TerminatedReadAsync(terminator, TimeSpan.FromMilliseconds((double)loginTimeOutMs), 1)).TrimEnd().EndsWith(terminator);
        }
    }
}