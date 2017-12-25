namespace ConsoleApplication6
{
    class Program
    {
        public static AutoResetEvent e = new AutoResetEvent(false);

        static void myAsyncCallback(IAsyncResult result)
        {
            var fs = (FileStream)((object[])result.AsyncState)[0];
            var buffer = (byte[])((object[])result.AsyncState)[1];
            string text = System.Text.Encoding.UTF8.GetString(buffer);
            Console.WriteLine(text);
            fs.Close();
            e.Set();
        }


        static void Main(string[] args)
        {
            FileStream fs = new FileStream("text.txt", FileMode.Open);
            byte[] buffer = new byte[1024];
            fs.BeginRead(buffer, 0, buffer.Length, myAsyncCallback, new object[] { fs, buffer });
            e.WaitOne();
        }
    }
}
