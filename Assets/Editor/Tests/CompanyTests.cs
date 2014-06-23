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
    }
}
