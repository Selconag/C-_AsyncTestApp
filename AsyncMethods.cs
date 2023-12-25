using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
namespace AsyncTest;

public class AsyncMethods
{
    static Task firstTask, secondTask, thirdTask;

    public static async Task Turret1(string method)
    {
        await Task.Run(() =>
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"Firing M249 from {method}");
                Task.Delay(100).Wait();
            }
        });
    }

    public static async Task Turret2(string method)
    {
        await Task.Run(() =>
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"Firing Howitzer from {method}");
                Task.Delay(200).Wait();
            }
        });

    }

    public static async Task Turret3(string method)
    {
        await Task.Run(() =>
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"Firing Railgun from {method}");
                Task.Delay(350).Wait();
            }
        });

    }

    public static string Turret2_Alt()
    {
        return "Firing Howitzer is over";
    }

    public static string Turret3_Alt()
    {
        return "Firing Railgun is over";

    }


    // Here's our (hypothetical) expensive calculation
    static int PerformExpensiveCalculation(int value)
    {
        return value * value; // For simplicity, let's just square it
    }

    public static async Task FirstExample()
    {
        Console.WriteLine("----------First Run-----------");
        Console.WriteLine("Press a button to start.");
        Console.ReadKey();
        Console.WriteLine();

        string methodName = "First Run";
        firstTask = Turret1(methodName);
        secondTask = Turret2(methodName);
        thirdTask = Turret3(methodName);

        var generalTasks = new List<Task> { firstTask, secondTask, thirdTask };

        //Runs asynchronously and triggered once for each tasks end
        while (generalTasks.Count > 0)
        {
            Task finishedTask = await Task.WhenAny(generalTasks);
            if (finishedTask == firstTask)
            {
                Console.WriteLine("Turret 1 stopped");
            }
            else if (finishedTask == secondTask)
            {
                Console.WriteLine("Turret 2 stopped");
            }
            else if (finishedTask == thirdTask)
            {
                Console.WriteLine("Turret 3 stopped");
            }
            await finishedTask;
            generalTasks.Remove(finishedTask);
        }
        Console.WriteLine($"All turrets finished on {methodName}");

    }
    /// <summary>
    /// In order to run same tasks again without finishing immediately since they already have a completed state in them
    /// Is to reassign them. It may look primite but it is what it is
    /// </summary>
    /// <returns></returns>
    public static async Task SecondExample()
    {
        Console.WriteLine("----------Second Run-----------");
        Console.WriteLine("Press a button to start.");
        Console.ReadKey();
        Console.WriteLine();
        string methodName = "Second Run";
        firstTask = Turret1(methodName);
        secondTask = Turret2(methodName);
        thirdTask = Turret3(methodName);
        var generalTasks2 = new List<Task> { firstTask, secondTask, thirdTask };

        //await Task.WhenAll(generalTasks2);
        await Task.WhenAll(firstTask, secondTask, thirdTask);
        Console.WriteLine($"All turrets finished on {methodName}");
    }

    public static async Task ThirdExample()
    {
        Console.WriteLine("----------Third Run-----------");
        Console.WriteLine("This process is started automatically");
        // Sample of creating a task with Task Factory Start New with cancellation
        var cancellationTokenSource = new CancellationTokenSource();
        Task longRunningTask = Task.Factory.StartNew(() =>
        {
            // This code will run in a separate thread
            // Check for cancellation request frequently so the task can be stopped
            for (int i = 0; i < 100; i++)
            {
                if (cancellationTokenSource.Token.IsCancellationRequested)
                {
                    break;
                }
                Console.WriteLine($"Iteration {i}");
                Thread.Sleep(100);
            }
        }, cancellationTokenSource.Token);

        // Tell user if stop is entered through console, the execution will stop
        var stopTask = Task.Run(() =>
        {
            Console.WriteLine("Type 'Stop' to cancel the task.");
            while (!cancellationTokenSource.IsCancellationRequested)
            {
                var key = Console.ReadLine();
                if (key != null && key.Trim().Equals("Stop", StringComparison.OrdinalIgnoreCase))
                {
                    cancellationTokenSource.Cancel();
                }
            }
        });

        await Task.WhenAny(longRunningTask, stopTask); // Wait for either task to complete

        // Check if the longRunningTask completed or was canceled
        if (longRunningTask.IsCompleted)
        {
            Console.WriteLine("Task completed.");
        }
        else
        {
            Console.WriteLine("Task canceled.");
        }
        Console.WriteLine("Press a button to exit the program");

    }

    public static Task FourthExample()
    {
        Console.WriteLine("----------Fourth Run | Non Async-----------");
        Console.WriteLine("Press a button to start.");
        Console.ReadKey();
        var watch = System.Diagnostics.Stopwatch.StartNew();

        // Initialize an array
        int[] data = Enumerable.Range(0, 1000000000).ToArray();

        foreach (var number in data)
        {
            //Console.WriteLine(PerformExpensiveCalculation(number));
            PerformExpensiveCalculation(number);
        }

        watch.Stop();
        Console.WriteLine($"Performed in: {watch.ElapsedMilliseconds} milliseconds");

        return Task.FromResult(data);
    }

    public static async Task FourthExampleAsync()
    {
        Console.WriteLine("----------Fourth Run | Async-----------");
        Console.WriteLine("Press a button to start.");
        Console.ReadKey();
        var watch = System.Diagnostics.Stopwatch.StartNew();
        // Initialize an array
        int[] data = Enumerable.Range(0, 1000000000).ToArray();


        // Transform the data in parallel
        Parallel.For(0, data.Length, i =>
        {
            // This calculation takes place in parallel
            data[i] = PerformExpensiveCalculation(data[i]);
        });
        watch.Stop();
        Console.WriteLine($"Performed in: {watch.ElapsedMilliseconds} milliseconds");
    }


    public static async Task FifthExample()
    {
        // Declare the tasks
        Task[] tasks = new Task[10];

        // Initialize and start the tasks
        for (var i = 0; i < 10; i++)
        {
            int taskNum = i; // To avoid closure on the loop variable
            tasks[i] = new Task(() =>
            {
                System.Threading.Thread.Sleep(1000);
                Console.WriteLine($"Task {taskNum} complete!");
            });
        }

        // Now, let’s start our tasks
        foreach (var task in tasks)
        {
            task.Start();
        }

        // And wait for all of them to complete
        Task.WaitAll(tasks);
    }

    public static async Task SixthExample()
    {
        // This example represent multiple asynchronous tasks
        List<Task> tasks = new List<Task>();
        for (int i = 0; i < 2000; i++)
        {
            int taskNum = i;  // To avoid the closure variable trap
            tasks.Add(Task.Run(async () =>
            {
                // Simulate a long-running task
                await Task.Delay(1000);
                Console.WriteLine($"Task {taskNum} completed!");
            }));
        };
        Console.WriteLine("Waiting for all tasks to complete...");
        await Task.WhenAll(tasks);
        Console.WriteLine("All tasks completed!");
    }

    public static async Task SeventhExample()
    {
        // Create a TaskCompletionSource
        var tcs = new TaskCompletionSource<bool>();

        // Start a task that waits for user input
        Task someTask = Task.Run(() =>
        {
            // Callback to signal that the operation has completed
            tcs.SetResult(true); // You can also use tcs.SetException to signal an error
        });


        // Now you can await the task
        await tcs.Task; //Equivalent of calling directly someTask

        // Output: Operation Complete!
        Console.WriteLine("Operation Complete!");
    }

    public static async Task SeventhExample_Alternative()
    {
        // Create a TaskCompletionSource
        var tcs = new TaskCompletionSource<int>();

        // Start a task that waits for user input
        Task.Run(() =>
        {
            Console.WriteLine("Enter a number:");
            string userInput = Console.ReadLine();

            int number;
            if (int.TryParse(userInput, out number))
            {
                // Set the result if parsing is successful
                tcs.SetResult(number);
            }
            else
            {
                // Set an exception if parsing fails
                tcs.SetException(new ArgumentException("Invalid input. Please enter a valid number."));
            }
        });

        // Wait for the task completion
        try
        {
            int result = await tcs.Task;
            Console.WriteLine($"You entered: {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        // Now you can await the task
        await tcs.Task;

        // Output: Operation Complete!
        Console.WriteLine("Operation Complete!");
    }

    //Bad example given by website, will be fixed later
    public static async Task EightthExample()
    {
        // Dummy method that fetches and processes customer data
        async Task ProcessCustomerDataAsync()
        {
            Console.WriteLine("Data processing started");

            // Simulate time-consuming data processing
            await Task.Delay(TimeSpan.FromSeconds(10));

            Console.WriteLine("Data processing completed after 10 second.");
        }

        Timer timer = new Timer(state =>
        {
            Task.Run(async () => await ProcessCustomerDataAsync());
        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

        await ProcessCustomerDataAsync();
    }

    public static async Task NinthExample()
    {
        //Chaining tasks using async
        var firstTask = new Task<string>(Turret3_Alt);
        var secondTask = new Task<string>(Turret2_Alt);
        firstTask.ContinueWith(t => secondTask.Start());
    }

}
