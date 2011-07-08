using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.IO;
using WowDotNetAPI.Explorers.Models;
using WowDotNetAPI.Explorers.Interfaces;
using WowDotNetAPI.Explorers.Extensions;

namespace WowDotNetAPI.Explorers.Explorers
{
    public class RealmExplorer : IRealmExplorer
    {
        private const string baseRealmAPIurl = "http://{0}.battle.net/api/wow/realm/status{1}";

        private readonly IJsonSource _JsonSource;
        private readonly JavaScriptSerializer _Serializer;

        public string Region { get; set; }

        public RealmExplorer(IJsonSource jsonSource) : this("us", jsonSource, new JavaScriptSerializer()) { }

        public RealmExplorer(string region, IJsonSource jsonSource, JavaScriptSerializer serializer)
        {
            if (region == null) throw new ArgumentNullException("region");
            if (jsonSource == null) throw new ArgumentNullException("jsonSource");

            this.Region = region;

            _JsonSource = jsonSource;
            _Serializer = serializer;
        }

        public Realm GetSingleRealm(string name)
        {
            var realmList = GetMultipleRealms(name);
            return realmList == null ? null : realmList.FirstOrDefault();
        }

        public IEnumerable<Realm> GetAllRealms()
        {
            return GetRealmData(string.Format(baseRealmAPIurl, Region, string.Empty));
        }

        public IEnumerable<Realm> GetRealmsByType(string type)
        {
            var realmList = GetRealmData(string.Format(baseRealmAPIurl, Region, string.Empty));
            realmList = realmList.WithType(type);
            return realmList;
        }

        public IEnumerable<Realm> GetRealmsByPopulation(string population)
        {
            var realmList = GetRealmData(string.Format(baseRealmAPIurl, Region, string.Empty));
            realmList = realmList.WithPopulation(population);
            return realmList;
        }

        public IEnumerable<Realm> GetRealmsByStatus(bool status)
        {
            var realmList = GetRealmData(string.Format(baseRealmAPIurl, Region, string.Empty));
            if (status)
            {
                realmList = realmList.WhereUp();
            }
            else
            {
                realmList = realmList.WhereDown();
            }
            return realmList;
        }

        public IEnumerable<Realm> GetRealmsByQueue(bool queue)
        {
            var realmList = this.GetRealmData(string.Format(baseRealmAPIurl, Region, string.Empty));
            if (queue)
            {
                realmList = realmList.WithQueue();
            }
            else
            {
                realmList = realmList.WithoutQueue();
            }

            return realmList;
        }

        public IEnumerable<Realm> GetMultipleRealms(params string[] names)
        {
            if (names == null
                || names.Length == 0
                || names.Any(r => r == null))
            {
                return Enumerable.Empty<Realm>();
            }

            var query = "?realms=" + String.Join(",", names);

            return GetMultipleRealmsViaQuery(query);
        }

        public IEnumerable<Realm> GetMultipleRealmsViaQuery(string query)
        {
            if (string.IsNullOrEmpty(query)) throw new ArgumentException("Value must not be null or empty.", "query");

            try
            {
                var url = string.Format(baseRealmAPIurl, Region, query);
                return GetRealmData(url);
            }
            catch
            {
                return null;
            }
        }

        public string GetRealmsByTypeAsJson(string type)
        {
            return ConvertRealmListToJson(GetRealmsByType(type));
        }

        public string GetRealmsByPopulationAsJson(string population)
        {
            return ConvertRealmListToJson(GetRealmsByPopulation(population));
        }

        public string GetRealmsByStatusAsJson(bool status)
        {
            return ConvertRealmListToJson(GetRealmsByStatus(status));
        }

        public string GetRealmsByQueueAsJson(bool queue)
        {
            return ConvertRealmListToJson(GetRealmsByQueue(queue));
        }

        public string GetAllRealmsAsJson()
        {
            var url = string.Format(baseRealmAPIurl, Region, string.Empty);
            return _JsonSource.GetJson(url);
        }

        public string GetSingleRealmAsJson(string name)
        {
            return ConvertRealmListToJson(GetMultipleRealms(name));
        }

        public string GetMultipleRealmsAsJson(params string[] mames)
        {
            return ConvertRealmListToJson(GetMultipleRealms(mames));
        }

        public string GetRealmsViaQueryAsJson(string query)
        {
            var url = string.Format(baseRealmAPIurl, Region, query);
            return _JsonSource.GetJson(url);
        }

        private string ConvertRealmListToJson(IEnumerable<Realm> realmList)
        {
            return _Serializer.Serialize(realmList);
        }

        public IEnumerable<Realm> GetRealmData(string url)
        {
            var sanitizedUrl = this.SanitizeUrl(url);
            var json = _JsonSource.GetJson(sanitizedUrl);
            var dictionary = _Serializer.Deserialize<Dictionary<string, List<Realm>>>(json);
            return dictionary["realms"];
        }

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
