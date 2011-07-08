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

        private readonly IJsonSource jsonSource;
        private readonly string region;
        private readonly JavaScriptSerializer serializer;

        public CharacterExplorer(IJsonSource jsonSource) : this("us", jsonSource, new JavaScriptSerializer()) { }

        public CharacterExplorer(string region, IJsonSource jsonSource, JavaScriptSerializer serializer)
        {
            if (region == null) throw new ArgumentNullException("region");
            if (jsonSource == null) throw new ArgumentNullException("jsonSource");
            if (serializer == null) throw new ArgumentNullException("serializer");

            this.region = region;

            this.jsonSource = jsonSource;
            this.serializer = serializer;
        }

        public Character GetSingleCharacter(string name, string realm, params string[] optionalFields)
        {
            string url;
            if (optionalFields != null && optionalFields.Length > 0)
            {
                var optionalFieldList = String.Join(",", optionalFields);
                url = string.Format(baseCharacterAPIurlWithFields, this.region, realm, name, optionalFieldList);
            }
            else
            {
                url = string.Format(baseCharacterAPIurl, this.region, realm, name);
            }

            var json = jsonSource.GetJson(url);
            return serializer.Deserialize<Character>(json);
        }
    }
}
