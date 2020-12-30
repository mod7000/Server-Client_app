using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress ip = Dns.GetHostEntry("localhost").AddressList[0];//Get localIP
            TcpListener server = new TcpListener(ip, 8080);
            TcpClient client = default(TcpClient);

            try
            {
                server.Start();
                Console.WriteLine("Server is being Started...");


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }


            while(true)
            {
                client = server.AcceptTcpClient();

                byte[] recieveBuffer = new byte[100];
                NetworkStream stream = client.GetStream();

                stream.Read(recieveBuffer, 0, recieveBuffer.Length);


                StringBuilder msg = new StringBuilder();

                foreach(byte b in recieveBuffer)
                {
                    if (b.Equals(00))
                    {
                        break;
                    }else
                    {
                        msg.Append(Convert.ToChar(b).ToString());
                    }
                }

                Console.WriteLine(msg.ToString());


            }
        }
    }
}
