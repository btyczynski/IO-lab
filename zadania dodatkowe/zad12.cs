namespace ConsoleApplication12
{
    class Program
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
          
            Program test = new Program();
            Tuple <int, int> twoIntegerResult = new Tuple<int, int>(3, 3);
			
            object[] p1 = twoIntegerResult.GetType().GetProperties();
            object[] p2 = test.Zadanie12().GetType().GetProperties();
			
            Console.WriteLine(p1.Length + " " + p2.Length);
            Console.WriteLine(p1[0].GetType().ToString() + " # " + p2[0].GetType().ToString());
            Console.WriteLine(p1[1].GetType().ToString() + " # " + p2[1].GetType().ToString());

            var p = test.Zadanie12();
            Console.WriteLine(p.L1 + " " + p.L2);
        }
    }
}