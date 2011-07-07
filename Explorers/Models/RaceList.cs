using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WowDotNetAPI.Explorers.Models
{
	public class RaceList
	{
		public IEnumerable<Race> races { get; set; }

		public Race GetRaceById(int id)
		{
			return races.Single(r => r.id == id);
		}

		public Race GetRaceByName(string name)
		{
			return races.Single(r => r.name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
		}

		public IEnumerable<Race> GetRacesBySide(string side)
		{
			return races.Where(r => r.side.Equals(side, StringComparison.InvariantCultureIgnoreCase));
		}
	}
}
