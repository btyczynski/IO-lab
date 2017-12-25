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