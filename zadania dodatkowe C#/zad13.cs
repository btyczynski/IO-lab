namespace ConsoleApplication13 
{
    class Program
    {
        private bool task13 = false;
		
        public bool Z2
        {
            get { return task13; }
            set { task13 = value; }
        }
        public void Task13()
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
            Program test = new P();
            Console.WriteLine("test.Z2: " + test.Z2);

            test.task13 = true;
            Console.WriteLine("test.Z2: " + test.Z2);
        }

    }
}
