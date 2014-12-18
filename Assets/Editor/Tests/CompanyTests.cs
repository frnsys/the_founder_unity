using UnityEngine;
using UnityEditor;
using System;
using System.Threading;
using NUnit.Framework;
using NSubstitute;
using System.Collections;
using System.Collections.Generic;

namespace UnityTest
{
	[TestFixture]
	internal class CompanyTests
	{
        private GameObject gameObj;
        private GameManager gameManager;
        private Company c;
        private Worker worker;
        private Item item;

        private ProductType pt = new ProductType();
        private Industry i = new Industry();
        private Market m = new Market();

        [SetUp]
        public void SetUp() {
            gameObj = new GameObject("Game Manager");
            gameManager = gameObj.AddComponent<GameManager>();

            c = new Company("Foo Inc");
            c.productPoints = 10;

            worker = ScriptableObject.CreateInstance<Worker>();
            worker.Init("Franklin");

            item = AssetDatabase.LoadAssetAtPath("Assets/Editor/Tests/Resources/TestItem.asset", typeof(Item)) as Item;
        }

        [TearDown]
        public void TearDown() {
            UnityEngine.Object.DestroyImmediate(gameObj);
            gameManager = null;
            worker = null;
            c = null;
            item = null;
        }

		[Test]
		public void CompanyConstructor() {
            Assert.IsNotNull(c);

            // Creates a name.
            Assert.AreEqual(c.name, "Foo Inc");
		}

		[Test]
		public void ManageWorkers() {
            Assert.AreEqual(c.workers.Count, 0);

            c.baseSizeLimit = 0;
            c.HireWorker(worker);
            Assert.AreEqual(c.workers.Count, 0);

            c.cash.baseValue = 2000;
            c.BuyItem(item);
            c.baseSizeLimit = 10;
            c.HireWorker(worker);
            Assert.AreEqual(c.workers.Count, 1);
            Assert.AreEqual(c.workers[0].happiness.value, 10);

            c.FireWorker(worker);
            Assert.AreEqual(c.workers.Count, 0);
        }

		[Test]
		public void PayMonthly() {
            Infrastructure inf = ScriptableObject.CreateInstance<Infrastructure>();
            inf.cost = 200;
            c.infrastructures.Add(inf);

            Location loc = ScriptableObject.CreateInstance<Location>();
            loc.rent = 100;
            c.locations.Add(loc);

            worker.salary = 500;
            c.cash.baseValue = 2000;

            float paid = worker.salary + c.researchCash + inf.cost + loc.rent;

            c.HireWorker(worker);

            c.PayMonthly();
            Assert.AreEqual(c.cash.baseValue, 2000 - paid);
        }



        // ===============================================
        // Product Management ============================
        // ===============================================

        [Test]
        public void StartNewProduct() {
            c.cash.baseValue = 2000;
            c.BuyItem(item);

            c.StartNewProduct(pt, i, m);
            Assert.AreEqual(c.developingProducts[0], c.products[0]);
            Assert.AreEqual(c.availableProductPoints, c.productPoints - (pt.points + i.points + m.points));

            // Creating a new product should apply existing items.
            Assert.AreEqual(c.products.Count, 1);
            Assert.AreEqual(c.products[0].appeal.value, 10);
        }

		[Test]
		public void DevelopProduct() {
            c.HireWorker(worker);

            worker.productivity.baseValue = 10;
            worker.charisma.baseValue = 10;
            worker.creativity.baseValue = 10;
            worker.cleverness.baseValue = 10;

            Product p = ScriptableObject.CreateInstance<Product>();
            p.Init(pt, i, m);
            c.DevelopProduct(p);

            Assert.IsTrue(p.progress > 0);
            Assert.IsTrue(p.appeal.value > 0);
            Assert.IsTrue(p.usability.value > 0);
            Assert.IsTrue(p.performance.value > 0);
        }

		[Test]
		public void HarvestProduct() {
            c.cash.baseValue = 2000;
            c.HireWorker(worker);

            worker.productivity.baseValue = 100;
            worker.charisma.baseValue = 100;
            worker.creativity.baseValue = 100;
            worker.cleverness.baseValue = 100;

            c.StartNewProduct(pt, i, m);
            Product p = c.products[0];
            c.DevelopProduct(p);

            p.Launch();
            Assert.AreEqual(c.activeProducts[0], p);

            c.HarvestProducts(2);

            Assert.IsTrue(c.cash.baseValue > 2000);
        }

        [Test]
        public void ShutdownProduct() {
            c.cash.baseValue = 2000;
            c.BuyItem(item);

            c.StartNewProduct(pt, i, m);
            Product p = c.products[0];
            Assert.AreEqual(p.appeal.value, 10);

            c.ShutdownProduct(p);

            Assert.AreEqual(p.state, Product.State.RETIRED);
            Assert.AreEqual(p.appeal.value, 0);
            Assert.AreEqual(c.availableProductPoints, c.productPoints);
        }



        // ===============================================
        // Item Management ===============================
        // ===============================================

		[Test]
		public void BuyItem_CanAfford() {
            c.cash.baseValue = 2000;
            c.StartNewProduct(pt, i, m);
            Product p = c.products[0];
            c.HireWorker(worker);

            Assert.IsTrue(c.BuyItem(item));
            Assert.AreEqual(c.cash.baseValue, 1500);
            Assert.AreEqual(c.items.Count, 1);
            Assert.AreEqual(p.appeal.value, 10);
            Assert.AreEqual(worker.happiness.value, 10);
            Assert.AreEqual(worker.productivity.value, 20);

            // Item should be removed from worker.
            c.FireWorker(worker);
            Assert.AreEqual(worker.happiness.value, 0);
            Assert.AreEqual(worker.productivity.value, 0);
        }

		[Test]
		public void BuyItem_CannotAfford() {
            c.cash.baseValue = 200;
            Assert.IsFalse(c.BuyItem(item));
            Assert.AreEqual(c.cash.baseValue, 200);
        }

        [Test]
        public void RemoveItem() {
            c.cash.baseValue = 2000;
            c.StartNewProduct(pt, i, m);
            Product p = c.products[0];
            c.HireWorker(worker);

            c.BuyItem(item);
            c.RemoveItem(item);

            Assert.AreEqual(c.items.Count, 0);
            Assert.AreEqual(p.appeal.value, 0);
            Assert.AreEqual(worker.happiness.value, 0);
        }
    }
}
