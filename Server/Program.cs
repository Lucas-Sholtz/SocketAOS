﻿using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            MyServer server = new MyServer();

            server.Work();
        }
    }
}
