using UnityEngine;
using System;
using System.Threading;
using NUnit.Framework;
using NSubstitute;

namespace UnityTest
{
	[TestFixture]
	internal class CompanyTests
	{
        private Company c = null;
        private GameObject workerObj = null;
        private Worker worker = null;

        [SetUp]
        public void SetUp() {
            c = new Company("Foo Inc");
            workerObj = new GameObject("A Worker");
            workerObj.AddComponent<Worker>();
            worker = workerObj.GetComponent<Worker>();
        }

        [TearDown]
        public void TearDown() {
            UnityEngine.Object.DestroyImmediate(workerObj);
            worker = null;
            c = null;
        }

		[Test]
		public void CompanyConstructor()
		{
            Assert.IsNotNull(c);

            // Creates a name.
            Assert.AreEqual(c.name, "Foo Inc");
		}

		[Test]
		public void ManageWorkers()
		{
            Assert.AreEqual(c.workers.Count, 0);

            c.sizeLimit = 0;
            c.HireWorker(workerObj);
            Assert.AreEqual(c.workers.Count, 0);

            c.sizeLimit = 10;
            c.HireWorker(workerObj);
            Assert.AreEqual(c.workers.Count, 1);

            c.FireWorker(workerObj);
            Assert.AreEqual(c.workers.Count, 0);
        }

		[Test]
		public void Pay()
		{
            worker.salary = 500;
            c.cash = 2000;
            c.HireWorker(workerObj);

            c.Pay();
            Assert.AreEqual(c.cash, 1500);
        }

		[Test]
		public void DevelopProduct()
		{
            c.HireWorker(workerObj);

            worker.productivity.baseValue = 10;
            worker.charisma.baseValue = 10;
            worker.creativity.baseValue = 10;
            worker.cleverness.baseValue = 10;

            IProduct p = Substitute.For<IProduct>();
            c.DevelopProduct(p);

            p.Received().Develop(10, 10, 10, 10);
        }

		[Test]
		public void Buy_CanAfford() {
            c.cash = 2000;
            Item i = new Item("Foo", 500);

            Assert.IsTrue(c.Buy(i));
            Assert.AreEqual(c.cash, 1500);
            Assert.AreEqual(c.items.Count, 1);
        }

		[Test]
		public void Buy_CannotAfford() {
            c.cash = 200;
            Item i = new Item("Foo", 500);

            Assert.IsFalse(c.Buy(i));
            Assert.AreEqual(c.cash, 200);
        }
    }
}
