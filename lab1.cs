//----Zadanie 1----

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            ThreadPool.QueueUserWorkItem(ThreadProc, new object[] { 900 });
            ThreadPool.QueueUserWorkItem(ThreadProc, new object[] { 700 });

            Thread.Sleep(1000);
        }
        static void ThreadProc(Object stateInfo)
        {
            int sleepTime = (int) ((object[])stateInfo)[0];

            Thread.Sleep(sleepTime);
            Console.WriteLine("Czekalem: " + sleepTime);
        }

    }
}

//---Zadanie 2----


namespace ConsoleApplication2
{
    class Program
    {



   



        static void Main(string[] args)
        {
            ThreadPool.QueueUserWorkItem(server);
            ThreadPool.QueueUserWorkItem(client);
            ThreadPool.QueueUserWorkItem(client);
            Thread.Sleep(1000);
    


            void server(Object state)
            {
                TcpListener serverr = new TcpListener(IPAddress.Any, 2048);
                serverr.Start();
                while (true)
                {
                    TcpClient clientt = serverr.AcceptTcpClient();
                    byte[] buffer = new byte[1024];
                    clientt.GetStream().Read(buffer, 0, 1024);
                    clientt.GetStream().Write(buffer, 0, buffer.Length);
                    clientt.Close();
                }


            }

            void client(Object state)
            {
                TcpClient clientt = new TcpClient();
                clientt.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2048));
                byte[] message = new ASCIIEncoding().GetBytes("wiadomosc");
                clientt.GetStream().Write(message, 0, message.Length);
            }   


         

        }


    }
}


//----Zadanie 3-4----


namespace ConsoleApplication3
{
    class Program
    {
        static Object thisLock = new Object();

        static void Main(string[] args)
        {
            ThreadPool.QueueUserWorkItem(ServerThreadProc);

            ThreadPool.QueueUserWorkItem(ClientThreadProc, "1st");
            ThreadPool.QueueUserWorkItem(ClientThreadProc, "2nd");

            while(true) { }
        }

        static void ServerThreadProc(Object stateInfo)
        {
            TcpListener server = new TcpListener(IPAddress.Any, 2048);
            server.Start();

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                ThreadPool.QueueUserWorkItem(NewClientThreadProc, client);
            }
        }

        static void NewClientThreadProc(Object stateInfo)
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

        static void ClientThreadProc(Object stateInfo)
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


//----Zadanie 5----

namespace ConsoleApplication4
{
    class Program
    {
        static Object thisLock = new Object();

        static int[] tab;
        static int sum;

        static int size;
        static int fragmentSize;

        static void Main(string[] args)
        {
            size = args.Length > 0 ? int.Parse(args[0]) : 640;
            fragmentSize = args.Length > 1 ? int.Parse(args[1]) : 10;

            tab = new int[size];
            sum = 0;
			          
            int workersCount = (int) Math.Ceiling((double) size / fragmentSize);
            AutoResetEvent[] events = new AutoResetEvent[workersCount];

            for (int i = 0; i < workersCount; ++i)
            {
                events[i] = new AutoResetEvent(false);

                ThreadPool.QueueUserWorkItem(new WaitCallback(Sum), new object[] { events[i], i });
            }


            WaitHandle.WaitAll(events);
        }

        static void Sum(object state)
        {
            AutoResetEvent e = (AutoResetEvent)((object[])state)[0];
            int from = (int)((object[])state)[1] * fragmentSize;
            int result = 0;

            for (int i = from; i < from + fragmentSize && i < size; ++i)
            {
                result += tab[i];
            }

            lock (thisLock)
            {
                sum += result;
            }

            e.Set();
        }
    }
}