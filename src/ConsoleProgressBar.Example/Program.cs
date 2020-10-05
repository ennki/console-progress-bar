using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConsoleProgressBar;

namespace ConsoleProgressBar.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");

            var messages =  Enumerable.Range(1, 1000).Select(e => e.ToString()).ToList();

            using (var progressBar = new ProgressBar(messages.Count()))
            {
                var random = new Random();

                using var throttler = new SemaphoreSlim(16);
                var tasks = new List<Task>();
                foreach (var msg in messages)
                {
                    throttler.Wait();

                    var t = Task.Run(async () =>
                    {
                        try
                        {
                            await Task.Delay(100);
                            int chance;
                            lock(random)
                            {
                                chance = random.Next(1, 100);
                            }

                            if (chance < 5)
                            {
                                //Console.WriteLine($"some random text {chance}");
                            }

                            progressBar.Increment();
                        }
                        finally
                        {
                            throttler.Release();
                        }
                    });

                    tasks.Add(t);
                }

                Task.WaitAll(tasks.ToArray());
            }

            Console.WriteLine("Done");
        }
    }
}
