## Task Programming Review

_**Note** This bilds on [NetAsync](https://github.com/radirckze/NetCoreBasics/tree/master/NetAsync) in the NetCoreBasics repo_  

### Reading / Resources:  
https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task?view=netframework-4.8  
https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/task-based-asynchronous-programming  
https://blog.stephencleary.com/2014/02/synchronous-and-asynchronous-delegate.html  
https://docs.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/task-based-asynchronous-pattern-tap  

### Task Parallel Library (TPL) related notes  

### TAP related notes  

* TAP uses a single method to represent the initiation and completion of an asynchronous operation (in contrast to APM and EAP patterns.  
* Generally asynchronous methods in TAP include the Async suffix after the operation name for methods that return awaitable types, for example, GetAsync.  
* TAP methods return TAsks.Task or a Tasks.Task<TResult> depending on whether the corresponding synchronous method returns void a some type.  
  * Exceptions are out an ref parameters that should generally be avoided (but can be added to TResult where necessary).  
* TAP methods can do a small amount of work synchronously, such as validating arguments, which should be kept to a minimum, so the method can return quickly.   
* An asynchronous method should throw an exception only in response to a usage error (e.g., user passes null to a paremeter). Usage errors should never occur in production code. For example, if passing a null reference (Nothing in Visual Basic) as one of the method’s arguments causes an error state (usually represented by an ArgumentNullException exception), you can modify the calling code to ensure that a null reference is never passed. For all other errors, exceptions that occur when an asynchronous method is running should be assigned to the returned task  
* See Task Status, Cancellation and Progress Reporting. 

**Implementing TAP Pattern**  
You can implement the Task-based Asynchronous Pattern (TAP) in three ways: by using the compilers in Visual Studio, manually, or a combination of the compiler and manual methods. 

**Workload Best Practices**  
While TAP methods can be used for compute-bound and I/O-bound asynchronous operations, when TAP methods are exposed publicly from a library, they should be provided only for workloads that primarily involve I/O-bound operations. If a method is purely compute-bound, it should be exposed only as a synchronous implementation. The code that consumes it may then choose whether to wrap it in a task to offload the work to another thread.

