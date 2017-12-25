namespace ConsoleApplication2
{
    class Program
    {

	
		static void server(Object state)
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
		
		static void client(Object state)
        {
                TcpClient clientt = new TcpClient();
				
                clientt.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2048));
				
                byte[] message = new ASCIIEncoding().GetBytes("wiadomosc");
                clientt.GetStream().Write(message, 0, message.Length);
				
				clientt.GetStream().Read(message, 0, message.Length);
                String data = System.Text.Encoding.ASCII.GetString(message);
				
        }   

	
	
        static void Main(string[] args)
        {
            ThreadPool.QueueUserWorkItem(server);
            ThreadPool.QueueUserWorkItem(client);
            ThreadPool.QueueUserWorkItem(client);
            Thread.Sleep(1000);
			
        }

    }
}

