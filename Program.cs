using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lamar;
using Microsoft.Extensions.DependencyInjection;

namespace ScopedPoc
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddScoped<IScopedGuidProvider, ScopedGuidProvider>()
                .AddTransient<ITransientGuidResolver, TransientGuidResolver>()
                .AddTransient<ITransientGuidProvider, TransientGuidProvider>().BuildServiceProvider();

            Console.WriteLine("!!!!! RUNNING WITH DOTNET BASIC DI !!!!!");

            Run(serviceProvider);

            Console.WriteLine("!!!!! RUNNING WITH LAMAR !!!!!");
            var container = new Container(x =>
            {
                // Using StructureMap style registrations
                x.For<IScopedGuidProvider>().Use<ScopedGuidProvider>().Scoped();
                x.For<ITransientGuidResolver>().Use<TransientGuidResolver>();
                x.For<ITransientGuidProvider>().Use<TransientGuidProvider>();
            });

            Run(container.ServiceProvider);
        }

        static void Run(IServiceProvider provider, int degreesOfParallelism = 100, int guidsToGenerate = 1000000)
        {
            Console.WriteLine($"Using {degreesOfParallelism} degrees of parallelism and generating {guidsToGenerate} guids");

            ConcurrentQueue<Guid> produce = new ConcurrentQueue<Guid>();

            var parallelOptions = new ParallelOptions();
            parallelOptions.MaxDegreeOfParallelism = degreesOfParallelism;

            Parallel.For(0, guidsToGenerate, parallelOptions, (iteration) =>
            {
                //produce.Enqueue(container.GetService<ScopedGuidResolver>().GetGuid());
                using (var scope = provider.CreateScope())
                {
                    produce.Enqueue(scope.ServiceProvider.GetService<ITransientGuidResolver>().GetGuid());
                }
            });


            int maxCount = produce.GroupBy(x => x).OrderByDescending(x => x.Count()).First().Count();

            HashSet<Guid> duplicateSet = new HashSet<Guid>();

            for (int i = 2; i < maxCount + 1; i++)
            {
                Console.WriteLine($"The following guids were duplicated {i} times!");

                var duplicates = produce.GroupBy(x => x).Where(x => x.Count() == i)
                .Select(x => x.Key)
                .ToList();

                duplicates.ForEach(x =>
                {
                    Console.WriteLine(x);
                    duplicateSet.Add(x);
                });
            }

            int unique = produce.Where(x => !duplicateSet.Contains(x)).Count();
            Console.WriteLine($"There were {unique} unique guids");
        }
    }
}
