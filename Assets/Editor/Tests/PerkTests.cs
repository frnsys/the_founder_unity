using UnityEngine;
using UnityEditor;
using System;
using System.Threading;
using NUnit.Framework;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace UnityTest
{
	[TestFixture]
	internal class PerkTests
	{
        private GameObject gameObj;
        private GameData gd;
        private GameManager gm;

        private APerk p = null;
        private Technology t = null;
        private Perk.Upgrade u1;
        private Perk.Upgrade u2;
        private Company c;

        [SetUp]
        public void SetUp() {
            gameObj = new GameObject("Game Manager");
            gm = gameObj.AddComponent<GameManager>();
            gd = GameData.New("DEFAULTCORP");
            gm.Load(gd);

            c = gd.company;
            c.office = Office.Type.Apartment;

            Perk perk = (Perk)GameObject.Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Editor/Tests/Resources/TestPerk.asset", typeof(Perk)));
            p = new APerk(perk);

            t = ScriptableObject.CreateInstance<Technology>();

            u1 = new Perk.Upgrade();
            u1.cost = 1000;
            u1.requiredOffice = Office.Type.Apartment;

            u2 = new Perk.Upgrade();
            u2.cost = 2000;
            u2.requiredOffice = Office.Type.Office;
            u2.requiredTechnologies.Add(t);

            p.upgrades.Add(u1);
            p.upgrades.Add(u2);
        }

        [TearDown]
        public void TearDown() {
            UnityEngine.Object.DestroyImmediate(gameObj);
            gm = null;
            p = null;
            c = null;
            t = null;
            u1 = null;
            u2 = null;
        }

		[Test]
		public void Upgrades() {
            Assert.AreEqual(p.upgradeLevel, 0);
            Assert.AreEqual(p.cost, 1000);
            Assert.IsTrue(p.hasNext);
            Assert.AreEqual(p.next, u2);

            // Next upgrade shouldn't be available since company is missing req. tech.
            Assert.IsFalse(p.NextAvailable(c));

            // Should still be unavailable as the company does not have the right office.
            c.technologies.Add(t);
            Assert.IsFalse(p.NextAvailable(c));

            c.office = Office.Type.Office;
            Assert.IsTrue(p.NextAvailable(c));

            p.upgradeLevel = 1;
            Assert.AreEqual(p.cost, 2000);
            Assert.IsFalse(p.hasNext);
		}

    }
}
