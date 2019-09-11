using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;


// Tasked based Anync Pattern review

namespace TAP
{

    //Review notes - see readme.com. Key design best practices ...
    //An asynchronous method should minimize the synchronous work it needs to do.
    //An asynchronous method should throw an exception only in response to a usage error. 

    public class TAP
    {

        public void Run()
        {

            Console.WriteLine("Starting TAP Pattern review ...");
 
            Task<int> getContentsAsync = GetContentsAsync("http://developer.microsoft.com/");
            Task<int> readTask = ReadTask("http://developer.microsoft.com/");
            Task.WaitAll(getContentsAsync, readTask);
            Console.WriteLine("GetContentsAsync: length of www.msdn.com is {0}", getContentsAsync.Result);
            Console.WriteLine("ReadTask: length of www.msdn.com is {0}", readTask.Result);      
            
            //See Task Status, Cancellation and Progress Reporting. 
            //Not addressed here - throttling, 

            Console.WriteLine("Completed TAP Pattern review");

        }

        #region TAP and auxiliary methods
        
        // Using compiler to generate TAP method (i.e., adding async keyword)
        public async Task<int> GetContentsAsync(string url) 
        {
            string retStr = "";
            HttpClient client = new HttpClient();
            retStr = await client.GetStringAsync(url); 
            return retStr.Length;
        }

        //Using the manual method (i.e., create a TaskCompletionSource and setResult, SetException, SetCancel, etc)
        public Task<int> ReadTask(string url)
        {
            var tcs = new TaskCompletionSource<int>();
            try
            {
                HttpClient client = new HttpClient();
                tcs.SetResult(client.GetStringAsync(url).Result.Length);
            }
            catch (Exception exc) 
            {
                tcs.SetException(exc); 
            }
            return tcs.Task;
        }

        //Hybrid approach (not shown here) - implement the TAP pattern manually in one method but delegate the core 
        //logic for the implementation to another method

        public async Task<int> GetContentsAsyncException(string url) 
        {
            string retStr = "";
            HttpClient client = new HttpClient();
            retStr = await client.GetStringAsync(url); 
            throw new ApplicationException();
            return retStr.Length;

        }

        #endregion TAP and auziliary methods
    }
}
