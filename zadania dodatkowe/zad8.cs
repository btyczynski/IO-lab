namespace ConsoleApplication8
{
    class Program
    {
        delegate int DelegateType(int arguments);
        static DelegateType silniaN;
        static DelegateType fibonaci;

        static int silniar(int n)
        {
            if (n == 0) return 1;
            return n * silnia(n - 1);
        }

        static int silniai(int n)
		{
    	    int result = 1;

			for (int i = 1; i <= n; i++)
			{
        		result *= i;
    		}

    	    return result;
		}

        static int fibr(int n)
        {
            if (n < 3)
                return 1;
            return fib(n - 2) + fib(n - 1);
        }
	
		static int fibi(int n)
		{
    		if (n <= 1)
    		{
        		return n;
    		}
			
			int fib = 1;
			int prevFib = 1;

    		for (int i = 2; i < n; i++)
    		{
        		int temp = fib;
        		fib += prevFib;
        		prevFib = temp;
    		}

    		return fib;	
		}

        static void Main(string[] args)
        {


            silniaN = new DelegateType(silniar);
			silnia = new DelegateType(silniai);	
            fibonaciN = new DelegateType(fibr);
			fibonaci = new DelegateType(fibi);	

            IAsyncResult ar = silniaN.BeginInvoke(5, null, null);
            int result = silniaN.EndInvoke(ar);

            IAsyncResult ar2 = fibonaciN.BeginInvoke(6, null, null);
            int result2 = fibonaciN.EndInvoke(ar2);

			IAsyncResult ar3 = silnia.BeginInvoke(5, null, null);
            int result3 = silnia.EndInvoke(ar3);

            IAsyncResult ar4 = fibonaci.BeginInvoke(6, null, null);
            int result4 = fibonaci.EndInvoke(ar4);

            Console.WriteLine("5!={0}\nF(6)={1}", result, result2);
			Console.WriteLine("5!={0}\nF(6)={1}", result3, result4);
        }
    }
}

