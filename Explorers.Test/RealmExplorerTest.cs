using System;
using System.Text;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using WowDotNetAPI.Explorers.Models;
using WowDotNetAPI.Explorers.Comparers;
using WowDotNetAPI.Explorers.Interfaces;
using WowDotNetAPI.Explorers.Explorers;

namespace Explorers.Test
{
    [TestClass]
    public abstract class RealmExplorerTest
    {
        private readonly int _ExpectedRealmCount;
        private readonly string[] _ExpectedRealmNames;
        private readonly JavaScriptSerializer _JsSerializer;
        private readonly IRealmExplorer _RealmExplorer;
        private readonly string _Region;

        public RealmExplorerTest(string region, int expectedRealmCount, params string[] expectedRealmNames)
        {
            _ExpectedRealmCount = expectedRealmCount;
            _ExpectedRealmNames = expectedRealmNames;
            _Region = region;
            var jsonSource = new WowDotNetAPI.JsonSource();
            _RealmExplorer = new RealmExplorer(region, jsonSource);
            _JsSerializer = new JavaScriptSerializer();

            this.RealmComparer = new RealmComparer();
            this.RealmNameComparer = new RealmNameComparer();
        }

        protected JavaScriptSerializer JsSerializer { get { return _JsSerializer; } }
        protected IRealmExplorer RealmExplorer { get { return _RealmExplorer; } }

        public IEqualityComparer<Realm> RealmComparer { get; set; }
        public IEqualityComparer<string> RealmNameComparer { get; set; }

        [TestMethod]
        public void Get_Null_Realm_Returns_Null()
        {
            var realm = _RealmExplorer.GetSingleRealm(null);
            Assert.IsNull(realm);
        }

        [TestMethod]
        public void Get_All_Realms_By_Type_Returns_Pvp_Realms()
        {
            var realmList = _RealmExplorer.GetRealmsByType("pvp");
            var allCollectedRealmsArePvp = realmList.Any() &&
                realmList.All(r => r.type.Equals("pvp", StringComparison.InvariantCultureIgnoreCase));

            Assert.IsTrue(allCollectedRealmsArePvp);
        }

        [TestMethod]
        public void Get_All_Realms_By_Type_As_Json_Returns_Pvp_Realms_And_Validates_Comparison_With_Pvp_RealmList()
        {
            IEnumerable<Realm> realmList = _RealmExplorer.GetRealmsByType("pvp");
            var realmsJson = _RealmExplorer.GetRealmsByTypeAsJson("pvp");

            IEnumerable<Realm> realmListFromJson = _JsSerializer.Deserialize<List<Realm>>(realmsJson);

            var allCollectedRealmsArePvp = realmListFromJson.Any() &&
                realmListFromJson.All(r => r.type.Equals("pvp", StringComparison.InvariantCultureIgnoreCase));

            Assert.IsTrue(allCollectedRealmsArePvp);
            Assert.IsTrue(Enumerable.SequenceEqual(realmList, realmListFromJson, this.RealmComparer));
        }

        [TestMethod]
        public void Get_All_Realms_By_Type_As_Json_Returns_Pve_Realms_And_Invalidates_Comparison_Againts_Pvp_RealmList()
        {
            var realmList = _RealmExplorer.GetRealmsByType("pvp");
            var realmsJson = _RealmExplorer.GetRealmsByTypeAsJson("pve");

            IEnumerable<Realm> realmListFromJson = _JsSerializer.Deserialize<List<Realm>>(realmsJson);

            var allCollectedRealmsArePve = realmListFromJson
                .All(r => r.type.Equals("pve", StringComparison.InvariantCultureIgnoreCase));

            Assert.IsTrue(allCollectedRealmsArePve);
            Assert.IsFalse(Enumerable.SequenceEqual(realmList, realmListFromJson, this.RealmComparer));
        }

        [TestMethod]
        public void Get_All_Realms_By_Status_Returns_Realms_That_Are_Up()
        {
            var realmList = _RealmExplorer.GetRealmsByStatus(true);

            //All servers being down is likely(maintenance) and will cause test to fail
            var allCollectedRealmsAreUp = realmList.Any() &&
                realmList.All(r => r.status == true);

            Assert.IsTrue(allCollectedRealmsAreUp);
        }

        [TestMethod]
        public void Get_All_Realms_By_Status_As_Json_Returns_Online_Realms_And_Validates_Comparison_With_Online_Only_RealmList()
        {
            var realmList = _RealmExplorer.GetRealmsByStatus(true);
            var realmsJson = _RealmExplorer.GetRealmsByStatusAsJson(true);

            IEnumerable<Realm> realmListFromJson = _JsSerializer.Deserialize<List<Realm>>(realmsJson);

            //All servers being down is likely(maintenance) and will cause test to fail
            var allCollectedRealmsAreOnline = realmListFromJson.Any() &&
                realmListFromJson.All(r => r.status == true);

            Assert.IsTrue(allCollectedRealmsAreOnline);
            Assert.IsTrue(Enumerable.SequenceEqual(realmList, realmListFromJson, this.RealmComparer));
        }

        [TestMethod]
        public void Get_All_Realms_By_Queue_Returns_Realms_That_Do_Not_Have_Queues()
        {
            var realmList = _RealmExplorer.GetRealmsByQueue(false);

            //All servers getting queues is unlikely but possible and will cause test to fail
            var allCollectedRealmsHaveQueues = realmList.Any() &&
                realmList.All(r => r.queue == false);

            Assert.IsTrue(allCollectedRealmsHaveQueues);
        }

        [TestMethod]
        public void Get_All_Realms_By_Queue_As_Json_Returns_Realms_That_Do_Not_Have_Queues_And_Validates_Comparison_With_Queue_Only_RealmList()
        {
            var realmList = _RealmExplorer.GetRealmsByQueue(false);
            var realmsJson = _RealmExplorer.GetRealmsByQueueAsJson(false);

            var realmListFromJson = _JsSerializer.Deserialize<List<Realm>>(realmsJson);

            //All servers getting queues is unlikely but possible and will cause test to fail
            var allCollectedRealmsDoNotHaveQueues = realmListFromJson.Any() &&
                realmListFromJson.All(r => r.queue == false);

            Assert.IsTrue(allCollectedRealmsDoNotHaveQueues);
            Assert.IsTrue(Enumerable.SequenceEqual(realmList, realmListFromJson, this.RealmComparer));
        }

        [TestMethod]
        public void Get_All_Realms_By_Population_Returns_Realms_That_Have_Low_Population()
        {
            var realmList = _RealmExplorer.GetRealmsByPopulation("low");
            var allCollectedRealmsHaveLowPopulation = realmList.Any() &&
                realmList.All(r => r.population.Equals("low", StringComparison.InvariantCultureIgnoreCase));

            Assert.IsTrue(allCollectedRealmsHaveLowPopulation);
        }

        [TestMethod]
        public void Get_All_Realms_By_Population_As_Json_Returns_MedPop_Realms_And_Validates_Comparison_With_MedPop_Only_RealmList()
        {
            var realmList = _RealmExplorer.GetRealmsByPopulation("medium");
            var realmsJson = _RealmExplorer.GetRealmsByPopulationAsJson("medium");

            IEnumerable<Realm> realmListFromJson = _JsSerializer.Deserialize<IEnumerable<Realm>>(realmsJson);

            //All servers getting queues is unlikely but possible and will cause test to fail
            var allCollectedRealmsHaveMedPopulation = realmListFromJson.Any() &&
                realmListFromJson.All(r => r.population.Equals("medium", StringComparison.InvariantCultureIgnoreCase));

            Assert.IsTrue(allCollectedRealmsHaveMedPopulation);
            Assert.IsTrue(Enumerable.SequenceEqual(realmList, realmListFromJson, this.RealmComparer));
        }

        [TestMethod]
        public void Get_Realms_Using_Multiple_ValidNames_Query_Returns_Valid_Results()
        {
            var realmList = _RealmExplorer.GetMultipleRealms(_ExpectedRealmNames);

            var allCollectedRealmsAreValid = realmList.Any() &&
                realmList.All(r => _ExpectedRealmNames.Contains(r.name, this.RealmNameComparer));

            Assert.IsTrue(realmList.Count() == _ExpectedRealmNames.Count());
            Assert.IsTrue(allCollectedRealmsAreValid);
        }

        [TestMethod]
        public void Get_Realms_Using_Multiple_InvalidNames_Query_Returns_One_Valid_Result()
        {
            var correctRealm = _ExpectedRealmNames.First();

            var realmList = _RealmExplorer.GetMultipleRealms(correctRealm, "AZUZU!", "dekuz");

            var allCollectedRealmsAreValid = realmList.Any() &&
                realmList.All(r => _ExpectedRealmNames.Contains(r.name, this.RealmNameComparer));

            Assert.IsTrue(realmList.Count() == 1);
            Assert.IsTrue(allCollectedRealmsAreValid);
        }

        [TestMethod]
        public void Get_Realms_Using_Multiple_ValidNamesArray_Query_Returns_Valid_Results()
        {
            var realmList = _RealmExplorer.GetMultipleRealms(_ExpectedRealmNames);

            var allCollectedRealmsAreValid = realmList.Any() &&
                realmList.All(r => _ExpectedRealmNames.Contains(r.name, this.RealmNameComparer));

            Assert.IsTrue(realmList.Count() == _ExpectedRealmNames.Count());
            Assert.IsTrue(allCollectedRealmsAreValid);
        }

        [TestMethod]
        public void Get_Realms_Using_Sending_Null_String_List_Returns_Empty_Lists()
        {
            string nullRealmNameString = null;
            string[] nullRealmNameStringArray = null;

            var realmList = _RealmExplorer.GetMultipleRealms(nullRealmNameString);
            Assert.IsTrue(realmList.Count() == 0);

            realmList = _RealmExplorer.GetMultipleRealms(nullRealmNameStringArray);
            Assert.IsTrue(realmList.Count() == 0);
        }

        [TestMethod]
        public void Get_Realms_Using_Garbage_Query_Still_Returns_All_Realms()
        {
            var realmList = _RealmExplorer.GetMultipleRealmsViaQuery("?asdf2&asdf");

            Assert.IsNotNull(realmList);
            Assert.IsTrue(realmList.Count() >= _ExpectedRealmCount);
        }

        [TestMethod]
        public void GetAll_Realms_Returns_All_Realms()
        {
            var realmList = _RealmExplorer.GetAllRealms();
            Assert.IsTrue(realmList.Count() >= _ExpectedRealmCount);
        }

        [TestMethod, ExpectedException(typeof(WebException), "The remote name could not be resolved: 'foo.battle.net'")]
        public void GetAllRealms_InvalidRegion_URL_Throws_WebException()
        {
            var realmExplorer = new RealmExplorer("foo", new WowDotNetAPI.JsonSource(), new JavaScriptSerializer());
            realmExplorer.GetAllRealms();
        }

        //tw - Taiwan
        //sea - Southeast Asia
        //china - China
    }
}
