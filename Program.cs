using System.Runtime.CompilerServices;
using AsyncTest;

Console.WriteLine("Hello, There!");

//NOTE: First and second examples are working asynchronously because the second method uses await and firt one is not
//usage of await keyword can actually kill the async proccess if we need to continue on.
//but must be used if we need to receive some kind of input from the method that is awaited.
//AsyncMethods.FirstExample();

//await AsyncMethods.SecondExample();

//await AsyncMethods.ThirdExample();

////IMPORTANT NOTE: While running test its found as (value > 10^9) values of array is performant to use parralel calculations.
////Not parallel version of 4th example
//await AsyncMethods.FourthExample();

////Parallel version of 4th example
////var newVar = 
//    await AsyncMethods.FourthExampleAsync();

//await AsyncMethods.FifthExample();

//await AsyncMethods.SixthExample();

////Example usage of TaskCompletionSource's
////NOTE: Alternative is a better example for any use case scenario
////await AsyncMethods.SeventhExample();
//await AsyncMethods.SeventhExample_Alternative();

////Exampel for repeating processes
//await AsyncMethods.EightthExample();

await AsyncMethods.NinthExample();






