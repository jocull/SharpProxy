using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Linq;
using System.IO;

namespace SharpProxy
{
    public partial class FrmMain : Form
    {
        private const int MinPort = 1;
        private const int MaxPort = 65535;

        public static readonly string CommonDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SharpProxy");
        public static readonly string ConfigInfoPath = Path.Combine(CommonDataPath, "config.txt");

        private ProxyThread _proxyThreadListener;

        public FrmMain()
        {
            InitializeComponent();
            Text += @" " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            var ips = getLocalIPs().OrderBy(x => x);
            if (ips.Any())
            {
                cmbIPAddress.Items.Clear();
                foreach (string ip in ips)
                {
                    cmbIPAddress.Items.Add(ip);
                }
                cmbIPAddress.Text = cmbIPAddress.Items[0].ToString();
            }

            var port = 5000;
            while (!checkPortAvailability(port))
            {
                port++;
            }
            txtExternalPort.Text = port.ToString(CultureInfo.InvariantCulture);
        }

        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            txtInternalPort.Focus();

            //Try to load config
            try
            {
                using (var sr = new StreamReader(ConfigInfoPath))
                {
                    var values = sr.ReadToEnd().Split('\n')
                                               .Select(x => x.Trim())
                                               .ToArray();

                    txtInternalPort.Text = values[0];
                    chkRewriteHostHeaders.Checked = bool.Parse(values[1]);
                }
            }
            catch (Exception)
            { }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_proxyThreadListener != null)
            {
                _proxyThreadListener.Stop();
            }

            //Try to save config
            try
            {
                if (!Directory.Exists(CommonDataPath))
                {
                    Directory.CreateDirectory(CommonDataPath);
                }
                using (var sw = new StreamWriter(ConfigInfoPath))
                {
                    sw.WriteLine(txtInternalPort.Text);
                    sw.WriteLine(chkRewriteHostHeaders.Checked);
                }
            }
            catch (Exception)
            { }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            int externalPort;
            int internalPort;
            //Validation
            int.TryParse(txtExternalPort.Text, out externalPort);
            int.TryParse(txtInternalPort.Text, out internalPort);
            if (!CheckPortRange(externalPort)
                || !CheckPortRange(internalPort)
                || externalPort == internalPort)
            {
                ShowError("Ports must be between " + MinPort + "-" + MaxPort + " and must not be the same.");
                return;
            }
            if (!checkPortAvailability(externalPort))
            {
                ShowError("Port " + externalPort + " is not available, please select a different port.");
                return;
            }

            _proxyThreadListener = new ProxyThread(externalPort, internalPort, chkRewriteHostHeaders.Checked);

            ToggleButtons();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _proxyThreadListener.Stop();

            ToggleButtons();
        }

        private static void ShowError(string msg)
        {
            MessageBox.Show(msg, @"Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static bool CheckPortRange(int port)
        {
            return port >= MinPort && port <= MaxPort;
        }

        private IEnumerable<string> getLocalIPs()
        {
            //Try to find our internal IP address...
            var myHost = Dns.GetHostName();
            var addresses = Dns.GetHostEntry(myHost).AddressList;
            var myIPs = new List<string>();
            var fallbackIp = "";

            foreach (string thisAddress in addresses.Where(address => address.AddressFamily == AddressFamily.InterNetwork).Select(address => address.ToString()).Where(thisAddress => thisAddress != "127.0.0.1"))
            {
                //169.x.x.x addresses are self-assigned "private network" IP by Windows
                if (thisAddress.StartsWith("169"))
                {
                    fallbackIp = thisAddress;
                    continue;
                }
                myIPs.Add(thisAddress);
            }
            if (myIPs.Count == 0 && !string.IsNullOrEmpty(fallbackIp))
            {
                myIPs.Add(fallbackIp);
            }

            return myIPs;
        }

        private void ToggleButtons()
        {
            btnStop.Enabled = !btnStop.Enabled;
            btnStart.Enabled = !btnStart.Enabled;
            txtExternalPort.Enabled = !txtExternalPort.Enabled;
            txtInternalPort.Enabled = !txtInternalPort.Enabled;
            chkRewriteHostHeaders.Enabled = !chkRewriteHostHeaders.Enabled;
        }

        private bool checkPortAvailability(int port)
        {
            //http://stackoverflow.com/questions/570098/in-c-how-to-check-if-a-tcp-port-is-available

            // Evaluate current system tcp connections. This is the same information provided
            // by the netstat command line application, just in .Net strongly-typed object
            // form.  We will look through the list, and if our port we would like to use
            // in our TcpClient is occupied, we will set isAvailable to false.
            var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            if (tcpConnInfoArray.Any(tcpi => tcpi.LocalEndPoint.Port == port))
            {
                return false;
            }

            try
            {
                var listener = new TcpListener(new IPEndPoint(IPAddress.Any, port));
                listener.Start();
                listener.Stop();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private void txtPorts_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnStart_Click(null, null);
            }
        }
    }
}
