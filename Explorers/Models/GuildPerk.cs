using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WowDotNetAPI.Explorers.Models
{
	public class GuildPerk
	{
		public int guildLevel { get; set; }
		public GuildPerkSpell spell { get; set; }
	}
}
