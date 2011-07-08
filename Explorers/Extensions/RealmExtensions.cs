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
        public static RealmList WithQueue(this RealmList source)
        {
            return new RealmList() { realms = source.realms.Where(r => r.queue) };
        }

        public static RealmList WithoutQueue(this RealmList source)
        {
            return new RealmList() { realms = source.realms.Where(r => !r.queue) };
        }

        public static RealmList WhereUp(this RealmList source)
        {
            return new RealmList() { realms = source.realms.Where(r => r.status) };
        }

        public static RealmList WhereDown(this RealmList source)
        {
            return new RealmList() { realms = source.realms.Where(r => !r.status) };
        }

        public static RealmList WithPopulation(this RealmList source, string population)
        {
            return new RealmList() { realms = source.realms.Where(r => r.population.Equals(population, StringComparison.InvariantCultureIgnoreCase)) };
        }

        public static RealmList WithType(this RealmList source, string realmType)
        {
            return new RealmList() { realms = source.realms.Where(r => r.type.Equals(realmType, StringComparison.InvariantCultureIgnoreCase)) };
        }
    }
}
