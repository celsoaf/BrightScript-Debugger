using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BrightScript.Loggger;

namespace BrightScript.Debugger.Transport
{
    public class TcpTransport : ITransport
    {
        private IPEndPoint _endPoint;
        private ITransportCallback _callback;
        private Thread _thread;
        private CancellationTokenSource _streamReadCancellationTokenSource = new CancellationTokenSource();
        private StreamReader _reader;
        private StreamWriter _writer;
        private TcpClient _client;
        private bool _bQuit;
        private Object _locker = new object();

        public void Init(IPEndPoint endPoint, ITransportCallback transportCallback)
        {
            _endPoint = endPoint;
            _callback = transportCallback;

            InitStreams();
            StartThread("MI.TcpTransport");
        }

        private void InitStreams()
        {
            _client = new TcpClient();
            _client.Connect(_endPoint);


            _reader = new StreamReader(_client.GetStream());
            _writer = new StreamWriter(_client.GetStream());
        }

        private void StartThread(string name)
        {
            _thread = new Thread(TransportLoop);
            _thread.Name = name;
            _thread.Start();
        }

        private void TransportLoop()
        {
            while (!_bQuit)
            {
                string line = GetLine();
                LiveLogger.WriteLine("->" + line);

                try
                {
                    if (!String.IsNullOrWhiteSpace(line))
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

        private string GetLine()
        {
            try
            {
                while (_client.Available == 0)
                    Thread.Sleep(1000);

                var sb = new StringBuilder();
                while (_client.Available > 0)
                {
                    byte[] buffer = new byte[_client.Available];
                    Task<int> task = _client.GetStream().ReadAsync(buffer, 0, _client.Available, _streamReadCancellationTokenSource.Token);
                    task.Wait(_streamReadCancellationTokenSource.Token);

                    sb.Append(Encoding.Default.GetString(buffer));
                }

                return Environment.NewLine + sb.ToString();
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

        public void Send(string cmd)
        {
            LiveLogger.WriteLine("<-" + cmd);
            _writer.WriteLine(cmd);
            _writer.Flush();
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

        public bool IsClosed
        {
            get { return _bQuit; }
        }
    }
}