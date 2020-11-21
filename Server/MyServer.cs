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
        const string SERVER_LOG_PATH = "";
        const string INFO = "Yakovenko Illia, 15, String sorter\n";
        private List<String> receivedStrings;
        private IPEndPoint endPoint;
        private Socket listenSocket;
        private Socket receiver;
        private string lastMessage;

        private void Send(string message, Socket client)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            client.Send(data);
        }
        private void FileLog(string logMessage)
        {
            StreamWriter serverLogFile = new StreamWriter(SERVER_LOG_PATH, true);
            string time = DateTime.Now.ToString();
            serverLogFile.WriteLine(time+" : "+logMessage);
        }
        public MyServer()
        {
            endPoint = new IPEndPoint(IPAddress.Parse(SERVER_IP), SOCKET_ID);
            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(endPoint);
            listenSocket.Listen(10);////?
            receiver = listenSocket.Accept();
            FileLog("Server started\n");
        }
        public void Who()
        {
            Send(INFO, receiver);
            FileLog("Server sent: \"" + INFO + "\"");
        }
        public void Receive()
        {
            StringBuilder builder = new StringBuilder();

            int bytes = 0;
            byte[] data = new byte[256];

            do
            {
                bytes = receiver.Receive(data);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }

            while (receiver.Available > 0);

            lastMessage = builder.ToString().Substring(1);
            FileLog("Received from client: \"" + lastMessage + "\"");
        }
    }
}
