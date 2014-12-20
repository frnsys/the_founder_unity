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
        private Vertical vert;

        [SetUp]
        public void SetUp() {
            gameObj = new GameObject("Game Manager");
            gm = gameObj.AddComponent<GameManager>();
            gm.researchManager = gameObj.AddComponent<ResearchManager>();

            gd = GameData.New("DEFAULTCORP");
            gm.Load(gd);

            rm = gm.researchManager;
            tech = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Editor/Tests/Resources/TestTechnology.asset", typeof(Technology))) as Technology;
            vert = AssetDatabase.LoadAssetAtPath("Assets/Editor/Tests/Resources/TestVertical.asset", typeof(Vertical)) as Vertical;
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

        [Test]
        public void TechnologyAvailableConditions() {
            // Should be false as the company has no verticals yet.
            Assert.IsFalse(tech.isAvailable(gd.company));

            gd.company.verticals.Add(vert);
            Assert.IsTrue(tech.isAvailable(gd.company));

            Technology requiredTech = ScriptableObject.CreateInstance<Technology>();
            tech.requiredTechnologies.Add(requiredTech);

            // Should be false as the company does not have the required tech.
            Assert.IsFalse(tech.isAvailable(gd.company));

            gd.company.technologies.Add(requiredTech);
            Assert.IsTrue(tech.isAvailable(gd.company));
        }
    }
}
