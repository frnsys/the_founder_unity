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
        private GameData gd;
        private GameManager gm;
        private ResearchManager rm;
        private Technology tech;

        [SetUp]
        public void SetUp() {
            gameObj = new GameObject("Game Manager");
            gm = gameObj.AddComponent<GameManager>();
            gm.researchManager = gameObj.AddComponent<ResearchManager>();

            gd = GameData.New("DEFAULTCORP");
            gm.Load(gd);

            rm = gm.researchManager;
            tech = AssetDatabase.LoadAssetAtPath("Assets/Editor/Tests/Resources/TestTechnology.asset", typeof(Technology)) as Technology;

            gd.company.research.baseValue = 50;
            gd.company.researchCash = 0;
        }

        [TearDown]
        public void TearDown() {
            UnityEngine.Object.DestroyImmediate(gameObj);
            gm = null;
        }

        [Test]
        public void Research() {
            Assert.IsFalse(rm.researching);

            rm.BeginResearch(tech);

            Assert.IsTrue(rm.researching);
            Assert.AreEqual(rm.technology, tech);
            Assert.AreEqual(rm.progress, 0);
            Assert.IsTrue(rm.research == 0);

            rm.Research();

            Assert.AreEqual(rm.progress, 0.5);
            Assert.IsTrue(rm.research == 50);

            rm.Research();

            // Research should be completed now.
            Assert.IsFalse(rm.researching);
            Assert.AreEqual(rm.technology, null);
            Assert.AreEqual(rm.progress, 0);
            Assert.IsTrue(rm.research == 0);
        }

        [Test]
        public void ResearchProgress() {
            tech.requiredResearch = 100;

            rm.BeginResearch(tech);

            // Check that progress is properly calculated.
            rm.Research();
            Assert.AreEqual(rm.progress, 0.5);
        }
    }
}
