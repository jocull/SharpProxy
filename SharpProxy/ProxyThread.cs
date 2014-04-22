using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Linq;
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

        private readonly long PROXY_TIMEOUT_TICKS = new TimeSpan(0, 1, 0).Ticks;
        
        private const string HTTP_SEPARATOR = "\r\n";
        private const string HTTP_HEADER_BREAK = HTTP_SEPARATOR + HTTP_SEPARATOR;
        
        private readonly string[] HTTP_SEPARATORS = new string[] { HTTP_SEPARATOR };
        private readonly string[] HTTP_HEADER_BREAKS = new string[] { HTTP_HEADER_BREAK };

        public ProxyThread(int extPort, int intPort, bool rewriteHostHeaders)
        {
            this.ExternalPort = extPort;
            this.InternalPort = intPort;
            this.RewriteHostHeaders = rewriteHostHeaders;
            this.Stopped = false;
            this.Listener = null;

            new Thread(new ThreadStart(Listen)).Start();
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
            Listener = new TcpListener(new IPEndPoint(IPAddress.Any, this.ExternalPort));
            Listener.Start();

            while (!Stopped)
            {
                try
                {
                    TcpClient client = Listener.AcceptTcpClient();
                    //Dispatch the thread and continue listening...
                    new Thread(new ThreadStart(() => Proxy(client))).Start();
                }
                catch (Exception ex)
                {
                    //TODO: Remove this. Only here to catch breakpoints.
                    bool failed = true;
                }
            }
        }

        protected void Proxy(object arg)
        {
            byte[] buffer = new byte[16384];
            int clientRead = -1;
            int hostRead = -1;

            long lastTime = DateTime.Now.Ticks;

            try
            {
                //Setup connections
                using (TcpClient client = (TcpClient)arg)
                using (TcpClient host = new TcpClient())
                {
                    host.Connect(new IPEndPoint(IPAddress.Loopback, this.InternalPort));

                    //Setup our streams
                    using (BinaryReader clientIn = new BinaryReader(client.GetStream()))
                    using (BinaryWriter clientOut = new BinaryWriter(client.GetStream()))
                    using (BinaryReader hostIn = new BinaryReader(host.GetStream()))
                    using (BinaryWriter hostOut = new BinaryWriter(host.GetStream()))
                    {
                        //Start funneling data!
                        while (clientRead != 0 || hostRead != 0 || (DateTime.Now.Ticks - lastTime) <= PROXY_TIMEOUT_TICKS)
                        {
                            while (client.Connected && (clientRead = client.Available) > 0)
                            {
                                clientRead = clientIn.Read(buffer, 0, buffer.Length);

                                //Rewrite the host header?
                                if (this.RewriteHostHeaders && clientRead > 0)
                                {
                                    string str = Encoding.UTF8.GetString(buffer, 0, clientRead);

                                    int startIdx = str.IndexOf(HTTP_SEPARATOR + "Host:");
                                    if (startIdx >= 0)
                                    {
                                        int endIdx = str.IndexOf(HTTP_SEPARATOR, startIdx + 1, str.Length - (startIdx + 1));
                                        if (endIdx > 0)
                                        {
                                            string replace = str.Substring(startIdx, endIdx - startIdx);
                                            string replaceWith = HTTP_SEPARATOR + "Host: localhost:" + InternalPort;

                                            Trace.WriteLine("Incoming HTTP header:\n\n" + str);

                                            str = str.Replace(replace, replaceWith);

                                            Trace.WriteLine("Rewritten HTTP header:\n\n" + str);
                                            
                                            byte[] strBytes = Encoding.UTF8.GetBytes(str);
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
                            if (this.Stopped)
                                return;
                            if (clientRead == 0 && hostRead == 0)
                            {
                                Thread.Sleep(100);
                            }
                        }

                        long waitTime = DateTime.Now.Ticks - lastTime;
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO: Remove this. Only here to catch breakpoints.
                bool failed = true;
            }
        }
    }
}