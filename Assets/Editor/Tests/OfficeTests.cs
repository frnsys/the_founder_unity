using UnityEngine;
using System;
using System.Threading;
using NUnit.Framework;

namespace UnityTest
{
	[TestFixture]
	internal class OfficeTests
	{
        private GameObject gO = null;
        private Office office = null;

        [SetUp]
        public void SetUp() {
            gO = new GameObject("Skyscraper");
            gO.AddComponent<Office>();
            office = gO.GetComponent<Office>();
        }

        [TearDown]
        public void TearDown() {
            UnityEngine.Object.DestroyImmediate(gO);
            office = null;
        }

		[Test]
		public void OfficeConstructor()
		{
            Assert.IsNotNull(office);
		}

		[Test]
		public void OfficeRent_WithLocation()
		{
            GameObject lgO = new GameObject("TestLocation");
            lgO.AddComponent<Location>();
            Location location = lgO.GetComponent<Location>();
            location.expensiveness = 10;

            office.baseRent = 1000;
            office.location = location;
            Assert.AreEqual(office.rent, 10000);

            UnityEngine.Object.DestroyImmediate(lgO);
		}

		[Test]
		public void OfficeRent_WithoutLocation()
		{
            office.baseRent = 1000;
            Assert.AreEqual(office.rent, 1000);
		}
    }
}
