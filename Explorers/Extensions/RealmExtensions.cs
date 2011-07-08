using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using WowDotNetAPI.Explorers.Models;

namespace WowDotNetAPI.Explorers.Extensions
{
    static class RealmExtensions
    {
        public static IEnumerable<Realm> WithQueue(this IEnumerable<Realm> source)
        {
            return source.Where(r => r.queue);
        }

        public static IEnumerable<Realm> WithoutQueue(this IEnumerable<Realm> source)
        {
            return source.Where(r => !r.queue);
        }

        public static IEnumerable<Realm> WhereUp(this IEnumerable<Realm> source)
        {
            return source.Where(r => r.status);
        }

        public static IEnumerable<Realm> WhereDown(this IEnumerable<Realm> source)
        {
            return source.Where(r => !r.status);
        }

        public static IEnumerable<Realm> WithPopulation(this IEnumerable<Realm> source, string population)
        {
            return source.Where(r => r.population.Equals(population, StringComparison.InvariantCultureIgnoreCase));
        }

        public static IEnumerable<Realm> WithType(this IEnumerable<Realm> source, string realmType)
        {
            return source.Where(r => r.type.Equals(realmType, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
