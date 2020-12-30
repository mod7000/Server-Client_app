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

namespace Server_Client_app
{
    public partial class Form1 : Form
    {
        Socket sck;
        EndPoint epLocal, epRemote;
        byte[] buffer;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
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
                listMessage.Items.Add("Friend: " + recieveMEssage);

                buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
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
            listMessage.Items.Add("Me: "+ txtMessage.Text);

            txtMessage.Clear();

        }

        private string GetlocalIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach(IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork) 
                {
                    return ip.ToString();
                }
            }

            return "127.0.0.1";
        }



    }
}
