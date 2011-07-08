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
	public class RealmExplorerTest
	{
		public IRealmExplorer realmExplorer;
		public JavaScriptSerializer jsSerializer;

		[TestInitialize]
		public void Setup()
		{
			realmExplorer = new RealmExplorer();
			jsSerializer = new JavaScriptSerializer();
		}

		//US - Americas; 241 realms as of 04/22/2011
		[TestMethod]
		public void GetAll_US_Realms_Returns_All_Realms()
		{
			var realmList = realmExplorer.GetAllRealms();
			Assert.IsTrue(realmList.realms.Count() >= 241);
		}

		[TestMethod]
		public void Get_Valid_US_Realm_Returns_Unique_Realm()
		{
			var realm = realmExplorer.GetSingleRealm("skullcrusher");

			Assert.IsTrue(realm.name == "Skullcrusher");
			Assert.IsTrue(realm.type == "pvp");
			Assert.IsTrue(realm.slug == "skullcrusher");
		}

		[TestMethod]
		public void Get_Invalid_US_Realm_Returns_Null()
		{
			var realm = realmExplorer.GetSingleRealm("dekuz");
			Assert.IsNull(realm);
		}

		[TestMethod]
		public void Get_Null_US_Realm_Returns_Null()
		{
			var realm = realmExplorer.GetSingleRealm(null);
			Assert.IsNull(realm);
		}

		[TestMethod]
		public void Get_All_Realms_By_Type_Returns_Pvp_Realms()
		{
			var realmList = realmExplorer.GetRealmsByType("pvp");
			var allCollectedRealmsArePvp = realmList.realms.Any() &&
				realmList.realms.All(r => r.type.Equals("pvp", StringComparison.InvariantCultureIgnoreCase));

			Assert.IsTrue(allCollectedRealmsArePvp);
		}

		[TestMethod]
		public void Get_All_Realms_By_Type_As_Json_Returns_Pvp_Realms_And_Validates_Comparison_With_Pvp_RealmList()
		{
			var realmList = realmExplorer.GetRealmsByType("pvp");
			var realmsJson = realmExplorer.GetRealmsByTypeAsJson("pvp");

			var jsonObjects = (Dictionary<string, object>)(jsSerializer.DeserializeObject(realmsJson));
			var realmListFromJson = jsSerializer.ConvertToType<IEnumerable<Realm>>(jsonObjects["realms"]);

			var allCollectedRealmsArePvp = realmListFromJson.Any() &&
				realmListFromJson.All(r => r.type.Equals("pvp", StringComparison.InvariantCultureIgnoreCase));

			Assert.IsTrue(allCollectedRealmsArePvp);
			Assert.IsTrue(Enumerable.SequenceEqual(realmList.realms, realmListFromJson, new RealmComparer()));
		}

		[TestMethod]
		public void Get_All_Realms_By_Type_As_Json_Returns_Pve_Realms_And_Invalidates_Comparison_Againts_Pvp_RealmList()
		{
			var realmList = realmExplorer.GetRealmsByType("pvp");
			var realmsJson = realmExplorer.GetRealmsByTypeAsJson("pve");

			var jsonObjects = (Dictionary<string, object>)(jsSerializer.DeserializeObject(realmsJson));
			var realmListFromJson = jsSerializer.ConvertToType<IEnumerable<Realm>>(jsonObjects["realms"]);

			var allCollectedRealmsArePve = realmListFromJson
				.All(r => r.type.Equals("pve", StringComparison.InvariantCultureIgnoreCase));

			Assert.IsTrue(allCollectedRealmsArePve);
			Assert.IsFalse(Enumerable.SequenceEqual(realmList.realms, realmListFromJson, new RealmComparer()));
		}

		[TestMethod]
		public void Get_All_Realms_By_Status_Returns_Realms_That_Are_Up()
		{
			var realmList = realmExplorer.GetRealmsByStatus(true);

			//All servers being down is likely(maintenance) and will cause test to fail
			var allCollectedRealmsAreUp = realmList.realms.Any() &&
				realmList.realms.All(r => r.status == true);

			Assert.IsTrue(allCollectedRealmsAreUp);
		}

		[TestMethod]
		public void Get_All_Realms_By_Status_As_Json_Returns_Online_Realms_And_Validates_Comparison_With_Online_Only_RealmList()
		{
			var realmList = realmExplorer.GetRealmsByStatus(true);
			var realmsJson = realmExplorer.GetRealmsByStatusAsJson(true);

			var jsonObjects = (Dictionary<string, object>)(jsSerializer.DeserializeObject(realmsJson));
			var realmListFromJson = jsSerializer.ConvertToType<IEnumerable<Realm>>(jsonObjects["realms"]);

			//All servers being down is likely(maintenance) and will cause test to fail
			var allCollectedRealmsAreOnline = realmListFromJson.Any() &&
				realmListFromJson.All(r => r.status == true);

			Assert.IsTrue(allCollectedRealmsAreOnline);
			Assert.IsTrue(Enumerable.SequenceEqual(realmList.realms, realmListFromJson, new RealmComparer()));
		}

		[TestMethod]
		public void Get_All_Realms_By_Queue_Returns_Realms_That_Do_Not_Have_Queues()
		{
			var realmList = realmExplorer.GetRealmsByQueue(false);

			//All servers getting queues is unlikely but possible and will cause test to fail
			var allCollectedRealmsHaveQueues = realmList.realms.Any() &&
				realmList.realms.All(r => r.queue == false);

			Assert.IsTrue(allCollectedRealmsHaveQueues);
		}

		[TestMethod]
		public void Get_All_Realms_By_Queue_As_Json_Returns_Realms_That_Do_Not_Have_Queues_And_Validates_Comparison_With_Queue_Only_RealmList()
		{
			var realmList = realmExplorer.GetRealmsByQueue(false);
			var realmsJson = realmExplorer.GetRealmsByQueueAsJson(false);
			
			var jsonObjects = (Dictionary<string, object>)(jsSerializer.DeserializeObject(realmsJson));
			var realmListFromJson = jsSerializer.ConvertToType<IEnumerable<Realm>>(jsonObjects["realms"]);

			//All servers getting queues is unlikely but possible and will cause test to fail
			var allCollectedRealmsDoNotHaveQueues = realmListFromJson.Any() &&
				realmListFromJson.All(r => r.queue == false);

			Assert.IsTrue(allCollectedRealmsDoNotHaveQueues);
			Assert.IsTrue(Enumerable.SequenceEqual(realmList.realms, realmListFromJson, new RealmComparer()));
		}

		[TestMethod]
		public void Get_All_Realms_By_Population_Returns_Realms_That_Have_Low_Population()
		{
			var realmList = realmExplorer.GetRealmsByPopulation("low");
			var allCollectedRealmsHaveLowPopulation = realmList.realms.Any() &&
				realmList.realms.All(r => r.population.Equals("low", StringComparison.InvariantCultureIgnoreCase));

			Assert.IsTrue(allCollectedRealmsHaveLowPopulation);
		}

		[TestMethod]
		public void Get_All_Realms_By_Population_As_Json_Returns_MedPop_Realms_And_Validates_Comparison_With_MedPop_Only_RealmList()
		{
			var realmList = realmExplorer.GetRealmsByPopulation("medium");
			var realmsJson = realmExplorer.GetRealmsByPopulationAsJson("medium");

			var jsonObjects = (Dictionary<string, object>)(jsSerializer.DeserializeObject(realmsJson));
			var realmListFromJson = jsSerializer.ConvertToType<IEnumerable<Realm>>(jsonObjects["realms"]);

			//All servers getting queues is unlikely but possible and will cause test to fail
			var allCollectedRealmsHaveMedPopulation = realmListFromJson.Any() &&
				realmListFromJson.All(r => r.population.Equals("medium", StringComparison.InvariantCultureIgnoreCase));

			Assert.IsTrue(allCollectedRealmsHaveMedPopulation);
			Assert.IsTrue(Enumerable.SequenceEqual(realmList.realms, realmListFromJson, new RealmComparer()));
		}

		[TestMethod]
		public void Get_Realms_Using_Multiple_ValidNames_Query_Returns_Valid_Results()
		{
			var realmList = realmExplorer.GetMultipleRealms("Skullcrusher", "Laughing Skull", "Blade's Edge");

			var allCollectedRealmsAreValid = realmList.realms.Any() &&
				realmList.realms.All(r => r.name.Equals("Skullcrusher", StringComparison.InvariantCultureIgnoreCase)
					|| r.name.Equals("Laughing Skull", StringComparison.InvariantCultureIgnoreCase)
					|| r.name.Equals("Blade's Edge", StringComparison.InvariantCultureIgnoreCase));

			Assert.IsTrue(realmList.realms.Count() == 3);
			Assert.IsTrue(allCollectedRealmsAreValid);
		}

		[TestMethod]
		public void Get_Realms_Using_Multiple_InvalidNames_Query_Returns_One_Valid_Result()
		{
			var realmList = realmExplorer.GetMultipleRealms("Blade's Edge", "AZUZU!", "dekuz");

			var allCollectedRealmsAreValid = realmList.realms.Any() &&
				realmList.realms.All(r => r.name.Equals("Blade's Edge", StringComparison.InvariantCultureIgnoreCase)
					|| r.name.Equals("AZUZU!", StringComparison.InvariantCultureIgnoreCase)
					|| r.name.Equals("dekuz", StringComparison.InvariantCultureIgnoreCase));

			Assert.IsTrue(realmList.realms.Count() == 1);
			Assert.IsTrue(allCollectedRealmsAreValid);
		}

		[TestMethod]
		public void Get_Realms_Using_Multiple_ValidNamesArray_Query_Returns_Valid_Results()
		{
			var namesList = new string[] { "Blade's Edge", "Aegwynn", "Area 52" };

			var realmList = realmExplorer.GetMultipleRealms(namesList);

			var allCollectedRealmsAreValid = realmList.realms.Any() &&
				realmList.realms.All(r => r.name.Equals("Blade's Edge", StringComparison.InvariantCultureIgnoreCase)
					|| r.name.Equals("Aegwynn", StringComparison.InvariantCultureIgnoreCase)
					|| r.name.Equals("Area 52", StringComparison.InvariantCultureIgnoreCase));

			Assert.IsTrue(realmList.realms.Count() == 3);
			Assert.IsTrue(allCollectedRealmsAreValid);
		}

		[TestMethod]
		public void Get_Realms_Using_Multiple_InvalidNamesArray_Query_Returns_Null()
		{
			var namesList = new string[] { "Aioros", "Aioria", "Shiryu" };
			var realmList = realmExplorer.GetMultipleRealms(namesList);

			Assert.IsNull(realmList);
		}

		[TestMethod]
		public void Get_Realms_Using_Valid_Query_Returns_Valid_Results()
		{
			var realmList = realmExplorer.GetMultipleRealmsViaQuery("?realm=Medivh&realm=Blackrock");

			var allCollectedRealmsAreValid = realmList.realms.Any() &&
				realmList.realms.All(r => r.name.Equals("Medivh", StringComparison.InvariantCultureIgnoreCase)
					|| r.name.Equals("Blackrock", StringComparison.InvariantCultureIgnoreCase));


			Assert.IsTrue(realmList.realms.Count() == 2);
			Assert.IsTrue(allCollectedRealmsAreValid);
		}

		[TestMethod]
		public void Get_Realms_Using_Valid_Query_As_Json_Returns_Valid_Results()
		{
			var realmList = realmExplorer.GetMultipleRealmsViaQuery("?realm=Medivh&realm=Blackrock");
			var realmsJson = realmExplorer.GetRealmsViaQueryAsJson("?realm=Medivh&realm=Blackrock");

			var jsonObjects = (Dictionary<string, object>)(jsSerializer.DeserializeObject(realmsJson));
			var realmListFromJson = jsSerializer.ConvertToType<IEnumerable<Realm>>(jsonObjects["realms"]);

			var allCollectedRealmsAreValid = realmListFromJson.Any() &&
				realmListFromJson.All(r => r.name.Equals("Medivh", StringComparison.InvariantCultureIgnoreCase)
					|| r.name.Equals("Blackrock", StringComparison.InvariantCultureIgnoreCase));

			Assert.IsTrue(realmListFromJson.Count() == 2);
			Assert.IsTrue(allCollectedRealmsAreValid);
			Assert.IsTrue(Enumerable.SequenceEqual(realmList.realms, realmListFromJson, new RealmComparer()));
		}

		[TestMethod]
		public void Get_Realms_Using_Sending_Null_String_List_Returns_Empty_Lists()
		{
			string nullRealmNameString = null;
			string[] nullRealmNameStringArray = null;

			var realmList = realmExplorer.GetMultipleRealms(nullRealmNameString);
			Assert.IsTrue(realmList.realms.Count() == 0);

			realmList = realmExplorer.GetMultipleRealms(nullRealmNameStringArray);
			Assert.IsTrue(realmList.realms.Count() == 0);
		}

		[TestMethod]
		public void Get_Realms_Using_Garbage_Query_Still_Returns_All_Realms()
		{
			var realmList = realmExplorer.GetMultipleRealmsViaQuery("?asdf2&asdf");

			Assert.IsNotNull(realmList);
			Assert.IsTrue(realmList.realms.Count() >= 241);
		}

		//EU- Europe; 265 realms as of 04/22/2011
		[TestMethod]
		public void GetAll_EU_Realms_Returns_All_EU_Realms()
		{
			realmExplorer.Region = "eu";
			var realmList = realmExplorer.GetAllRealms();
			Assert.IsTrue(realmList.realms.Count() >= 265);
		}

		[TestMethod]
		public void Get_Valid_EU_Realm_Returns_Unique_EU_Realm()
		{
			realmExplorer.Region = "eu";
			var realm = realmExplorer.GetSingleRealm("drek'thar");

			Assert.IsTrue(realm.name == "Drek'Thar");
			Assert.IsTrue(realm.type == "pve");
			Assert.IsTrue(realm.slug == "drekthar");
		}

		[TestMethod]
		public void Get_Invalid_EU_Realm_Returns_Null()
		{
			realmExplorer.Region = "eu";
			var realm = realmExplorer.GetSingleRealm("dekuz");
			Assert.IsNull(realm);
		}

		//KR - Korea; 33 realms as of 04/22/2011
		[TestMethod]
		public void GetAll_KR_Realms_Returns_All_KR_Realms()
		{
			realmExplorer.Region = "kr";
			var realmList = realmExplorer.GetAllRealms();
			Assert.IsTrue(realmList.realms.Count() >= 33);
		}

		[TestMethod]
		public void Get_Valid_KR_Realm_Returns_Unique_KR_Realm()
		{
			realmExplorer.Region = "kr";
			var realm = realmExplorer.GetSingleRealm("kul tiras");

			Assert.IsTrue(realm.name == "Kul Tiras");
			Assert.IsTrue(realm.type == "pvp");
			Assert.IsTrue(realm.slug == "kul-tiras");
		}

		[TestMethod]
		public void Get_Invalid_KR_Realm_Returns_Null()
		{
			realmExplorer.Region = "kr";
			var realm = realmExplorer.GetSingleRealm("dekuz");
			Assert.IsNull(realm);
		}

		[TestMethod]
		public void GetAllRealms_InvalidRegion_URL_Throws_WebException()
		{
			realmExplorer.Region = "foo";
			ThrowsException<WebException>(() => realmExplorer.GetAllRealms(), "The remote name could not be resolved: 'foo.battle.net'");
		}

		//tw - Taiwan
		//sea - Southeast Asia
		//china - China

		//Assert.ThrowException 
		public static void ThrowsException<T>(Action action, string expectedMessage) where T : Exception
		{
			try
			{
				action.Invoke();
				Assert.Fail("Exception of type {0} should be thrown", typeof(T));
			}
			catch (T e)
			{
				Assert.AreEqual(expectedMessage, e.Message);
			}
		}
	}
}
