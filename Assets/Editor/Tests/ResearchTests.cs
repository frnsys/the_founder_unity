using UnityEngine;
using System;
using System.Threading;
using NUnit.Framework;
using NSubstitute;
using System.Collections;
using System.Collections.Generic;

namespace UnityTest
{
    [TestFixture]
    internal class ResearchTests
    {
        private GameObject gameObj;
        private GameManager gameManager;

        [SetUp]
        public void SetUp() {
            gameObj = new GameObject("Game Manager");
            gameManager = gameObj.AddComponent<GameManager>();
            gameManager.researchManager = gameObj.AddComponent<ResearchManager>();
            gameManager.playerCompany = new Company("Foo Inc");
        }

        [TearDown]
        public void TearDown() {
            UnityEngine.Object.DestroyImmediate(gameObj);
            gameManager = null;
        }

        [Test]
        public void HireConsultancy() {
            Consultancy con = new Consultancy();

            gameManager.playerCompany.cash.baseValue = 2000;
            gameManager.HireConsultancy(con);

            Assert.AreEqual(gameManager.playerCompany.cash.baseValue, 2000 - con.cost);
            Assert.AreEqual(gameManager.playerCompany.consultancy, con);
            Assert.AreEqual(gameManager.researchManager.consultancy, con);
        }
    }
}
