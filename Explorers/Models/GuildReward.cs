using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WowDotNetAPI.Explorers.Models
{
	public class GuildReward
	{
		public int minGuildLevel { get; set; }
		public int minGuildRepLevel { get; set; }
		public int[] races { get; set; }
		public Achievement achievement { get; set; }
		public Item item { get; set; }
	}
}
