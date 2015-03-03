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

        private Technology tech;
        private Vertical vert;

        [SetUp]
        public void SetUp() {
            gameObj = new GameObject("Game Manager");
            gm = gameObj.AddComponent<GameManager>();
            gd = GameData.New("DEFAULTCORP");
            gm.Load(gd);

            tech = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Editor/Tests/Resources/TestTechnology.asset", typeof(Technology))) as Technology;
            vert = AssetDatabase.LoadAssetAtPath("Assets/Editor/Tests/Resources/TestVertical.asset", typeof(Vertical)) as Vertical;
            gd.company.research.baseValue = 50;
            gd.company.researchInvestment = 0;
        }

        [TearDown]
        public void TearDown() {
            UnityEngine.Object.DestroyImmediate(gameObj);
            gm = null;
        }

        [Test]
        public void TechnologyAvailableConditions() {
            // Should be false as the company has no verticals yet.
            Assert.IsFalse(tech.isAvailable(gd.company));

            gd.company.cash.baseValue = 2000;
            gd.company.ExpandToVertical(vert);
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
