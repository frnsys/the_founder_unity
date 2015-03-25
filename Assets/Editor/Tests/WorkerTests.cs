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
    internal class WorkerTests
    {
        private GameObject gameObj;
        private GameManager gm;
        private GameData gd;
        private WorkerManager wm;

        private Worker worker;

        [SetUp]
        public void SetUp() {
            gameObj = new GameObject("Game Manager");
            gm = gameObj.AddComponent<GameManager>();

            gd = GameData.New("DEFAULTCORP");
            gm.Load(gd);

            wm = gm.workerManager;

            worker = ScriptableObject.CreateInstance<Worker>();
            worker.Init("Franklin");

            gd.unemployed.Add(worker);
        }

        [TearDown]
        public void TearDown() {
            UnityEngine.Object.DestroyImmediate(gameObj);
            gm = null;
            worker = null;
        }

        [Test]
        public void WorkerConstructor() {
            Assert.IsNotNull(worker);
            Assert.AreEqual(worker.happiness.value, 0);
            Assert.AreEqual(worker.productivity.value, 0);
        }

        [Test]
        public void MinimumSalary() {
            worker.baseMinSalary = 1;

            Assert.AreEqual(worker.MinSalaryForCompany(gd.company), worker.baseMinSalary);

            // Emulate the worker being hired
            worker.salary = 10000;
            worker.happiness.baseValue = 10;

            // The min salary should be higher now.
            Assert.IsTrue(worker.MinSalaryForCompany(gd.company) > worker.baseMinSalary);
        }

        [Test]
        public void MinimumSalaryAgainstCompanyHappiness() {
            // Create a worker for the company.
            Worker w = ScriptableObject.CreateInstance<Worker>();
            w.Init("Dava");
            gm.playerCompany.HireWorker(w);
            Assert.AreEqual(gm.playerCompany.workers.Count, 1);

            // Current employee is unhappy.
            w.happiness.baseValue = 0;

            // Emulate the worker being hired
            // Happy at current job.
            worker.baseMinSalary = 1;
            worker.salary = 1;
            worker.happiness.baseValue = 10;

            // The min salary should be higher since the worker expects to be
            // less happy at the new company.
            Assert.IsTrue(worker.MinSalaryForCompany(gd.company) > worker.baseMinSalary);

            // Current employee is happy.
            w.happiness.baseValue = 40;

            // The min salary should be lower since the worker expects to be happier
            // at the new company.
            Assert.IsTrue(worker.MinSalaryForCompany(gd.company) < worker.baseMinSalary);
        }

        [Test]
        public void HireWorkers() {
            Company c = gm.playerCompany;
            Worker w = ScriptableObject.CreateInstance<Worker>();
            w.Init("Sammy");
            gd.unemployed.Add(w);

            int startingCount = wm.AvailableWorkers.Count();

            wm.HireWorker(w, c);
            Assert.IsTrue(c.workers.Contains(w));

            // Should be -1 now that the other worker is hired.
            Assert.AreEqual(wm.AvailableWorkers.Count(), startingCount - 1);
            Assert.IsFalse(gd.unemployed.Contains(w));

            Company c_ = ScriptableObject.CreateInstance<Company>();
            c_.Init();
            w.salary = 333333;
            c_.cash.baseValue = 333333;
            wm.HireWorker(w, c_);

            // These should not have changed.
            Assert.AreEqual(wm.AvailableWorkers.Count(), startingCount - 1);
            Assert.IsFalse(gd.unemployed.Contains(w));

            // The worker should now be gone from the first company.
            Assert.IsFalse(c.workers.Contains(w));
            Assert.IsTrue(c_.workers.Contains(w));

            // Just check that the salary is preserved.
            Assert.AreEqual(w.salary, 333333);

            wm.FireWorker(w, c_);

            Assert.AreEqual(wm.AvailableWorkers.Count(), startingCount);
            Assert.IsTrue(gd.unemployed.Contains(w));
            Assert.IsFalse(c.workers.Contains(w));
            Assert.IsFalse(c_.workers.Contains(w));
        }
    }
}