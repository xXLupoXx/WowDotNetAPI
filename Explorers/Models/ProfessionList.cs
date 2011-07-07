using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WowDotNetAPI.Explorers.Models
{
	public class ProfessionList
	{
		public IEnumerable<Profession> primary { get; set; }
		public IEnumerable<Profession> secondary { get; set; }
	}
}
