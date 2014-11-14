using UnityEngine;
using UnityEditor;
using System;
using System.Threading;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

namespace UnityTest
{
    [TestFixture]
    internal class ResearchTests
    {
        private GameObject gameObj;
        private GameManager gm;
        private ResearchManager rm;
        private Discovery disc;

        [SetUp]
        public void SetUp() {
            gameObj = new GameObject("Game Manager");
            gm = gameObj.AddComponent<GameManager>();
            gm.researchManager = gameObj.AddComponent<ResearchManager>();
            gm.Load(GameData.New("DEFAULTCORP"));

            rm = gm.researchManager;
            disc = AssetDatabase.LoadAssetAtPath("Assets/Editor/Tests/Resources/TestDiscovery.asset", typeof(Discovery)) as Discovery;
        }

        [TearDown]
        public void TearDown() {
            UnityEngine.Object.DestroyImmediate(gameObj);
            gm = null;
        }

        [Test]
        public void HireConsultancy() {
            Consultancy con = new Consultancy();

            gm.playerCompany.cash.baseValue = 2000;
            gm.researchManager.HireConsultancy(con);

            Assert.AreEqual(gm.playerCompany.cash.baseValue, 2000 - con.cost);
            Assert.AreEqual(gm.playerCompany.consultancy, con);
            Assert.AreEqual(gm.researchManager.consultancy, con);
        }

        [Test]
        public void Research() {
            Assert.IsFalse(rm.researching);

            rm.BeginResearch(disc);

            Assert.IsTrue(rm.researching);
            Assert.AreEqual(rm.discovery, disc);
            Assert.AreEqual(rm.progress, 0);
            Assert.IsTrue(rm.research == new Research(0,0,0));

            Consultancy con = new Consultancy();
            rm.HireConsultancy(con);
            con.research = new Research(50,50,50);

            rm.Research();

            Assert.AreEqual(rm.progress, 0.5);
            Assert.IsTrue(rm.research == new Research(50,50,50));

            rm.Research();

            // Research should be completed now.
            Assert.IsFalse(rm.researching);
            Assert.AreEqual(rm.discovery, null);
            Assert.AreEqual(rm.progress, 0);
            Assert.IsTrue(rm.research == new Research(0,0,0));
        }

        [Test]
        public void ResearchProgress() {
            disc.requiredResearch = new Research(100,0,0);

            rm.BeginResearch(disc);

            Consultancy con = new Consultancy();
            rm.HireConsultancy(con);
            con.research = new Research(50,0,0);

            // Check that progress is properly calculated.
            rm.Research();
            Assert.AreEqual(rm.progress, 0.5);

            // Check that progress goes by the required research point types.
            con.research = new Research(0,100,0);

            rm.Research();
            Assert.AreEqual(rm.progress, 0.5);
        }
    }
}
