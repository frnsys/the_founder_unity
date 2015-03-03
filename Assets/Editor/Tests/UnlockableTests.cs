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

            Assert.AreEqual(unlocked.productTypes.Count, 0);
            Assert.AreEqual(unlocked.verticals.Count, 0);

            unlocked.Unlock(unlockables);

            Assert.AreEqual(unlocked.productTypes.Count, 1);
            Assert.AreEqual(unlocked.verticals.Count, 1);
		}

        [Test]
        public void UnlockSetIgnoresDuplicates() {
            UnlockSet unlockables = new UnlockSet();

            unlockables.productTypes.Add(LoadTestResource("TestProductType", typeof(ProductType)) as ProductType);
            Assert.AreEqual(unlocked.productTypes.Count, 0);

            unlocked.Unlock(unlockables);
            Assert.AreEqual(unlocked.productTypes.Count, 1);

            unlocked.Unlock(unlockables);
            Assert.AreEqual(unlocked.productTypes.Count, 1);
        }

        private UnityEngine.Object LoadTestResource(string name, Type type) {
            return AssetDatabase.LoadAssetAtPath("Assets/Editor/Tests/Resources/"+name+".asset", type);
        }
    }
}
