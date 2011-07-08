using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Explorers.Test
{
    //EU- Europe; 264 realms as of  July 8th 2011
    [TestClass]
    public class EURealmExplorerTest : RealmExplorerTest
    {
        public EURealmExplorerTest() : base("eu", 264, "Aerie Peak", "Forscherliga", "Rajaxx", "Vek'nilash") { }

        [TestMethod]
        public void Get_Valid_EU_Realm_Returns_Unique_EU_Realm()
        {
            var realm = this.RealmExplorer.GetSingleRealm("drek'thar");

            Assert.IsTrue(realm.name == "Drek'Thar");
            Assert.IsTrue(realm.type == "pve");
            Assert.IsTrue(realm.slug == "drekthar");
        }
    }
}
