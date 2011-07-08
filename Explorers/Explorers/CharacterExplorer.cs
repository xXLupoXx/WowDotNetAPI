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

        private readonly IJsonSource _JsonSource;
        private readonly JavaScriptSerializer _Serializer;

        string Region { get; set; }

        public CharacterExplorer(IJsonSource jsonSource) : this("us", jsonSource) { }

        public CharacterExplorer(string region, IJsonSource jsonSource)
        {
            if (region == null) throw new ArgumentNullException("region");
            if (jsonSource == null) throw new ArgumentNullException("jsonSource");

            this.Region = region;

            _JsonSource = jsonSource;
            _Serializer = new JavaScriptSerializer();
        }

        public Character GetSingleCharacter(string name, string realm, params string[] optionalFields)
        {
            string url;
            if (optionalFields != null && optionalFields.Length > 0)
            {
                var optionalFieldList = String.Join(",", optionalFields);
                url = string.Format(baseCharacterAPIurlWithFields, Region, realm, name, optionalFieldList);
            }
            else
            {
                url = string.Format(baseCharacterAPIurl, Region, realm, name);
            }

            var json = _JsonSource.GetJson(url);
            return _Serializer.Deserialize<Character>(json);
        }
    }
}
