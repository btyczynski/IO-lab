namespace ConsolApplication15
{
    class Program
    {
        class Server
        {
            #region Variables
            TcpListener server;
            int port;
            IPAddress address;
            bool running = false;
			Task serverTask;
			
            CancellationTokenSource cts = new CancellationTokenSource();
           
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

        public void task15()
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
            Program test = new Program();
            test.task15();
        }

    }
}

