using System;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace SharpProxy
{
    public class ProxyThread
    {
        public int ExternalPort { get; set; }
        public int InternalPort { get; set; }
        public bool RewriteHostHeaders { get; set; }
        public bool Stopped { get; set; }

        private TcpListener Listener { get; set; }

        private readonly long _proxyTimeoutTicks = new TimeSpan(0, 1, 0).Ticks;
        
        private const string HttpSeparator = "\r\n";
        private const string HttpHeaderBreak = HttpSeparator + HttpSeparator;
        
        private readonly string[] _httpSeparators = { HttpSeparator };
        private readonly string[] _httpHeaderBreaks = { HttpHeaderBreak };

        public ProxyThread(int extPort, int intPort, bool rewriteHostHeaders)
        {
            ExternalPort = extPort;
            InternalPort = intPort;
            RewriteHostHeaders = rewriteHostHeaders;
            Stopped = false;
            Listener = null;

            new Thread(Listen).Start();
        }

        public void Stop()
        {
            Stopped = true;
            if (Listener != null)
            {
                Listener.Stop();
            }
        }

        protected void Listen()
        {
            Listener = new TcpListener(new IPEndPoint(IPAddress.Any, ExternalPort));
            Listener.Start();

            while (!Stopped)
            {
                try
                {
                    var client = Listener.AcceptTcpClient();
                    //Dispatch the thread and continue listening...
                    new Thread(() => Proxy(client)).Start();
                }
                catch (Exception)
                {
                }
            }
        }

        protected void Proxy(object arg)
        {
            var buffer = new byte[16384];
            var clientRead = -1;
            var hostRead = -1;

            var lastTime = DateTime.Now.Ticks;

            try
            {
                //Setup connections
                using (var client = (TcpClient)arg)
                using (var host = new TcpClient())
                {
                    host.Connect(new IPEndPoint(IPAddress.Loopback, InternalPort));

                    //Setup our streams
                    using (var clientIn = new BinaryReader(client.GetStream()))
                    using (var clientOut = new BinaryWriter(client.GetStream()))
                    using (var hostIn = new BinaryReader(host.GetStream()))
                    using (var hostOut = new BinaryWriter(host.GetStream()))
                    {
                        //Start funneling data!
                        while (clientRead != 0 || hostRead != 0 || (DateTime.Now.Ticks - lastTime) <= _proxyTimeoutTicks)
                        {
                            while (client.Connected && (clientRead = client.Available) > 0)
                            {
                                clientRead = clientIn.Read(buffer, 0, buffer.Length);

                                //Rewrite the host header?
                                if (RewriteHostHeaders && clientRead > 0)
                                {
                                    var str = Encoding.UTF8.GetString(buffer, 0, clientRead);

                                    var startIdx = str.IndexOf(HttpSeparator + "Host:", StringComparison.Ordinal);
                                    if (startIdx >= 0)
                                    {
                                        var endIdx = str.IndexOf(HttpSeparator, startIdx + 1, str.Length - (startIdx + 1), StringComparison.Ordinal);
                                        if (endIdx > 0)
                                        {
                                            var replace = str.Substring(startIdx, endIdx - startIdx);
                                            var replaceWith = HttpSeparator + "Host: localhost:" + InternalPort;

                                            Trace.WriteLine("Incoming HTTP header:\n\n" + str);

                                            str = str.Replace(replace, replaceWith);

                                            Trace.WriteLine("Rewritten HTTP header:\n\n" + str);
                                            
                                            var strBytes = Encoding.UTF8.GetBytes(str);
                                            Array.Clear(buffer, 0, buffer.Length);
                                            Array.Copy(strBytes, buffer, strBytes.Length);
                                            clientRead = strBytes.Length;
                                        }
                                    }
                                }

                                hostOut.Write(buffer, 0, clientRead);
                                lastTime = DateTime.Now.Ticks;
                                hostOut.Flush();
                            }
                            while (host.Connected && (hostRead = host.Available) > 0)
                            {
                                hostRead = hostIn.Read(buffer, 0, buffer.Length);
                                clientOut.Write(buffer, 0, hostRead);
                                lastTime = DateTime.Now.Ticks;
                                clientOut.Flush();
                            }

                            //Sleepy time?
                            if (Stopped)
                                return;
                            if (clientRead == 0 && hostRead == 0)
                            {
                                Thread.Sleep(100);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}