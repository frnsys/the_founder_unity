using UnityEngine;
using UnityEditor;
using System;
using System.Threading;
using NUnit.Framework;

namespace UnityTest
{
	[TestFixture]
	internal class UnlockableTests
	{
        private UnlockSet unlocked = null;

        [SetUp]
        public void SetUp() {
            unlocked = new UnlockSet();
        }

        [TearDown]
        public void TearDown() {
            unlocked = null;
        }

		[Test]
		public void UnlockSetConstructor() {
            Assert.IsNotNull(unlocked);
		}

		[Test]
		public void UnlocksUnlockables() {
            UnlockSet unlockables = new UnlockSet();

            unlockables.productTypes.Add(LoadTestResource("TestProductType", typeof(ProductType)) as ProductType);
            unlockables.verticals.Add(LoadTestResource("TestVertical", typeof(Vertical)) as Vertical);
            unlockables.events.Add(LoadTestResource("TestGameEvent", typeof(GameEvent)) as GameEvent);
            unlockables.workers.Add(LoadTestResource("TestWorker", typeof(Worker)) as Worker);
            unlockables.items.Add(LoadTestResource("TestItem", typeof(Item)) as Item);

            Assert.AreEqual(unlocked.productTypes.Count, 0);
            Assert.AreEqual(unlocked.verticals.Count, 0);
            Assert.AreEqual(unlocked.events.Count, 0);
            Assert.AreEqual(unlocked.workers.Count, 0);
            Assert.AreEqual(unlocked.items.Count, 0);

            unlocked.Unlock(unlockables);

            Assert.AreEqual(unlocked.productTypes.Count, 1);
            Assert.AreEqual(unlocked.verticals.Count, 1);
            Assert.AreEqual(unlocked.events.Count, 1);
            Assert.AreEqual(unlocked.workers.Count, 1);
            Assert.AreEqual(unlocked.items.Count, 1);
		}

        private UnityEngine.Object LoadTestResource(string name, Type type) {
            return AssetDatabase.LoadAssetAtPath("Assets/Editor/Tests/Resources/"+name+".asset", typeof(Item));
        }
    }
}
