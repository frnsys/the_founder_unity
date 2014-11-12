using UnityEngine;
using UnityEditor;
using System;
using System.Threading;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

namespace UnityTest
{
    [TestFixture]
    internal class WorkerTests
    {
        private Worker worker;
        private Item item;

        [SetUp]
        public void SetUp() {
            worker = ScriptableObject.CreateInstance<Worker>();
            worker.Init("Franklin");
            item = AssetDatabase.LoadAssetAtPath("Assets/Editor/Tests/Resources/TestItem.asset", typeof(Item)) as Item;
        }

        [TearDown]
        public void TearDown() {
            worker = null;
        }

        [Test]
        public void WorkerConstructor() {
            Assert.IsNotNull(worker);
            Assert.AreEqual(worker.happiness.value, 0);
            Assert.AreEqual(worker.productivity.value, 0);
        }

        [Test]
        public void ApplyItem() {
            worker.ApplyItem(item);
            Assert.AreEqual(worker.happiness.value, 10);
            Assert.AreEqual(worker.productivity.value, 20);
        }

        [Test]
        public void RemoveItem() {
            worker.ApplyItem(item);
            worker.RemoveItem(item);
            Assert.AreEqual(worker.happiness.value, 0);
            Assert.AreEqual(worker.productivity.value, 0);
        }
    }
}