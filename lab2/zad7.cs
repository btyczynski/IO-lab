namespace ConsoleApplication7
{
    class Program
    {

        static void Main(string[] args)
        {
            FileStream fs = new FileStream("tekst.txt", FileMode.Open);
            byte[] buffer = new byte[1024];
			
            var ar = fs.BeginRead(buffer, 0, buffer.Length, null, null);
			
            fs.EndRead(ar);
			
            string text= System.Text.Encoding.UTF8.GetString(buffer);
            Console.WriteLine(tekst);
			
            fs.Close();
        }
    }
}