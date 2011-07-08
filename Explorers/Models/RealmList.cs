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
	}
}
