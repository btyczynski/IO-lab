namespace ConsoleApplication3
{
    class Program
    {
        static Object thisLock = new Object();

        static void Main(string[] args)
        {
            ThreadPool.QueueUserWorkItem(ServerThread);

            ThreadPool.QueueUserWorkItem(ClientThread, "1st");
            ThreadPool.QueueUserWorkItem(ClientThread, "2nd");

            while(true) 
			{ 
		
			}
        }

        static void ServerThread(Object stateInfo)
        {
            TcpListener server = new TcpListener(IPAddress.Any, 2048);
            server.Start();

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                ThreadPool.QueueUserWorkItem(NewClientThread, client);
            }
        }

        static void NewClientThread(Object stateInfo)
        {

            TcpClient client = (TcpClient) stateInfo;
            byte[] buffer = new byte[1024];

            while(true)
            {
                int bytes = client.GetStream().Read(buffer, 0, 1024);

                if (bytes == 0)
                    break;

                string msg = System.Text.Encoding.ASCII.GetString(buffer, 0, bytes);
                writeConsoleMessage("Otrzymano wiadomosc: " + msg, ConsoleColor.Red);

                client.GetStream().Write(buffer, 0, bytes);
            }

            client.Close();
        }

        static void ClientThread(Object stateInfo)
        {
            TcpClient client = new TcpClient("localhost", 2048);
            Console.WriteLine("Nawiazano polczenie");

            byte[] buffer = new byte[1024];
            buffer = System.Text.Encoding.ASCII.GetBytes( (string) stateInfo );

            client.GetStream().Write(buffer, 0, buffer.Length);


            while (true)
            {
                buffer = new byte[1024];
                int bytes = client.GetStream().Read(buffer, 0, 1024);

                if (bytes == 0)
                    break;

                string msg = System.Text.Encoding.ASCII.GetString(buffer, 0, bytes);
                writeConsoleMessage("Otrzymano wiadomosc: " + msg, ConsoleColor.Green);
                
                client.GetStream().Write(buffer, 0, bytes);
            }
        }

        static void writeConsoleMessage(string message, ConsoleColor color)
        {
            lock (thisLock)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }

    }
}
