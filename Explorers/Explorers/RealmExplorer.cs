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

namespace WowDotNetAPI.Explorers.Explorers
{
    public class RealmExplorer : IRealmExplorer
    {
        private const string baseRealmAPIurl = "http://{0}.battle.net/api/wow/realm/status{1}";

        public string Region { get; set; }
        public JavaScriptSerializer Serializer { get; set; }
        public WebRequest Request { get; set; }
        public string ProxyURL { get; set; }
        public string ProxyUser { get; set; }
        public string ProxyPassword { get; set; }
        public bool HasProxy { get; set; }

        public RealmExplorer() : this("us") { }

        public RealmExplorer(string region)
        {
            this.Region = region;
            this.Serializer = new JavaScriptSerializer();
            this.HasProxy = false;
        }

        public RealmExplorer(string proxyUser, string proxyPassword, string proxyURL)
            : this("us")
        {
            this.ProxyUser = proxyUser;
            this.ProxyPassword = proxyPassword;
            this.ProxyURL = proxyURL;
            this.HasProxy = true;
        }

        public RealmExplorer(string region, string proxyUser, string proxyPassword, string proxyURL)
            : this(region)
        {
            this.ProxyUser = proxyUser;
            this.ProxyPassword = proxyPassword;
            this.ProxyURL = proxyURL;
            this.HasProxy = true;
        }

        public Realm GetSingleRealm(string name)
        {
            var realmList = GetMultipleRealms(name);
            return realmList == null ? null : realmList.realms.FirstOrDefault();
        }

        public RealmList GetAllRealms()
        {
            return GetRealmData(string.Format(baseRealmAPIurl, Region, string.Empty));
        }

        public RealmList GetRealmsByType(string type)
        {
            var realmList = GetRealmData(string.Format(baseRealmAPIurl, Region, string.Empty));
            realmList.FilterByType(type);
            return realmList;
        }

        public RealmList GetRealmsByPopulation(string population)
        {
            var realmList = GetRealmData(string.Format(baseRealmAPIurl, Region, string.Empty));
            realmList.FilterByPopulation(population);
            return realmList;
        }

        public RealmList GetRealmsByStatus(bool status)
        {
            var realmList = GetRealmData(string.Format(baseRealmAPIurl, Region, string.Empty));
            realmList.FilterByStatus(status);
            return realmList;
        }

        public RealmList GetRealmsByQueue(bool queue)
        {
            var realmList = GetRealmData(string.Format(baseRealmAPIurl, Region, string.Empty));
            realmList.FilterByQueue(queue);
            return realmList;
        }

        public RealmList GetMultipleRealms(params string[] names)
        {
            if (names == null
                || names.Length == 0
                || names.Any(r => r == null))
            {
                return new RealmList();
            }

            var query = "?realms=" + String.Join(",", names);

            return GetMultipleRealmsViaQuery(query);
        }

        public RealmList GetMultipleRealmsViaQuery(string query)
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
            return GetJson(string.Format(baseRealmAPIurl, Region, string.Empty));
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
            return GetJson(string.Format(baseRealmAPIurl, Region, query));
        }

        private string ConvertRealmListToJson(RealmList realmList)
        {
            return Serializer.Serialize(realmList);
        }

        public RealmList GetRealmData(string url)
        {
            var sanitizedUrl = this.SanitizeUrl(url);
            var json = this.GetJson(sanitizedUrl);
            return this.Serializer.Deserialize<RealmList>(json);
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
