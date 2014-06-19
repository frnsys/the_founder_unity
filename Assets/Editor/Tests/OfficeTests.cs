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

		[Test]
		public void OfficeHappiness()
		{
            GameObject egO = new GameObject("TestEmployee");
            egO.AddComponent<Character>();
            Character employee = egO.GetComponent<Character>();
            employee.happiness.baseValue = 100f;
            office.employees.Add(employee);

            GameObject egO_ = new GameObject("TestEmployee_");
            egO_.AddComponent<Character>();
            Character employee_ = egO_.GetComponent<Character>();
            employee_.happiness.baseValue = 200f;
            office.employees.Add(employee_);

            Assert.AreEqual(office.happiness, 150f);

            UnityEngine.Object.DestroyImmediate(egO);
            UnityEngine.Object.DestroyImmediate(egO_);
		}

		[Test]
		public void OfficeProductivity()
		{
            GameObject egO = new GameObject("TestEmployee");
            egO.AddComponent<Character>();
            Character employee = egO.GetComponent<Character>();
            employee.productivity.baseValue = 100f;
            office.employees.Add(employee);

            GameObject egO_ = new GameObject("TestEmployee_");
            egO_.AddComponent<Character>();
            Character employee_ = egO_.GetComponent<Character>();
            employee_.productivity.baseValue = 200f;
            office.employees.Add(employee_);

            Assert.AreEqual(office.productivity, 150f);

            UnityEngine.Object.DestroyImmediate(egO);
            UnityEngine.Object.DestroyImmediate(egO_);
		}
    }
}
