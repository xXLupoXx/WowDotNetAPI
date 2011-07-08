using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WowDotNetAPI.Explorers.Models;

namespace Explorers.Test
{
    //US - Americas; 241 realms as of 04/22/2011
    [TestClass]
    public class USRealmExplorerTest : RealmExplorerTest
    {
        public USRealmExplorerTest() : base("us", 241, "Skullcrusher", "Laughing Skull", "Blade's Edge") { }

        [TestMethod]
        public void Get_Valid_US_Realm_Returns_Unique_Realm()
        {
            var realm = this.RealmExplorer.GetSingleRealm("skullcrusher");

            Assert.IsTrue(realm.name == "Skullcrusher");
            Assert.IsTrue(realm.type == "pvp");
            Assert.IsTrue(realm.slug == "skullcrusher");
        }

        [TestMethod]
        public void Get_Realms_Using_Valid_Query_As_Json_Returns_Valid_Results()
        {
            var realmList = this.RealmExplorer.GetMultipleRealmsViaQuery("?realm=Medivh&realm=Blackrock");
            var realmsJson = this.RealmExplorer.GetRealmsViaQueryAsJson("?realm=Medivh&realm=Blackrock");

            var jsonObjects = (Dictionary<string, object>)(this.JsSerializer.DeserializeObject(realmsJson));
            var realmListFromJson = this.JsSerializer.ConvertToType<IEnumerable<Realm>>(jsonObjects["realms"]);

            var allCollectedRealmsAreValid = realmListFromJson.Any() &&
                realmListFromJson.All(r => r.name.Equals("Medivh", StringComparison.InvariantCultureIgnoreCase)
                    || r.name.Equals("Blackrock", StringComparison.InvariantCultureIgnoreCase));

            Assert.IsTrue(realmListFromJson.Count() == 2);
            Assert.IsTrue(allCollectedRealmsAreValid);
            Assert.IsTrue(Enumerable.SequenceEqual(realmList, realmListFromJson, this.RealmComparer));
        }

        [TestMethod]
        public void Get_Realms_Using_Valid_Query_Returns_Valid_Results()
        {
            var realmList = this.RealmExplorer.GetMultipleRealmsViaQuery("?realm=Medivh&realm=Blackrock");

            var allCollectedRealmsAreValid = realmList.Any() &&
                realmList.All(r => r.name.Equals("Medivh", StringComparison.InvariantCultureIgnoreCase)
                    || r.name.Equals("Blackrock", StringComparison.InvariantCultureIgnoreCase));


            Assert.IsTrue(realmList.Count() == 2);
            Assert.IsTrue(allCollectedRealmsAreValid);
        }
    }
}
