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
            var container = new Container(x =>
            {
                // Using StructureMap style registrations
                x.For<IScopedGuidProvider>().Use<ScopedGuidProvider>().Scoped();
                x.For<IScopedGuidResolver>().Use<ScopedGuidResolver>().Scoped();
                x.For<ITransientGuidProvider>().Use<TransientGuidProvider>();
            });

            Console.WriteLine("Using 100 degrees of parallelism and generating 1000000 guids");

            ConcurrentQueue<Guid> produce = new ConcurrentQueue<Guid>();

            var parallelOptions = new ParallelOptions();
            parallelOptions.MaxDegreeOfParallelism = 100;

            Parallel.For(0, 1000000, parallelOptions, (iteration) =>
            {
                //produce.Enqueue(container.GetService<ScopedGuidResolver>().GetGuid());
                using (var scope = container.ServiceProvider.CreateScope())
                {
                    produce.Enqueue(scope.ServiceProvider.GetService<ScopedGuidResolver>().GetGuid());
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
