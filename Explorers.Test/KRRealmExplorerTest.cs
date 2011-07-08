using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WowDotNetAPI.Explorers.Models;

namespace Explorers.Test
{
    //KR - Korea; 33 realms as of 04/22/2011
    [TestClass]
    public class KRRealmExplorerTest : RealmExplorerTest
    {
        public KRRealmExplorerTest() : base("kr", 33, "Kul Tiras", "Stormrage") { }

        [TestMethod]
        public void Get_Valid_KR_Realm_Returns_Unique_KR_Realm()
        {
            Realm realm = this.RealmExplorer.GetSingleRealm("kul tiras");

            Assert.IsTrue(realm.name == "Kul Tiras");
            Assert.IsTrue(realm.type == "pvp");
            Assert.IsTrue(realm.slug == "kul-tiras");
        }

    }
}
