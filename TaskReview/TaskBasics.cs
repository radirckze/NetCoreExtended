using System;
using System.Threading;
using System.Threading.Tasks;

// Reading / Resources:
// https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task?view=netframework-4.8
// https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/task-based-asynchronous-programming



namespace TaskReview
{

    public class TaskBasics
    {

        #region Actions

        Action<string> PrintMsgAction = (msg) =>
        {
            Console.WriteLine(String.Format("PrintMsgAction() called with msg: {0}", msg));
            Thread.Sleep(2000);
            Console.WriteLine(String.Format("The message is: {0}", msg));
        };

        #endregion Actions

        int counter = 0;
        object sem1 = new object();

        public TaskBasics()
        {

        }

        public void Run()
        {

            Console.WriteLine("Starting TaskBasics ...");

            bool runInitiationPatterns = false;
            if (runInitiationPatterns) {


               Task t1 = new Task(() => PrintMsgAction("Action Task 1"));
               t1.Start();
               Task.WaitAll(t1);
            }

            bool runTaskCancellation = true;
            if (runTaskCancellation) {
                CancellationTokenSource cts = new CancellationTokenSource();
               Task cancelTask = new Task(() => PrintMsgAction("Action Task 1"), cts.Token);
               cancelTask.Start();
               cts.Cancel();
               Task.WaitAll(new Task[] {cancelTask}, cts.Token);
               Console.WriteLine(String.Format("Task Cancellation - task status = {0}, IsCancelled = {1}", cancelTask.Status, cancelTask.IsCanceled));
            }

            bool runTestWait = false;
            if (runTestWait) 
            {
                // Task cration patterns
                 
                // Create a task but do not start it.

                Task.Run(() => PrintMsg("Task 1")).Wait(); // will wait before moving on to next line of code
                Task.Run(() => PrintMsg("Task 2")); // will start task and move on to next line of code
                Task task3 = Task.Run(() => PrintMsg("Task 3")); // will start task and move on to next line of code

               
                Task.WaitAll(task3);
            }

            Console.WriteLine("Completed TaskBasics.");
        }

        #region methods

        public async Task PrintIntAsync(string msg)
        {
            await Task.Run(() => PrintMsg(msg));
        }

        public void PrintMsg(string msg)
        {
            Console.WriteLine(String.Format("PrintMsg() called with msg: {0}", msg));
            Thread.Sleep(2000);
            Console.WriteLine(String.Format("The message is: {0}", msg)); 
        }

        public async Task<int> UpdateCounter()
        {
           lock(sem1)
           {
               counter++;
           }
           return counter;
        }

        #endregion methods


    }
}