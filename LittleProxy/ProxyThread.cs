using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace LittleProxy
{
    public class ProxyThread
    {
        public int ExternalPort = 0;
        public int InternalPort = 0;

        public bool Stopped = false;
        protected TcpListener Listener = null;

        protected readonly long PROXY_TIMEOUT_TICKS = new TimeSpan(0, 1, 0).Ticks;

        public ProxyThread(int extPort, int intPort)
        {
            this.ExternalPort = extPort;
            this.InternalPort = intPort;

            new Thread(new ThreadStart(Listen)).Start();
        }

        public void Stop()
        {
            Stopped = true;
            Listener.Stop();
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
            TcpClient client = null;
            TcpClient host = null;

            BinaryReader clientIn = null;
            BinaryWriter clientOut = null;
            BinaryReader hostIn = null;
            BinaryWriter hostOut = null;

            byte[] buffer = new byte[16384];
            int clientRead = -1;
            int hostRead = -1;

            long lastTime = DateTime.Now.Ticks;

            try
            {
                //Setup connections
                client = (TcpClient)arg;
                host = new TcpClient();
                host.Connect(new IPEndPoint(IPAddress.Loopback, this.InternalPort));

                //Setup our streams
                clientIn = new BinaryReader(client.GetStream());
                clientOut = new BinaryWriter(client.GetStream());
                hostIn = new BinaryReader(host.GetStream());
                hostOut = new BinaryWriter(host.GetStream());

                //Start funneling data!
                while (clientRead != 0 || hostRead != 0 || (DateTime.Now.Ticks - lastTime) <= PROXY_TIMEOUT_TICKS)
                {
                    while (client.Connected && (clientRead = client.Available) > 0)
                    {
                        clientRead = clientIn.Read(buffer, 0, buffer.Length);
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
            catch (Exception ex)
            {
                //TODO: Remove this. Only here to catch breakpoints.
                bool failed = true;
            }
            finally
            {
                try { client.Close(); }
                catch { }
                try { host.Close(); }
                catch { }

                //TODO: Do I need all of these close statements?

                //try { clientOut.Close(); }
                //catch { }
                //try { clientIn.Close(); }
                //catch { }
                //try { hostOut.Close(); }
                //catch { }
                //try { hostIn.Close(); }
                //catch { }
            }
        }
    }
}