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
            // Create a starting location with some infrastructure capacity.
            Location startLoc = ScriptableObject.CreateInstance<Location>();
            startLoc.cost = 0;
            startLoc.capacity = new Infrastructure();
            startLoc.capacity[Infrastructure.Type.Datacenter] = 2;
            startLoc.capacity[Infrastructure.Type.Factory]    = 1;
            gd.company.ExpandToLocation(startLoc);

            // The company does not yet have the required vertical.
            Assert.IsFalse(pt.isAvailable(gd.company));

            gd.company.cash.baseValue = 2000;
            gd.company.ExpandToVertical(vert);

            // Should still be false since the company does not have the required infrastructure.
            Assert.IsFalse(pt.isAvailable(gd.company));

            Infrastructure i = new Infrastructure();
            i[Infrastructure.Type.Datacenter] = 2;
            i[Infrastructure.Type.Factory] = 1;

            gd.company.cash.baseValue = i.cost;
            gd.company.BuyInfrastructure(i, startLoc);

            Assert.IsTrue(pt.isAvailable(gd.company));
        }
    }
}
