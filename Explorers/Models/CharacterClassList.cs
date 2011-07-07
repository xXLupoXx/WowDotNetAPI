using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WowDotNetAPI.Explorers.Models
{
	public class CharacterClassList
	{
		public IEnumerable<CharacterClass> classes { get; set; }

		public CharacterClass GetCharacterClassById(int id)
		{
			return classes.Single(c => c.id == id);
		}

		public CharacterClass GetCharacterClassByName(string name)
		{
			return classes.Single(c => c.name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
		}

		public IEnumerable<CharacterClass> GetCharacterClassByPowerType(string powerType)
		{
			return classes.Where(c => c.powerType.Equals(powerType, StringComparison.InvariantCultureIgnoreCase));
		}
	}
}
