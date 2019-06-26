using System;
using System.Collections;
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

    public class TaskParams1
    {
        public int Val1 {get; set;}
        public int Val2 {get; set;}
    }

    #endregion utility classes

    public class TPLReview
    {

        #region Delegates

        Action<string> PrintMsgAction = (msg) =>
        {
            Console.WriteLine(String.Format("PrintMsgAction() called with msg: {0}", msg));
            Thread.Sleep(2000);
            Console.WriteLine(String.Format("The message is: {0}", msg));
        };

        Action<int, int> AddIntegers = (num1, num2) => { Console.WriteLine(String.Format("{0} + {1} is {2}", num1, num2, num1+num2));};

        Func<int, int, int> ReturnSumFuncDelegate = ReturnSum;

        #endregion Delegates

        int counter = 0;
        object sem1 = new object();

        public TPLReview()
        {
        }

        public void Run()
        {

            Console.WriteLine("Starting Task Parallel Library (TPL) review ...");

            bool runTaskInstantiationOptions = false;
            if (runTaskInstantiationOptions) 
            {

                //Tasks can be created implicitly (i.e., Parallel.Invoke) or explicitly (see below).

                //Creating tasks implicitly
                Parallel.Invoke(() => Console.WriteLine("Implicit taks creation - Parallel.Invoke called."));
                Parallel.Invoke(() => PrintMsgAction("Implicit taks creation - Parallel.Invoke called with Action ..."));
                //You can create and run multiple tasks (in parallel) in same invoke statement
                Parallel.Invoke
                (
                    () => AddIntegers(3,5),
                    () => PrintMsgAction("Implicit taks creation - Parallel.Invoke with multiple tasks")
                );

                //The three options for instantiating a task explicitly, i.e., new Task followed by Start, Task.Run and 
                // Task.Facory.StartNew
                Task ePattern1 = new Task(() => PrintMsgAction("Explicit task instantiation - new Task ...")); 
                ePattern1.Start();
                Task ePattern2 = Task.Run(() => PrintMsgAction("Explicit task instantiation - Task.Run ..."));                
                Task ePattern3 = Task.Factory.StartNew(() => PrintMsgAction("Explicit task instantiation - Task.Facory.StartNew ..."));
                Task.WaitAll(ePattern1, ePattern2, ePattern3);
                //Note, Task.Run and Task.Factory.StartNew are convenience methods. For example, Task.Run uses the default scheduler, 
                //and the StartNew is used when you want to create and start in the same line of code
                
                //When creating a task, the delegate can be a named delegate, an anonymous method or a lambda expression
                Task dPattern1A = new Task(() => PrintMsgAction("Task with named action")); //using named (Action() delegate 
                Task dPattern1B = new Task(() => PrintMsg("Task with named method"));  //using named method 
                Task dPattern2 = new Task(() => {Console.WriteLine("Task with anonymous method");});          
                Task dPattern3 = new Task(() => Console.WriteLine("Task with lambda expression")); //using lambda expression
                
                dPattern1A.Start();
                dPattern1B.Start();
                dPattern2.Start();
                dPattern3.Start();
                Task.WaitAll(dPattern1A, dPattern1B, dPattern2, dPattern3);

                //Example: using  StartNew to pass in state
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

                

                //Example: Specifying task scheduling options
                //Available options None, PreferFairness (scheduling), LongRunning, AttachdToParent (for sub-tasks), ...
                Task rcoTask = new Task(() => PrintMsg("runCreationOptions task running ..."), TaskCreationOptions.LongRunning | TaskCreationOptions.PreferFairness);
                rcoTask.Start();
                Task.WaitAll(rcoTask); 

            }

            bool runTaskParamsAndResult = true;
            if (runTaskParamsAndResult)
            {

                //Recall the two types of Tasks, i.e., one that returns a void and one that returns a result (i.e., Task<TResult>). (So the type 
                //declaration does not include the arguments.)
                int two = 2;
                Task prTask1 = new Task( (x) => 
                {
                    int intx = (int)x;
                    Console.WriteLine(@"Task params and results: {0} squared is {1}", intx, intx*intx);
                }, two);
                prTask1.Start();

                Task<int> prTask2 =  new Task<int>((x) => {return (int)x*(int)x;}, two);
                prTask2.Start();
                Task.WaitAll(prTask1, prTask2);
                if (prTask2.Status == TaskStatus.RanToCompletion)
                {
                    Console.WriteLine(@"The result from prTask2 is: {0}", prTask2.Result);
                }
                else
                {
                     Console.WriteLine(@"The completion status of prTask2 is: {0}", prTask2.Status);
                }

                string prMsg1 = "Task Parameters and Results section - using a delegate";
                Task prTask3 = Task.Factory.StartNew(() => PrintMsgAction(prMsg1));
                Task.WaitAll(prTask3);

                Task<int> prTask4 = Task<int>.Factory.StartNew(() => ReturnSum(1,3));
                Task.WaitAll(prTask4);
                Console.WriteLine(@"[Params and Results Section] The result from ReturnSum(1,3) is {0}", prTask4.Result);

                Task<int> prTask5 = Task<int>.Factory.StartNew(() => ReturnSumFuncDelegate(2,4));
                Task.WaitAll(prTask5);
                Console.WriteLine(@"[Params and Results Section] The result from ReturnSumFuncDelegate(2,4) is {0}", prTask5.Result);

                //Note: we can use the  '=> (...)' to pass arguments to the task constructor, if any.
                //Below we use a the constructor that takes a state object to pass in variables to the task.
                Task<int> prTask6 = Task.Factory.StartNew((Object obj) => 
                    {
                        TaskParams1 taskParams1 = obj as TaskParams1;
                        return taskParams1.Val1 + taskParams1.Val2;
                    }, new TaskParams1() {Val1=3, Val2=6});
                Console.WriteLine(@"[Params and Results Section] Using state object to pass parameters, 3 + 6 is {0}", prTask6.Result);

                // For more details on returning a result from a task see 
                // https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/how-to-return-a-value-from-a-task
                // Note: Task.Result should be avoided when using async as this could result in a deadlok. More on this later.

                //When you create a task and pass in a delegate, can you also pass state to the task method? If so, how do you 
                //acess the state object from withing that method.                
            
            }

            bool runTaskCustomization = false;
            if (runTaskCustomization)
            {

                //Task.StartNew provides more control such as, specifying the scheduler, providing state, etc.
                //TBD


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

            bool runTaskCulture = false;
            if (runTaskCulture)
            {
                //Unless you have sepecified the culture for all threads using  CultureInfo.DefaultThreadCurrentCulture, each
                //thread gets its culture from the System Culture. (Note that a new thread does not get its culture from the
                //the thread that created it. )

                //Starting from .NET 4.6 onwards each task inherits its culture from it's calling thread.
            }

            bool runTaskContinue = false;
            if (runTaskContinue)
            {
                //Task<int, int> squareTask = new Task<int>(() => {return 5*5;});
            }

            bool runTaskSuncAndManagement = false;
            if (runTaskSuncAndManagement)
            {
                //Task cancellation ....
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

                //Taks wait
                 Console.WriteLine("Runninng task wait ..."); 
                Task.Run(() => PrintMsg("Task 1")).Wait(); // will wait before moving on to next line of code
                Task task2 = Task.Run(() => PrintMsg("Task 2")); // will start task and move on to next line of code
                Task task3 = Task.Run(() => PrintMsg("Task 3")); // will start task and move on to next line of code               
                Task.WaitAll(task2, task3);

                Console.WriteLine(String.Format("Tasks wait completed. Task2 status = {0}, Task3 status = {1}", task2.Status, task3.Status));

                //RunSynchronously and Async
            }

            Console.WriteLine("Completed TaskBasics.");
        }

        #region methods

        private static int ReturnSum(int num1, int num2) 
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

        private async Task<int> SumTaskParams1Vals(TaskParams1 taskParams1)
        {
            return taskParams1.Val1 + taskParams1.Val2;
        }

        #endregion methods


    }
}