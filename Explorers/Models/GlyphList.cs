using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WowDotNetAPI.Explorers.Models
{
	public class GlyphList
	{
		public IEnumerable<Glyph> prime { get; set; }
		public IEnumerable<Glyph> major { get; set; }
		public IEnumerable<Glyph> minor { get; set; }
	}
}
