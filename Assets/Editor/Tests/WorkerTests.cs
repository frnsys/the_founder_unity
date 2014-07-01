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
    internal class WorkerTests
    {
        private Worker worker;
        private Item item;

        [SetUp]
        public void SetUp() {
            worker = new Worker(0, 0, 0, 0, 0);
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
        }

        [Test]
        public void ApplyItem() {
            worker.ApplyItem(item);
            Assert.AreEqual(worker.happiness.value, 10);
        }
    }
}