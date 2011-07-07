using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WowDotNetAPI.Explorers.Models
{
	public class GuildPerkSpell
	{
		public int id { get; set; }
		public string name { get; set; }
		public string subtext { get; set; }
		public string icon { get; set; }
		public string description { get; set; }
		public string range { get; set; }
		public string castTime { get; set; }
		public string cooldown { get; set; }
	}
}
