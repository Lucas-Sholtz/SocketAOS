using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;

namespace Client
{

    class MyClient
    {
        const string SERVER_IP = "127.0.0.1";
        const string CLIENT_LOG_PATH = "C:\\Users\\miste\\source\\repos\\SocketAOS\\Client\\client_log.txt";
        const int SOCKET_ID = 1040;
        private IPEndPoint endPoint;
        private Socket server;
        private string lastMessage;
        private StringBuilder builder;
        private bool working;
        public MyClient()
        {
            working = true;
            builder = new StringBuilder();
            endPoint = new IPEndPoint(IPAddress.Parse(SERVER_IP), SOCKET_ID);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Connect(endPoint);
            FileLog("Connected to server");
        }
        private void FileLog(string logMessage)
        {
            StreamWriter clientLogFile = new StreamWriter(CLIENT_LOG_PATH, true);
            string time = DateTime.Now.ToString();
            clientLogFile.WriteLine(time + " : " + logMessage);
            Console.WriteLine(logMessage);
            clientLogFile.Close();
        }
        private void Send(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            server.Send(data);
            FileLog("Client sent: \"" + message + "\"");
        }
        private void ReceiveReply()
        {
            byte[] data = new byte[256];
            int bytes = 0;
            do
            {
                bytes = server.Receive(data, data.Length, 0);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while(server.Available > 0);

            lastMessage = builder.ToString();
            builder.Clear();
            FileLog("Received from server: \"" + lastMessage + "\"");
        }
        public void Work()
        {
            while (working)
            {
                int count = 0;
                string msg;
                Console.WriteLine("Type commands");
                do
                {
                    msg = Console.ReadLine();
                    
                    switch (msg)
                    {
                        case "SORT":
                            {
                                Send(msg);
                                goto Receive;
                            }
                        case "DELTA":
                            {
                                Send(msg);
                                ReceiveReply();
                            }
                            break;
                        case "WHO":
                            {
                                Send(msg);
                                ReceiveReply();
                            }
                            break;
                        case "DISCONNECT":
                            {
                                Send(msg);
                                working = false;
                                Console.WriteLine("Client shutted down");
                                return;
                            }
                        default:
                            {
                                Send(msg);
                                if(msg!=String.Empty)
                                count++;
                            }
                            break;
                    }
                }
                while (msg != "EOS");
                Receive:
                for(int i = 0; i<count; i++)
                {
                    ReceiveReply();
                }
            }
        }
    }
}
