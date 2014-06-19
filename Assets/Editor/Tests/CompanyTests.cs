using UnityEngine;
using System;
using System.Threading;
using NUnit.Framework;

namespace UnityTest
{
	[TestFixture]
	internal class CompanyTests
	{
        private GameObject gO = null;
        private Company company = null;

        [SetUp]
        public void SetUp() {
            gO = new GameObject("Spacebook");
            gO.AddComponent<Company>();
            company = gO.GetComponent<Company>();
        }

        [TearDown]
        public void TearDown() {
            UnityEngine.Object.DestroyImmediate(gO);
            company = null;
        }

		[Test]
		public void CompanyConstructor()
		{
            Assert.IsNotNull(company);
		}

		[Test]
		public void EmployeeManagement()
		{
            GameObject egO = new GameObject("TestEmployee");
            egO.AddComponent<Character>();
            Character employee = egO.GetComponent<Character>();

            company.HireEmployee(employee);
            Assert.AreEqual(company.employees.Count, 1);

            company.FireEmployee(employee);
            Assert.AreEqual(company.employees.Count, 0);

            UnityEngine.Object.DestroyImmediate(egO);
		}
    }
}
