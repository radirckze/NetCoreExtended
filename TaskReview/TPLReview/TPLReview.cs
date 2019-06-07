using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


// TPL (Task Parallel Library) Review

namespace TaskReview
{

    #region utility classes

    public class TaskState 
    {
        public string Name {get; set;}
        public int ThreadNum {get; set;}
    }

    #endregion utility classes

    public class TPLReview
    {

        #region Actions

        Action<string> PrintMsgAction = (msg) =>
        {
            Console.WriteLine(String.Format("PrintMsgAction() called with msg: {0}", msg));
            Thread.Sleep(2000);
            Console.WriteLine(String.Format("The message is: {0}", msg));
        };

        Action<int, int> AddIntegers = (num1, num2) => { Console.WriteLine(String.Format("{0} + {1} is {2}", num1, num2, num1+num2));};

        #endregion Actions

        int counter = 0;
        object sem1 = new object();

        public TPLReview()
        {

        }

        public void Run()
        {

            Console.WriteLine("Starting TaskBasics ...");

            bool runInitiationPatterns = false;
            if (runInitiationPatterns) 
            {
                //There are 3 primary task initiations patterns, ie., new Task followed by Start, Task.Run and 
                // Task.Facory.StartNew
                
                Console.WriteLine("Runninng task initiation patterns ...");
                Task ipTask1 = new Task(() => PrintMsgAction("Action Task 1"));
                Task ipTask2 = Task.Run(() => {Console.WriteLine("Lambda expression ipTask2");});
                ipTask1.Start();
                Task.WaitAll(ipTask1, ipTask2);


                // StartNew(...) gives us more task options than Task.Run(...) such as passing task state
                List<Task> statefulTasks = new List<Task>();
                for(int i=0; i<5; i++)
                {
                    statefulTasks.Add(
                        Task.Factory.StartNew((Object obj) => 
                        {
                            TaskState state = obj as TaskState;
                            state.ThreadNum = Thread.CurrentThread.ManagedThreadId;
                            Console.WriteLine(String.Format("Task {0} is running", state.Name));
                        }, new TaskState() {Name = String.Format("Stateful Task {0}", i+1)}
                        ));
                }

                Task.WaitAll(statefulTasks.ToArray());
                foreach(Task task in statefulTasks)
                {
                    TaskState state = task.AsyncState as TaskState;
                    Console.WriteLine(String.Format("[Stateful task execution info] Name = {0}, thread = {1}, status = {2}", state.Name, state.ThreadNum, task.Status));
                }

                //Specifying task scheduling options
                //Available options None, PreferFairness (scheduling), LongRunning, AttachdToParent (for sub-tasks), ...
                Task rcoTask = new Task(() => PrintMsg("runCreationOptions task running ..."), TaskCreationOptions.LongRunning | TaskCreationOptions.PreferFairness);
                rcoTask.Start();
                Task.WaitAll(rcoTask); 

            }

             bool runTaskParemeter = false;
            if (runTaskParemeter)
            {
                 string w = "";
                Task temp = new Task((o) => {Console.WriteLine(0);}, w);

                //With Task.Run as this is a single statment you cannot pass any parametrs. 
                //You can access variables in the task but be aware as the task in on a separate thread so multiple
                //threads may update the same variable. 
                int i = 1;
                int iCopy = i; //Create a copy for the task, just to be on the safe side. 
                Task.Run(() => ReturnSum(iCopy, 2));

                var startNew = Task<string>.Factory.StartNew((o) => ("holy " + o), "cow");

                //https://www.dotnetforall.com/correct-way-provide-input-parameter-task/
               

            } 

            bool runParallel = false;
            if (runParallel)
            {
                Parallel.Invoke(() => AddIntegers(3,5), () => {Console.WriteLine("2nd task running in parallel");} );
            }

            bool runTaskCancellation = false;
            if (runTaskCancellation) {

                Console.WriteLine("Runninng task cancellation ...");
                CancellationTokenSource cts = new CancellationTokenSource();
                Task cancelTask = new Task(() => PrintMsgAction("Action Task 1"), cts.Token);
                cancelTask.Start();
                cts.Cancel();
                try 
                {
                    cancelTask.Wait(cts.Token);
                   //Task.WaitAll(new Task[] {cancelTask}, cts.Token);
                }
                catch (OperationCanceledException ocex)
                {
                    //Note: An exception *is* thrown when waiting on a task that is cancelled 
                }

                Console.WriteLine(String.Format("Task Cancellation completed. Task status = {0}, IsCancelled = {1}", cancelTask.Status, cancelTask.IsCanceled));
            }

            bool runTestWait = false;
            if (runTestWait) 
            {
                Console.WriteLine("Runninng task wait ..."); 
                Task.Run(() => PrintMsg("Task 1")).Wait(); // will wait before moving on to next line of code
                Task task2 = Task.Run(() => PrintMsg("Task 2")); // will start task and move on to next line of code
                Task task3 = Task.Run(() => PrintMsg("Task 3")); // will start task and move on to next line of code               
                Task.WaitAll(task2, task3);

                Console.WriteLine(String.Format("Tasks wait completed. Task2 status = {0}, Task3 status = {1}", task2.Status, task3.Status));
            }

            bool runTaskResult = false;
            if (runTaskResult)
            {
                Task<int> rtTask1 = Task.Factory.StartNew(() => ReturnSum(1,2));
                int result = rtTask1.Result + Task<int>.Run(() => ReturnSum(3,4)).Result; // calling task.Result will block
                Console.WriteLine(String.Format("Running task result .... 1+2+3+4 = {0}", result));

                // For more details on returning a result from a task see 
                // https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/how-to-return-a-value-from-a-task

                // Note: Task.Result should be avoided when using async as this could result in a deadlok. More on this later.
            }

            bool runTaskContinuations = true;
            if (runTaskContinuations)
            {
                
                
               
            }

            Console.WriteLine("Completed TaskBasics.");
        }

        #region methods

        private int ReturnSum(int num1, int num2) 
        {
            return num1 + num2;
        }
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