using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WowDotNetAPI.Interfaces;

namespace WowDotNetAPI
{
	public abstract class JsonSourceBase : IJsonSource
	{
		public string GetJson(string url)
		{
			string sanitizedUrl = this.SanitizeUrl(url);
			return this.GetJsonFromSanitizedUrl(sanitizedUrl);
		}

		public abstract string GetJsonFromSanitizedUrl(string sanitizedUrl);

		//Todo: Improve URL sanitizer
		//http://stackoverflow.com/questions/25259/how-do-you-include-a-webpage-title-as-part-of-a-webpage-url/25486#25486
		private string SanitizeUrl(string url)
		{
			if (string.IsNullOrEmpty(url)) return "";

			url = Regex.Replace(url.Trim(), @"\s+", "-");
			url = Regex.Replace(url, "[#']", "");

			return url;
		}
	}
}
