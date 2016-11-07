using System;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace BrightScript.Debugger.Core.Transports
{
    public class TcpTransport : ITransport
    {
        private ITransportCallback _callback;
        private Thread _thread;
        private bool _bQuit;
        private CancellationTokenSource _streamReadCancellationTokenSource = new CancellationTokenSource();
        protected StreamReader _reader;
        protected StreamWriter _writer;
        private bool _filterStdout;
        private Object _locker = new object();
        private TcpClient _client;

        protected Logger Logger
        {
            get; private set;
        }

        public TcpTransport()
        {
        }

        protected string GetThreadName()
        {
            return "MI.TcpTransport";
        }

        public virtual void Init(ITransportCallback transportCallback, LaunchOptions options, Logger logger)
        {
            Logger = logger;
            _callback = transportCallback;
            InitStreams(options, out _reader, out _writer);
            StartThread(GetThreadName());
        }

        private void StartThread(string name)
        {
            _thread = new Thread(TransportLoop);
            _thread.Name = name;
            _thread.Start();
        }

        public void InitStreams(LaunchOptions options, out StreamReader reader, out StreamWriter writer)
        {
            TcpLaunchOptions tcpOptions = (TcpLaunchOptions)options;

            _client = new TcpClient();
            _client.Connect(tcpOptions.Hostname, tcpOptions.Port);

            reader = new StreamReader(_client.GetStream());
            writer = new StreamWriter(_client.GetStream());
        }



        protected virtual string FilterLine(string line)
        {
            return line;
        }

        private void TransportLoop()
        {
            try
            {
                while (!_bQuit)
                {
                    string line = GetLine();
                    if (line == null)
                        break;

                    line = line.TrimEnd();
                    Logger?.WriteLine("->" + line);

                    try
                    {
                        if (_filterStdout)
                        {
                            line = FilterLine(line);
                        }
                        if (!String.IsNullOrWhiteSpace(line) && !line.StartsWith("-", StringComparison.Ordinal))
                        {
                            _callback.OnStdOutLine(line);
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                        Debug.Assert(_bQuit);
                        break;
                    }
                }
                if (!_bQuit)
                {
                    OnReadStreamAborted();
                }
            }
            finally
            {
                lock (_locker)
                {
                    _bQuit = true;
                    _streamReadCancellationTokenSource.Dispose();
                    _reader.Dispose();

                    try
                    {
                        _writer.Dispose();
                    }
                    catch
                    {
                        // This can fail flush side effects if the debugger goes down. When this happens we don't want
                        // to crash OpenDebugAD7/VS. Stack:
                        //   System.IO.UnixFileStream.WriteNative(Byte[] array, Int32 offset, Int32 count)
                        //   System.IO.UnixFileStream.FlushWriteBuffer()
                        //   System.IO.UnixFileStream.Dispose(Boolean disposing)
                        //   System.IO.FileStream.Dispose(Boolean disposing)
                        //   System.IO.Stream.Close()
                        //   System.IO.StreamWriter.Dispose(Boolean disposing)
                        //   System.IO.TextWriter.Dispose()
                    }
                }
            }
        }

        protected virtual void OnReadStreamAborted()
        {
            try
            {
                _callback.OnDebuggerProcessExit(null);
            }
            catch
            {
                // eat exceptions on this thread so we don't bring down VS
            }
        }
        protected void Echo(string cmd)
        {
            Logger?.WriteLine("<-" + cmd);
            _writer.WriteLine(cmd);
            _writer.Flush();
        }

        private string GetLine()
        {
            try
            {
                while(_client.Available == 0)
                    Thread.Sleep(1000);

                if (_client.Available > 0)
                {
                    byte[] buffer = new byte[_client.Available];
                    Task<int> task = _client.GetStream().ReadAsync(buffer, 0, _client.Available, _streamReadCancellationTokenSource.Token);
                    task.Wait(_streamReadCancellationTokenSource.Token);

                    return System.Text.Encoding.Default.GetString(buffer);
                }

                return null;
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (ObjectDisposedException)
            {
                return null;
            }
            // I have seen the StreamReader throw both an ObjectDisposedException (which makes sense) and a NullReferenceException
            // (which seems like a bug) after it is closed. Since we have no exception back stop here, we are catching all exceptions
            // here (we don't want to crash VS).
            catch
            {
                Debug.Assert(_bQuit, "Exception throw from ReadLine when we haven't quit yet");
                return null;
            }
        }

        public void Send(string cmd)
        {
            Echo(cmd);
        }

        public bool IsClosed
        {
            get { return _bQuit; }
        }

        protected ITransportCallback Callback
        {
            get { return _callback; }
        }

        public void Close()
        {
            lock (_locker)
            {
                if (!_bQuit)
                {
                    _bQuit = true;
                    _streamReadCancellationTokenSource.Cancel();
                }
            }
            ((IDisposable)_client).Dispose();
        }
    }
}