using System;
using System.Threading;
using System.Threading.Tasks;





namespace TaskReview
{
    class Program
    {
        
        static void Main(string[] args)
        {
    
            Console.WriteLine("Starting Task Review main ...");

            bool runTaskBasics = true;
            if (runTaskBasics)
            {
                (new TPLReview()).Run();
            }

            // Lets TAP ....
            Console.WriteLine("Runninng TAP ...");
           

            Console.WriteLine("TAP review completed.");

            Console.WriteLine("Task Review main done! Press any key to terminate ...");
            Console.ReadLine();

        }

        
    }
}
