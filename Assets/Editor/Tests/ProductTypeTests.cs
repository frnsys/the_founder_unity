using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

namespace UnityTest
{
    [TestFixture]
    internal class ProductTypeTests
    {
        private GameObject gameObj;
        private GameData gd;
        private GameManager gm;
        private ProductType pt;
        private Vertical vert;

        [SetUp]
        public void SetUp() {
            gameObj = new GameObject("Game Manager");
            gm = gameObj.AddComponent<GameManager>();

            gd = GameData.New("DEFAULTCORP");
            gm.Load(gd);

            pt = (ProductType)GameObject.Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Editor/Tests/Resources/TestProductType.asset", typeof(ProductType)));
            vert = AssetDatabase.LoadAssetAtPath("Assets/Editor/Tests/Resources/TestVertical.asset", typeof(Vertical)) as Vertical;
        }

        [TearDown]
        public void TearDown() {
            UnityEngine.Object.DestroyImmediate(gameObj);
            gm = null;
            pt = null;
        }

        [Test]
        public void AvailableConditions() {
            // The company does not yet have the required vertical.
            Assert.IsFalse(pt.isAvailable(gd.company));

            gd.company.cash.baseValue = 2000;
            gd.company.ExpandToVertical(vert);

            Assert.IsTrue(pt.isAvailable(gd.company));
        }
    }
}
