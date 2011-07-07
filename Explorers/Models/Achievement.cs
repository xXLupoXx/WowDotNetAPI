using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WowDotNetAPI.Explorers.Models
{
	public class Achievement
	{
		public int id { get; set; }
		public string title { get; set; }
		public int points { get; set; }
		public string description { get; set; }
		public string reward { get; set; }
		public Item rewardItem { get; set; }
	}
}
