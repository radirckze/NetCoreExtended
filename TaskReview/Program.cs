using System;
using System.Threading;
using System.Threading.Tasks;

// Reading / Resources:
// https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task?view=netframework-4.8
// https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/task-based-asynchronous-programming
// https://docs.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/task-based-asynchronous-pattern-tap



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
