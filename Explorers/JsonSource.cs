using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace WowDotNetAPI
{
	public class JsonSource : JsonSourceBase
	{
		public override string GetJsonFromSanitizedUrl(string sanitizedUrl)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sanitizedUrl);

			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			StreamReader reader = new StreamReader(response.GetResponseStream());
			return reader.ReadToEnd();
		}
	}
}
