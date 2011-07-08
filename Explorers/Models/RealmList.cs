using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WowDotNetAPI.Explorers.Models
{
	public class RealmList
	{
        public RealmList()
        {
            this.realms = new List<Realm>();
        }

        public IEnumerable<Realm> realms { get; set; }
		
		public void FilterByQueue(bool queue)
		{
			realms = realms.Where(r => r.queue == queue);
		}

		public void FilterByStatus(bool status)
		{
			realms = realms.Where(r => r.status == status);
		}

		public void FilterByPopulation(string population)
		{
			realms = realms.Where(r => r.population.Equals(population, StringComparison.InvariantCultureIgnoreCase));
		}

		public void FilterByType(string type)
		{
			realms = realms.Where(r => r.type.Equals(type, StringComparison.InvariantCultureIgnoreCase));
		}
	}
}
