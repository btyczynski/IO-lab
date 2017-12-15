//----Zadanie 12----

namespace Zadanie12
{
    class zadanie12
    {

        public struct TResultDataStructure
        {

            int x, y;

            public int x { get => x; set => x = value; }
            public int y { get => y; set => y = value; }

            public TResultDataStructure(int x1, int x2)
            {
                x = x1;
                y = x2;
            }
        }

        public Task<TResultDataStructure> AsyncMethod1(byte[] buffer)
        {
            TaskCompletionSource<TResultDataStructure> tcs = new TaskCompletionSource<TResultDataStructure>();
            Task.Run(() =>
            {
                tcs.SetResult(new TResultDataStructure(12, 5));
            });
            return tcs.Task;
        }

        public TResultDataStructure Zadanie12()
        {
            var task = AsyncMethod1(null);
            task.Wait();
            return task.GetAwaiter().GetResult();
        }

        static void Main(string[] args)
        {
          
            Zadanie12 test = new Zadanie12();
            Tuple <int, int> twoIntegerResult = new Tuple<int, int>(3, 3);
            object[] p1 = twoIntegerResult.GetType().GetProperties();
            object[] p2 = test.Zadanie12().GetType().GetProperties();
            Console.WriteLine(p1.Length + " " + p2.Length);
            Console.WriteLine(p1[0].GetType().ToString() + " # " + p2[0].GetType().ToString());
            Console.WriteLine(p1[1].GetType().ToString() + " # " + p2[1].GetType().ToString());

            var p = test.Zadanie1();
            Console.WriteLine(p.L1 + " " + p.L2);
        }
    }
}

//----Zadanie 13----


namespace zadanie13
{
    class Zadanie13
    {
        private bool zadanie13 = false;
        public bool Z2
        {
            get { return zadanie13; }
            set { zadanie13 = value; }
        }
        public void Zadanie13()
        {

		var t = Task.Run(
                  () =>
                  {
                     Z2 = true;
                  });

            t.Wait();
            Task.WaitAll(t);

        }
        static void Main(string[] args)
        {
            Zadanie13 test = new Zadanie13();
            Console.WriteLine("test.Z2: " + test.Z2);

            test.zadanie13 = true;
            Console.WriteLine("test.Z2: " + test.Z2);
        }

    }
}


//----Zadanie14----


namespace zadanie14
{
    class Zadanie14
    {
        public async Task<XmlDocument> Zadanie14(string address)
        {
            WebClient webClient = new WebClient(); ;
            var result = await webClient.DownloadStringTaskAsync(new Uri(address));
            XmlDocument myXML = new XmlDocument();
            myXML.LoadXml(result);

            return myXML;
        }

        static void Main(string[] args)
        {
            Zadanie14 test = new Zadanie14();

            var task = test.Zadanie14("http://www.feedforall.com/sample.xml");
            var xml = task.Result;

            using (var stringWriter = new StringWriter())
	    {
            	using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            	{
                	xml.WriteTo(xmlTextWriter);
                	xmlTextWriter.Flush();
                	Console.WriteLine(stringWriter.GetStringBuilder().ToString());
            	}
	    }
        }

    }
}


//----Zadanie15----


namespace zadanie15
{
    class Zadanie15
    {
        class Server
        {
            #region Variables
            TcpListener server;
            int port;
            IPAddress address;
            bool running = false;
            CancellationTokenSource cts = new CancellationTokenSource();
            Task serverTask;
            public Task ServerTask
            {
                get { return serverTask; }
            }
            #endregion
            #region Properties
            public IPAddress Address
            {
                get { return address; }
                set
                {
                    if (!running) address = value;
                    else;
                }
            }
            public int Port
            {
                get { return port; }
                set
                {
                    if (!running)
                        port = value;
                    else;
                }
            }
            #endregion
            #region Constructors
            public Server()
            {
                Address = IPAddress.Any;
                port = 2048;
            }
            public Server(int port)
            {
                this.port = port;
            }
            public Server(IPAddress address)
            {
                this.address = address;
            }
            #endregion
            #region Methods
            public async Task RunAsync(CancellationToken ct)
            {
                server = new TcpListener(address, port);
                try
                {
                    server.Start();
                    running = true;
                }
                catch (SocketException ex)
                {
                    throw (ex);
                }
                while (true && !ct.IsCancellationRequested)
                {
                    TcpClient client = await server.AcceptTcpClientAsync();
                    byte[] buffer = new byte[1024];
                    using (ct.Register(() => client.GetStream().Close()))
                    {
                        await client.GetStream().ReadAsync(buffer, 0, buffer.Length, ct).ContinueWith(
                        async (t) =>
                        {
                            int i = t.Result;
                            while (true)
                            {
                               await client.GetStream().WriteAsync(buffer, 0, i, ct);
                                try
                                {
                                    i = await client.GetStream().ReadAsync(buffer, 0, buffer.Length, ct);
                                }
                                catch
                                {
                                    break;
                                }
                            }
                        });
                    }
                }
            }
            public void RequestCancellation()
            {
                cts.Cancel();
                server.Stop();
            }
            public void Run()
            {
                serverTask = RunAsync(cts.Token);
            }
            public void StopRunning()
            {
                RequestCancellation();
            }
            #endregion
        }
        class Client
        {
    
            TcpClient client;
   
            public void Connect()
            {
                client = new TcpClient();
                client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2048));
            }
            public async Task<string> Ping(string message)
            {
                byte[] buffer = new ASCIIEncoding().GetBytes(message);
                await client.GetStream().WriteAsync(buffer, 0, buffer.Length);
                buffer = new byte[1024];
                var t = await client.GetStream().ReadAsync(buffer, 0, buffer.Length);
                return Encoding.UTF8.GetString(buffer, 0, t);
            }
            public async Task<IEnumerable<string>> keepPinging(string message, CancellationToken token)
            {
                List<string> messages = new List<string>();
                bool done = false;
                while (!done)
                {
                    if (token.IsCancellationRequested)
                        done = true;
                    messages.Add(await Ping(message));
                }
                return messages;
            }
        }

        public void zad15()
        {
            Server s = new Server();
            s.Run();
            Client c1 = new Client();
            Client c2 = new Client();
            Client c3 = new Client();
            c1.Connect();
            c2.Connect();
            c3.Connect();
            CancellationTokenSource ct1 = new CancellationTokenSource();
            CancellationTokenSource ct2 = new CancellationTokenSource();
            CancellationTokenSource ct3 = new CancellationTokenSource();
            var client1 = c1.keepPinging("message", ct1.Token);
            var client2 = c2.keepPinging("message", ct2.Token);
            var client3 = c3.keepPinging("message", ct3.Token);
            ct1.CancelAfter(2000);
            ct2.CancelAfter(3000);
            ct3.CancelAfter(4000);
            Task.WaitAll(new Task[] { client1, client2, client3 });
            s.StopRunning();
        }

        static void Main(string[] args)
        {
            Zadanie15 test = new Zadanie15();
            test.zad15();
        }

    }
}

