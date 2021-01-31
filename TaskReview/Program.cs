using System;
using System.Threading;
using System.Threading.Tasks;

using TAP;

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
            bool runTapReview = false;
            if (runTapReview)
            {
                TAP.TAP tapInstance = new TAP.TAP();
                tapInstance.Run();
            }


            Console.WriteLine("Task Review main done! Press any key to terminate ...");
            Console.ReadLine();

        }

        
    }
}
