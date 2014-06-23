using UnityEngine;
using System;
using System.Threading;
using NUnit.Framework;

namespace UnityTest
{
	[TestFixture]
	internal class LevelsTests
	{
        private GameObject gO = null;
        private Levels levels = null;

        [SetUp]
        public void SetUp() {
            gO = new GameObject("Foo Bar");
            gO.AddComponent<Levels>();
            levels = gO.GetComponent<Levels>();
        }

        [TearDown]
        public void TearDown() {
            UnityEngine.Object.DestroyImmediate(gO);
            levels = null;
        }

		[Test]
		public void LevelsConstructor()
		{
            Assert.IsNotNull(levels);
            Assert.AreEqual(levels.level, 1);
            Assert.AreEqual(levels.xp, 0);
		}

		[Test]
		public void GainExperience()
		{
            Assert.AreEqual(levels.xp, 0);
            levels.GainExperience();
            Assert.AreEqual(levels.xp, levels.xpRate);
		}

		[Test]
		public void LevelUp()
		{
            Assert.AreEqual(levels.level, 1);

            levels.GainExperience(1000);
            Assert.AreEqual(levels.level, 2);

            levels.GainExperience(5000);
            Assert.AreEqual(levels.level, 3);
		}
    }
}
