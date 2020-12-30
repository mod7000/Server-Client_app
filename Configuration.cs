using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace Server_Client_v3
{
    public partial class Configuration : Form
    {
        Socket sck;
        EndPoint epLocal, epRemote;
        byte[] buffer;
        public Configuration()
        {
            InitializeComponent();
        }
        private void Configuration_Load(object sender, EventArgs e)
        {
            //setup Socket
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            //get local IP
            txtlocalIP.Text = GetlocalIP();
            txtremoteIP.Text = GetlocalIP();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (CheckConfig() == true)
                {
                    this.BackColor = Color.FromArgb(18, 121, 62);

                    pnlConfig.Visible = false;
                    pnlChat.Location = new Point(6, 27);
                    //bind Socket
                    epLocal = new IPEndPoint(IPAddress.Parse(txtlocalIP.Text), Convert.ToInt32(txtlocalPort.Text));
                    sck.Bind(epLocal);
                    //connect to Remote IP
                    epRemote = new IPEndPoint(IPAddress.Parse(txtremoteIP.Text), Convert.ToInt32(txtremotePort.Text));
                    sck.Connect(epRemote);

                    //Listen Specific POrt
                    buffer = new byte[1500];
                    sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);

                    btnConnect.Enabled = false;
                    listMessage.Items.Add("Server: Connected!");
                    lblName.Text = txtName.Text;
                }
                else
                {
                    MessageBox.Show("Configuration is Wrong!", "Server-Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Invalid Configuration", "Server-Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool CheckConfig()
        {
            if (txtName.Text != "" && txtFrndName.Text != "" && txtlocalIP.Text != "" && txtlocalPort.Text != "" && txtremoteIP.Text != "" && txtremotePort.Text != "") 
            {
                return true;
            }else
                return false;
        }

        private string GetlocalIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            return "127.0.0.1";
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            //convert message string to byte[]
            ASCIIEncoding aEncoding = new ASCIIEncoding();
            byte[] sendingMessage = new byte[1500];
            sendingMessage = aEncoding.GetBytes(txtMessage.Text);

            //sending the encoded message
            sck.Send(sendingMessage);

            //add to the list box
            listMessage.Items.Add(lblName.Text+": " + txtMessage.Text);
            txtMessage.Clear();
        }

        private void txtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                btnSend.PerformClick();
            }
        }

        private void MessageCallBack(IAsyncResult aResult)
        {
            try
            {
                byte[] recieveData = new byte[1500];
                recieveData = (byte[])aResult.AsyncState;

                //convert byte[] to string
                ASCIIEncoding aEncoding = new ASCIIEncoding();
                string recieveMEssage = aEncoding.GetString(recieveData);

                //Adding this message into textbox
                listMessage.Items.Add(txtFrndName.Text + ": " + recieveMEssage);

                buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

    }
}
