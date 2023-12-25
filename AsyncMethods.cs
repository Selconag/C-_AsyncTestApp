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

    public static async Task FirstRun()
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
    public static async Task SecondRun()
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

    public static async Task ThirdRun()
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

    public static async Task RunTaskAsync(Func<Task> taskMethod)
    {
        Task task = taskMethod();
        await task;
    }

    public static void AssignTaskAsync(Task task, Func<Task> method)
    {
        task = null;
        task = method();
    }

   
}
