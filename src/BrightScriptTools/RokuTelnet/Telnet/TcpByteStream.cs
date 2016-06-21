﻿using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RokuTelnet.Telnet
{
    /// <summary>
    /// A ByteStream acting over a TCP channel.
    /// 
    /// </summary>
    public class TcpByteStream : IByteStream, IDisposable
    {
        private readonly ISocket socket;

        /// <summary>
        /// Gets the amount of data that has been received from the network and is available to be read.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The number of bytes of data received from the network and available to be read.
        /// 
        /// </value>
        public int Available
        {
            get
            {
                return this.socket.Available;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:PrimS.Telnet.IByteStream"/> is connected.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// <c>True</c> if connected; otherwise, <c>false</c>.
        /// 
        /// </value>
        public bool Connected
        {
            get
            {
                return this.socket.Connected;
            }
        }

        /// <summary>
        /// Gets or sets the amount of time this <see cref="T:PrimS.Telnet.IByteStream"/> will wait to receive data once a read operation is initiated.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The time-out value of the connection in milliseconds. The default value is 0.
        /// 
        /// </value>
        public int ReceiveTimeout
        {
            get
            {
                return this.socket.ReceiveTimeout;
            }
            set
            {
                this.socket.ReceiveTimeout = value;
            }
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="T:PrimS.Telnet.TcpByteStream"/> class.
        /// 
        /// </summary>
        /// <param name="hostname">The hostname.</param><param name="port">The port.</param>
        public TcpByteStream(string hostname, int port)
            : this((ISocket)new TcpClient(hostname, port))
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="T:PrimS.Telnet.TcpByteStream"/> class.
        /// 
        /// </summary>
        /// <param name="tcpSocket">The TCP socket.</param>
        internal TcpByteStream(ISocket tcpSocket)
        {
            this.socket = tcpSocket;
        }

        /// <summary>
        /// Reads a byte from the stream and advances the position within the stream by one byte, or returns -1 if at the end of the stream.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// The unsigned byte cast to an integer, or -1 if at the end of the stream.
        /// 
        /// </returns>
        public int ReadByte()
        {
            return this.socket.GetStream().ReadByte();
        }

        /// <summary>
        /// Writes a byte to the current position in the stream and advances the position within the stream by one byte.
        /// 
        /// </summary>
        /// <param name="value">The byte to write to the stream.</param>
        public void WriteByte(byte value)
        {
            this.socket.GetStream().WriteByte(value);
        }

        /// <summary>
        /// Asynchronously writes a sequence of bytes to the current stream, advances the current position within this stream by the number of bytes written, and monitors cancellation requests.
        /// 
        /// </summary>
        /// <param name="buffer">The buffer to write data from.</param><param name="offset">The zero-based byte offset in buffer from which to begin copying bytes to the stream.</param><param name="count">The maximum number of bytes to write.</param><param name="cancellationToken">The token to monitor for cancellation requests. The default value is System.Threading.CancellationToken.None.</param>
        /// <returns>
        /// A task that represents the asynchronous write operation.
        /// 
        /// </returns>
        public Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return this.socket.GetStream().WriteAsync(buffer, offset, count, cancellationToken);
        }

        /// <summary>
        /// Asynchronously writes the specified command to the stream.
        /// 
        /// </summary>
        /// <param name="command">The command.</param><param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task representing the asynchronous action.
        /// </returns>
        public Task WriteAsync(string command, CancellationToken cancellationToken)
        {
            byte[] buffer = TcpByteStream.ConvertStringToByteArray(command);
            return this.socket.GetStream().WriteAsync(buffer, 0, buffer.Length, cancellationToken);
        }

        /// <summary>
        /// Disposes the instance and requests that the underlying connection be closed.
        /// 
        /// </summary>
        public void Close()
        {
            this.socket.Close();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// 
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize((object)this);
        }

        private static byte[] ConvertStringToByteArray(string command)
        {
            return Encoding.ASCII.GetBytes(command.Replace("\0xFF", "\0xFF\0xFF"));
        }

        private void Dispose(bool isDisposing)
        {
            if (!isDisposing)
                return;
            this.Close();
        }
    }
}