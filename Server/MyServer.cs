using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Server
{
    class MyServer
    {
        const int SOCKET_ID = 1040;
        const string SERVER_IP = "127.0.0.1";
        const string SERVER_LOG_PATH = "C:\\Users\\miste\\source\\repos\\SocketAOS\\Server\\server_log.txt";
        const string INFO = "Yakovenko Illia, 15, String sorter";
        private List<String> receivedStrings;
        private IPEndPoint endPoint;
        private Socket listenSocket;
        private Socket receiver;
        private string delta;
        private string lastMessage;
        private StringBuilder builder;
        private bool working;

        private void Send(string message, Socket client)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            client.Send(data);
            FileLog("Server sent: \"" + message + "\"");
        }
        private void FileLog(string logMessage)
        {   
            StreamWriter serverLogFile = new StreamWriter(SERVER_LOG_PATH, true);
            string time = DateTime.Now.ToString();
            serverLogFile.WriteLine(time+" : "+logMessage);
            Console.WriteLine(logMessage);
            serverLogFile.Close();
        }
        public MyServer()
        {
            delta = "The operation \"SORT\" has not yet been performed";
            builder = new StringBuilder();
            working = true;
            receivedStrings = new List<string>();
            endPoint = new IPEndPoint(IPAddress.Parse(SERVER_IP), SOCKET_ID);
            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(endPoint);
            listenSocket.Listen(10);
            receiver = listenSocket.Accept();
            FileLog("Client connected");
        }
        private void Who()
        {
            Send(INFO, receiver);
        }
        private void Receive()
        {


            int bytes = 0;
            byte[] data = new byte[256];

            do
            {
                bytes = receiver.Receive(data);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }

            while (receiver.Available > 0);

            lastMessage = builder.ToString().Substring(0);
            builder.Clear();
            FileLog("Received from client: \"" + lastMessage + "\"");
        }
        private void SortStrings()
        {
            var startTime = System.Diagnostics.Stopwatch.StartNew();
            receivedStrings.Sort();
            startTime.Stop();
            var resultTime = startTime.Elapsed;
            delta = resultTime.ToString();
        }
        private void Process()
        {
            if (lastMessage == "WHO")
            {
                Who();
            }
            else if (lastMessage == "SORT")
            {
                SortStrings();
                for (int i = 0; i < receivedStrings.Count; i++)
                {
                    System.Threading.Thread.Sleep(5);
                    Send(receivedStrings[i], receiver);
                }
                receivedStrings.Clear();
            }
            else if (lastMessage == "DISCONNECT")
            {
                receiver.Shutdown(SocketShutdown.Both);
                receiver.Close();
                Console.WriteLine("Server shutted down");
                working = false;
            }
            else if (lastMessage == "DELTA")
            {
                Send(delta, receiver);
            }
            else
            {
                receivedStrings.Add(lastMessage);
            }
        }
        public void Work()
        {
            while (working)
            {
                Receive();
                Process();
            }
            return;
        }
    }
}
