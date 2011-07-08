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
        private readonly int expectedRealmCount;
        private readonly string[] expectedRealmNames;
        private readonly JavaScriptSerializer serializer;
        private readonly IRealmExplorer realmExplorer;
        private readonly string region;

        public RealmExplorerTest(string region, int expectedRealmCount, params string[] expectedRealmNames)
        {
            expectedRealmCount = expectedRealmCount;
            expectedRealmNames = expectedRealmNames;
            region = region;
            var jsonSource = new WowDotNetAPI.JsonSource();
            realmExplorer = new RealmExplorer(region, jsonSource);
            serializer = new JavaScriptSerializer();

            this.RealmComparer = new RealmComparer();
            this.RealmNameComparer = new RealmNameComparer();
        }

        protected JavaScriptSerializer JsSerializer { get { return serializer; } }
        protected IRealmExplorer RealmExplorer { get { return realmExplorer; } }

        public IEqualityComparer<Realm> RealmComparer { get; set; }
        public IEqualityComparer<string> RealmNameComparer { get; set; }

        [TestMethod]
        public void Get_Null_Realm_Returns_Null()
        {
            var realm = realmExplorer.GetSingleRealm(null);
            Assert.IsNull(realm);
        }

        [TestMethod]
        public void Get_All_Realms_By_Type_Returns_Pvp_Realms()
        {
            var realmList = realmExplorer.GetRealmsByType("pvp");
            var allCollectedRealmsArePvp = realmList.Any() &&
                realmList.All(r => r.type.Equals("pvp", StringComparison.InvariantCultureIgnoreCase));

            Assert.IsTrue(allCollectedRealmsArePvp);
        }

        [TestMethod]
        public void Get_All_Realms_By_Type_As_Json_Returns_Pvp_Realms_And_Validates_Comparison_With_Pvp_RealmList()
        {
            IEnumerable<Realm> realmList = realmExplorer.GetRealmsByType("pvp");
            var realmsJson = realmExplorer.GetRealmsByTypeAsJson("pvp");

            IEnumerable<Realm> realmListFromJson = serializer.Deserialize<List<Realm>>(realmsJson);

            var allCollectedRealmsArePvp = realmListFromJson.Any() &&
                realmListFromJson.All(r => r.type.Equals("pvp", StringComparison.InvariantCultureIgnoreCase));

            Assert.IsTrue(allCollectedRealmsArePvp);
            Assert.IsTrue(Enumerable.SequenceEqual(realmList, realmListFromJson, this.RealmComparer));
        }

        [TestMethod]
        public void Get_All_Realms_By_Type_As_Json_Returns_Pve_Realms_And_Invalidates_Comparison_Againts_Pvp_RealmList()
        {
            var realmList = realmExplorer.GetRealmsByType("pvp");
            var realmsJson = realmExplorer.GetRealmsByTypeAsJson("pve");

            IEnumerable<Realm> realmListFromJson = serializer.Deserialize<List<Realm>>(realmsJson);

            var allCollectedRealmsArePve = realmListFromJson
                .All(r => r.type.Equals("pve", StringComparison.InvariantCultureIgnoreCase));

            Assert.IsTrue(allCollectedRealmsArePve);
            Assert.IsFalse(Enumerable.SequenceEqual(realmList, realmListFromJson, this.RealmComparer));
        }

        [TestMethod]
        public void Get_All_Realms_By_Status_Returns_Realms_That_Are_Up()
        {
            var realmList = realmExplorer.GetRealmsByStatus(true);

            //All servers being down is likely(maintenance) and will cause test to fail
            var allCollectedRealmsAreUp = realmList.Any() &&
                realmList.All(r => r.status == true);

            Assert.IsTrue(allCollectedRealmsAreUp);
        }

        [TestMethod]
        public void Get_All_Realms_By_Status_As_Json_Returns_Online_Realms_And_Validates_Comparison_With_Online_Only_RealmList()
        {
            var realmList = realmExplorer.GetRealmsByStatus(true);
            var realmsJson = realmExplorer.GetRealmsByStatusAsJson(true);

            IEnumerable<Realm> realmListFromJson = serializer.Deserialize<List<Realm>>(realmsJson);

            //All servers being down is likely(maintenance) and will cause test to fail
            var allCollectedRealmsAreOnline = realmListFromJson.Any() &&
                realmListFromJson.All(r => r.status == true);

            Assert.IsTrue(allCollectedRealmsAreOnline);
            Assert.IsTrue(Enumerable.SequenceEqual(realmList, realmListFromJson, this.RealmComparer));
        }

        [TestMethod]
        public void Get_All_Realms_By_Queue_Returns_Realms_That_Do_Not_Have_Queues()
        {
            var realmList = realmExplorer.GetRealmsByQueue(false);

            //All servers getting queues is unlikely but possible and will cause test to fail
            var allCollectedRealmsHaveQueues = realmList.Any() &&
                realmList.All(r => r.queue == false);

            Assert.IsTrue(allCollectedRealmsHaveQueues);
        }

        [TestMethod]
        public void Get_All_Realms_By_Queue_As_Json_Returns_Realms_That_Do_Not_Have_Queues_And_Validates_Comparison_With_Queue_Only_RealmList()
        {
            var realmList = realmExplorer.GetRealmsByQueue(false);
            var realmsJson = realmExplorer.GetRealmsByQueueAsJson(false);

            var realmListFromJson = serializer.Deserialize<List<Realm>>(realmsJson);

            //All servers getting queues is unlikely but possible and will cause test to fail
            var allCollectedRealmsDoNotHaveQueues = realmListFromJson.Any() &&
                realmListFromJson.All(r => r.queue == false);

            Assert.IsTrue(allCollectedRealmsDoNotHaveQueues);
            Assert.IsTrue(Enumerable.SequenceEqual(realmList, realmListFromJson, this.RealmComparer));
        }

        [TestMethod]
        public void Get_All_Realms_By_Population_Returns_Realms_That_Have_Low_Population()
        {
            var realmList = realmExplorer.GetRealmsByPopulation("low");
            var allCollectedRealmsHaveLowPopulation = realmList.Any() &&
                realmList.All(r => r.population.Equals("low", StringComparison.InvariantCultureIgnoreCase));

            Assert.IsTrue(allCollectedRealmsHaveLowPopulation);
        }

        [TestMethod]
        public void Get_All_Realms_By_Population_As_Json_Returns_MedPop_Realms_And_Validates_Comparison_With_MedPop_Only_RealmList()
        {
            var realmList = realmExplorer.GetRealmsByPopulation("medium");
            var realmsJson = realmExplorer.GetRealmsByPopulationAsJson("medium");

            IEnumerable<Realm> realmListFromJson = serializer.Deserialize<IEnumerable<Realm>>(realmsJson);

            //All servers getting queues is unlikely but possible and will cause test to fail
            var allCollectedRealmsHaveMedPopulation = realmListFromJson.Any() &&
                realmListFromJson.All(r => r.population.Equals("medium", StringComparison.InvariantCultureIgnoreCase));

            Assert.IsTrue(allCollectedRealmsHaveMedPopulation);
            Assert.IsTrue(Enumerable.SequenceEqual(realmList, realmListFromJson, this.RealmComparer));
        }

        [TestMethod]
        public void Get_Realms_Using_Multiple_ValidNames_Query_Returns_Valid_Results()
        {
            var realmList = realmExplorer.GetMultipleRealms(expectedRealmNames);

            var allCollectedRealmsAreValid = realmList.Any() &&
                realmList.All(r => expectedRealmNames.Contains(r.name, this.RealmNameComparer));

            Assert.IsTrue(realmList.Count() == expectedRealmNames.Count());
            Assert.IsTrue(allCollectedRealmsAreValid);
        }

        [TestMethod]
        public void Get_Realms_Using_Multiple_InvalidNames_Query_Returns_One_Valid_Result()
        {
            var correctRealm = expectedRealmNames.First();

            var realmList = realmExplorer.GetMultipleRealms(correctRealm, "AZUZU!", "dekuz");

            var allCollectedRealmsAreValid = realmList.Any() &&
                realmList.All(r => expectedRealmNames.Contains(r.name, this.RealmNameComparer));

            Assert.IsTrue(realmList.Count() == 1);
            Assert.IsTrue(allCollectedRealmsAreValid);
        }

        [TestMethod]
        public void Get_Realms_Using_Multiple_ValidNamesArray_Query_Returns_Valid_Results()
        {
            var realmList = realmExplorer.GetMultipleRealms(expectedRealmNames);

            var allCollectedRealmsAreValid = realmList.Any() &&
                realmList.All(r => expectedRealmNames.Contains(r.name, this.RealmNameComparer));

            Assert.IsTrue(realmList.Count() == expectedRealmNames.Count());
            Assert.IsTrue(allCollectedRealmsAreValid);
        }

        [TestMethod]
        public void Get_Realms_Using_Sending_Null_String_List_Returns_Empty_Lists()
        {
            string nullRealmNameString = null;
            string[] nullRealmNameStringArray = null;

            var realmList = realmExplorer.GetMultipleRealms(nullRealmNameString);
            Assert.IsTrue(realmList.Count() == 0);

            realmList = realmExplorer.GetMultipleRealms(nullRealmNameStringArray);
            Assert.IsTrue(realmList.Count() == 0);
        }

        [TestMethod]
        public void Get_Realms_Using_Garbage_Query_Still_Returns_All_Realms()
        {
            var realmList = realmExplorer.GetMultipleRealmsViaQuery("?asdf2&asdf");

            Assert.IsNotNull(realmList);
            Assert.IsTrue(realmList.Count() >= expectedRealmCount);
        }

        [TestMethod]
        public void GetAll_Realms_Returns_All_Realms()
        {
            var realmList = realmExplorer.GetAllRealms();
            Assert.IsTrue(realmList.Count() >= expectedRealmCount);
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
