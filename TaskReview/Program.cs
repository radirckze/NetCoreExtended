using System;
using System.Threading;
using System.Threading.Tasks;

namespace TaskReview
{
    class Program
    {
        static int counter = 0;
        static object sem1 = new object();
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Task Review main ...");

            try 
            {
                // Task cration patterns
                 
                // Create a task but do not start it.

                Task.Run(() => PrintMsg("Task 1")).Wait(); // will wait before moving on to next line of code
                Task.Run(() => PrintMsg("Task 2")); // will start task and move on to next line of code
                Task task3 = Task.Run(() => PrintMsg("Task 3")); // will start task and move on to next line of code

                //PrintIntAsync("Called indirectly using ...");

        // // Construct a started task
        // Task t2 = Task.Factory.StartNew(action, "beta");
        // // Block the main thread to demonstrate that t2 is executing
        // t2.Wait();

        // // Launch t1 
        // t1.Start();
        // Console.WriteLine("t1 has been launched. (Main Thread={0})",
        //                   Thread.CurrentThread.ManagedThreadId);
        // // Wait for the task to finish.
        // t1.Wait();

        // // Construct a started task using Task.Run.
        // String taskData = "delta";
        // Task t3 = Task.Run( () => {Console.WriteLine("Task={0}, obj={1}, Thread={2}",
        //                                              Task.CurrentId, taskData,
        //                                               Thread.CurrentThread.ManagedThreadId);
        //                            });
        // // Wait for the task to finish.
        // t3.Wait();

        // // Construct an unstarted task
        // Task t4 = new Task(action, "gamma");
        // // Run it synchronously
        // t4.RunSynchronously();
        // // Although the task was run synchronously, it is a good practice
        // // to wait for it in the event exceptions were thrown by the task.
        // t4.Wait();
                Task.WaitAll(task3);
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Exception encounted in Task Review Main() {0}", ex.Message));
            }

            Console.WriteLine("Task Review main done! Press any key to terminate ...");
            Console.ReadLine();

        }

        public static async Task PrintIntAsync(string msg)
        {
            await Task.Run(() => PrintMsg(msg));
        }

        public static void PrintMsg(string msg)
        {
            Console.WriteLine(String.Format("PrintMsg() called with msg: {0}", msg));
            Thread.Sleep(2000);
            Console.WriteLine(String.Format("The message is: {0}", msg)); 
        }

        public static async Task<int> UpdateCounter()
        {
           lock(sem1)
           {
               counter++;
           }
           return counter;
        }
    }
}
