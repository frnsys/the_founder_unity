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

            GameObject ogO = new GameObject("TestOffice");
            ogO.AddComponent<Office>();
            Office office = ogO.GetComponent<Office>();
            office.size = 0;

            // Should not add employee if company
            // doesn't have this office.
            Assert.IsFalse(company.HireEmployee(employee, office));
            Assert.AreEqual(company.employees.Count, 0);

            // Should not add employee if
            // office is full.
            company.offices.Add(office);
            Assert.IsFalse(company.HireEmployee(employee, office));
            Assert.AreEqual(company.employees.Count, 0);

            // Should add employee if company
            // has this office and it has space.
            office.size = 1;
            Assert.IsTrue(company.HireEmployee(employee, office));
            Assert.AreEqual(company.employees.Count, 1);

            // Should remove the employee.
            company.FireEmployee(employee);
            Assert.AreEqual(company.employees.Count, 0);

            UnityEngine.Object.DestroyImmediate(egO);
            UnityEngine.Object.DestroyImmediate(ogO);
		}

		[Test]
		public void CompanyPayments()
		{
            GameObject egO = new GameObject("TestEmployee");
            egO.AddComponent<Character>();
            Character employee = egO.GetComponent<Character>();
            employee.salary = 1000f;

            GameObject ogO = new GameObject("TestOffice");
            ogO.AddComponent<Office>();
            Office office = ogO.GetComponent<Office>();
            office.size = 1;
            office.baseRent = 500f;

            company.offices.Add(office);
            company.HireEmployee(employee, office);
            company.cash = 2000f;
            company.Pay();

            Assert.AreEqual(company.cash, 500f);

            UnityEngine.Object.DestroyImmediate(egO);
            UnityEngine.Object.DestroyImmediate(ogO);
        }
    }
}
