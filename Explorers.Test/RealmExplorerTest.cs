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
using WowDotNetAPI.Explorers;
using WowDotNetAPI;

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
			this.expectedRealmCount = expectedRealmCount;
			this.expectedRealmNames = expectedRealmNames;
			this.region = region;
			this.realmExplorer = new RealmExplorer(region, new WowDotNetAPI.JsonSource());
			this.serializer = new JavaScriptSerializer();

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
			Realm realm = realmExplorer.GetSingleRealm(null);
			Assert.IsNull(realm);
		}

		[TestMethod]
		public void Get_All_Realms_By_Type_Returns_Pvp_Realms()
		{
			RealmList realmList = realmExplorer.GetRealmsByType("pvp");
			bool allCollectedRealmsArePvp = realmList.realms.Any() &&
				realmList.realms.All(r => r.type.Equals("pvp", StringComparison.InvariantCultureIgnoreCase));

			Assert.IsTrue(allCollectedRealmsArePvp);
		}

		[TestMethod]
		public void Get_All_Realms_By_Type_As_Json_Returns_Pvp_Realms_And_Validates_Comparison_With_Pvp_RealmList()
		{
			RealmList realmList = realmExplorer.GetRealmsByType("pvp");
			string realmsJson = realmExplorer.GetRealmsByTypeAsJson("pvp");

			RealmList realmListFromJson = serializer.Deserialize<RealmList>(realmsJson);

			bool allCollectedRealmsArePvp = realmListFromJson.realms.Any() &&
				realmListFromJson.realms.All(r => r.type.Equals("pvp", StringComparison.InvariantCultureIgnoreCase));

			Assert.IsTrue(allCollectedRealmsArePvp);
			Assert.IsTrue(Enumerable.SequenceEqual(realmList.realms, realmListFromJson.realms, this.RealmComparer));
		}

		[TestMethod]
		public void Get_All_Realms_By_Type_As_Json_Returns_Pve_Realms_And_Invalidates_Comparison_Againts_Pvp_RealmList()
		{
			RealmList realmList = realmExplorer.GetRealmsByType("pvp");
			string realmsJson = realmExplorer.GetRealmsByTypeAsJson("pve");

			RealmList realmListFromJson = serializer.Deserialize<RealmList>(realmsJson);

			bool allCollectedRealmsArePve = realmListFromJson.realms
				.All(r => r.type.Equals("pve", StringComparison.InvariantCultureIgnoreCase));

			Assert.IsTrue(allCollectedRealmsArePve);
			Assert.IsFalse(Enumerable.SequenceEqual(realmList.realms, realmListFromJson.realms, this.RealmComparer));
		}

		[TestMethod]
		public void Get_All_Realms_By_Status_Returns_Realms_That_Are_Up()
		{
			RealmList realmList = realmExplorer.GetRealmsByStatus(true);

			//All servers being down is likely(maintenance) and will cause test to fail
			bool allCollectedRealmsAreUp = realmList.realms.Any() &&
				realmList.realms.All(r => r.status == true);

			Assert.IsTrue(allCollectedRealmsAreUp);
		}

		[TestMethod]
		public void Get_All_Realms_By_Status_As_Json_Returns_Online_Realms_And_Validates_Comparison_With_Online_Only_RealmList()
		{
			RealmList realmList = realmExplorer.GetRealmsByStatus(true);
			string realmsJson = realmExplorer.GetRealmsByStatusAsJson(true);

			RealmList realmListFromJson = serializer.Deserialize<RealmList>(realmsJson);

			//All servers being down is likely(maintenance) and will cause test to fail
			bool allCollectedRealmsAreOnline = realmListFromJson.realms.Any() &&
				realmListFromJson.realms.All(r => r.status == true);

			Assert.IsTrue(allCollectedRealmsAreOnline);
			Assert.IsTrue(Enumerable.SequenceEqual(realmList.realms, realmListFromJson.realms, this.RealmComparer));
		}

		[TestMethod]
		public void Get_All_Realms_By_Queue_Returns_Realms_That_Do_Not_Have_Queues()
		{
			RealmList realmList = realmExplorer.GetRealmsByQueue(false);

			//All servers getting queues is unlikely but possible and will cause test to fail
			bool allCollectedRealmsHaveQueues = realmList.realms.Any() &&
				realmList.realms.All(r => r.queue == false);

			Assert.IsTrue(allCollectedRealmsHaveQueues);
		}

		[TestMethod]
		public void Get_All_Realms_By_Queue_As_Json_Returns_Realms_That_Do_Not_Have_Queues_And_Validates_Comparison_With_Queue_Only_RealmList()
		{
			RealmList realmList = realmExplorer.GetRealmsByQueue(false);
			string realmsJson = realmExplorer.GetRealmsByQueueAsJson(false);

			RealmList realmListFromJson = serializer.Deserialize<RealmList>(realmsJson);

			//All servers getting queues is unlikely but possible and will cause test to fail
			bool allCollectedRealmsDoNotHaveQueues = realmListFromJson.realms.Any() &&
				realmListFromJson.realms.All(r => r.queue == false);

			Assert.IsTrue(allCollectedRealmsDoNotHaveQueues);
			Assert.IsTrue(Enumerable.SequenceEqual(realmList.realms, realmListFromJson.realms, this.RealmComparer));
		}

		[TestMethod]
		public void Get_All_Realms_By_Population_Returns_Realms_That_Have_Low_Population()
		{
			RealmList realmList = realmExplorer.GetRealmsByPopulation("low");
			bool allCollectedRealmsHaveLowPopulation = realmList.realms.Any() &&
				realmList.realms.All(r => r.population.Equals("low", StringComparison.InvariantCultureIgnoreCase));

			Assert.IsTrue(allCollectedRealmsHaveLowPopulation);
		}

		[TestMethod]
		public void Get_All_Realms_By_Population_As_Json_Returns_MedPop_Realms_And_Validates_Comparison_With_MedPop_Only_RealmList()
		{
			RealmList realmList = realmExplorer.GetRealmsByPopulation("medium");
			string realmsJson = realmExplorer.GetRealmsByPopulationAsJson("medium");

			RealmList realmListFromJson = serializer.Deserialize<RealmList>(realmsJson);

			//All servers getting queues is unlikely but possible and will cause test to fail
			bool allCollectedRealmsHaveMedPopulation = realmListFromJson.realms.Any() &&
				realmListFromJson.realms.All(r => r.population.Equals("medium", StringComparison.InvariantCultureIgnoreCase));

			Assert.IsTrue(allCollectedRealmsHaveMedPopulation);
			Assert.IsTrue(Enumerable.SequenceEqual(realmList.realms, realmListFromJson.realms, this.RealmComparer));
		}

		[TestMethod]
		public void Get_Realms_Using_Multiple_ValidNames_Query_Returns_Valid_Results()
		{
			RealmList realmList = realmExplorer.GetMultipleRealms(expectedRealmNames);

			bool allCollectedRealmsAreValid = realmList.realms.Any() &&
				realmList.realms.All(r => expectedRealmNames.Contains(r.name, this.RealmNameComparer));

			Assert.IsTrue(realmList.realms.Count() == expectedRealmNames.Count());
			Assert.IsTrue(allCollectedRealmsAreValid);
		}

		[TestMethod]
		public void Get_Realms_Using_Multiple_InvalidNames_Query_Returns_One_Valid_Result()
		{
			string correctRealm = expectedRealmNames.First();

			RealmList realmList = realmExplorer.GetMultipleRealms(correctRealm, "AZUZU!", "dekuz");

			bool allCollectedRealmsAreValid = realmList.realms.Any() &&
				realmList.realms.All(r => expectedRealmNames.Contains(r.name, this.RealmNameComparer));

			Assert.IsTrue(realmList.realms.Count() == 1);
			Assert.IsTrue(allCollectedRealmsAreValid);
		}

		[TestMethod]
		public void Get_Realms_Using_Multiple_ValidNamesArray_Query_Returns_Valid_Results()
		{
			RealmList realmList = realmExplorer.GetMultipleRealms(expectedRealmNames);

			bool allCollectedRealmsAreValid = realmList.realms.Any() &&
				realmList.realms.All(r => expectedRealmNames.Contains(r.name, this.RealmNameComparer));

			Assert.IsTrue(realmList.realms.Count() == expectedRealmNames.Count());
			Assert.IsTrue(allCollectedRealmsAreValid);
		}

		[TestMethod]
		public void Get_Realms_Using_Sending_Null_String_List_Returns_Empty_Lists()
		{
			string nullRealmNameString = null;
			string[] nullRealmNameStringArray = null;

			RealmList realmList = realmExplorer.GetMultipleRealms(nullRealmNameString);
			Assert.IsTrue(realmList.realms.Count() == 0);

			realmList = realmExplorer.GetMultipleRealms(nullRealmNameStringArray);
			Assert.IsTrue(realmList.realms.Count() == 0);
		}

		[TestMethod]
		public void Get_Realms_Using_Garbage_Query_Still_Returns_All_Realms()
		{
			RealmList realmList = realmExplorer.GetMultipleRealmsViaQuery("?asdf2&asdf");

			Assert.IsNotNull(realmList);
			Assert.IsTrue(realmList.realms.Count() >= expectedRealmCount);
		}

		[TestMethod]
		public void GetAll_Realms_Returns_All_Realms()
		{
			RealmList realmList = realmExplorer.GetAllRealms();
			Assert.IsTrue(realmList.realms.Count() >= expectedRealmCount);
		}

		[TestMethod, ExpectedException(typeof(WebException), "The remote name could not be resolved: 'foo.battle.net'")]
		public void GetAllRealms_InvalidRegion_URL_Throws_WebException()
		{
			RealmExplorer realmExplorer = new RealmExplorer("foo", new WowDotNetAPI.JsonSource(), new JavaScriptSerializer());
			realmExplorer.GetAllRealms();
		}

		//tw - Taiwan
		//sea - Southeast Asia
		//china - China
	}
}
