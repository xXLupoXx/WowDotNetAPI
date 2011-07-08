using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WowDotNetAPI.Explorers.Models;
using System.Web.Script.Serialization;
using System.Net;
using System.IO;

namespace WowDotNetAPI.Explorers.Explorers
{
	public class CharacterExplorer
	{
		private const string baseCharacterAPIurl = "http://{0}.battle.net/api/wow/character/{1}/{2}";
		private const string baseCharacterAPIurlWithFields = "http://{0}.battle.net/api/wow/character/{1}/{2}?fields={3}";

		string Region { get; set; }
		JavaScriptSerializer Serializer { get; set; }
		WebRequest Request { get; set; }
		string ProxyURL { get; set; }
		string ProxyUser { get; set; }
		string ProxyPassword { get; set; }
		bool HasProxy { get; set; }

		public CharacterExplorer() : this("us") { }

		public CharacterExplorer(string region)
		{
			this.Region = region;
			this.Serializer = new JavaScriptSerializer();
			this.HasProxy = false;
		}

		public CharacterExplorer(string proxyUser, string proxyPassword, string proxyURL)
			: this("us")
		{
			this.ProxyUser = proxyUser;
			this.ProxyPassword = proxyPassword;
			this.ProxyURL = proxyURL;
			this.HasProxy = true;
		}

		public CharacterExplorer(string region, string proxyUser, string proxyPassword, string proxyURL)
			: this(region)
		{
			this.ProxyUser = proxyUser;
			this.ProxyPassword = proxyPassword;
			this.ProxyURL = proxyURL;
			this.HasProxy = true;
		}

		public Character GetSingleCharacter(string name, string realm, params string[] optionalFields)
		{
			if(optionalFields != null && optionalFields.Length > 0)
				return Serializer.Deserialize<Character>(GetJson(string.Format(baseCharacterAPIurlWithFields, Region, realm, name, GetOptionalFieldList(optionalFields))));
			else
				return Serializer.Deserialize<Character>(GetJson(string.Format(baseCharacterAPIurl, Region, realm, name)));
		}

		private string GetOptionalFieldList(string[] optionalFields)
		{
			string fieldList = optionalFields[0];

			for (int i = 1; i < optionalFields.Length; i++)
				fieldList += "," + optionalFields[i];

			return fieldList;
		}

		private string GetJson(string url)
		{
			Request = WebRequest.Create(url);

			if (HasProxy)
			{
				WebProxy proxy = new WebProxy(ProxyURL);
				proxy.Credentials = new NetworkCredential(ProxyUser, ProxyPassword);
				Request.Proxy = proxy;
			}

			WebResponse response = Request.GetResponse();
			StreamReader reader = new StreamReader(response.GetResponseStream());
			return reader.ReadToEnd();
		}
	}
}
