/*
 * See https://devblogs.microsoft.com/pfxteam/implementing-a-simple-foreachasync-part-2/
 * 
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accounts.Ports.Events
{
    /// <summary>
    /// Extension methods, utiltuy
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Iterates over a collection asynchronously
        /// </summary>
        /// <param name="source">The collection to iterate over</param>
        /// <param name="dop">Create partitions to iterate over</param>
        /// <param name="body">The work todo</param>
        /// <typeparam name="T">The type</typeparam>
        /// <returns></returns>
        public static Task ForEachAsync<T>(this IEnumerable<T> source, int dop, Func<T, Task> body)
        {
            return Task.WhenAll(
                from partition in Partitioner.Create(source).GetPartitions(dop)
                select Task.Run((Func<Task>) async delegate {
                    using (partition)
                        while (partition.MoveNext())
                            await body(partition.Current);
                }));
        }
    }
}