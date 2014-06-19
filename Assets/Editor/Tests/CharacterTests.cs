using UnityEngine;
using System;
using System.Threading;
using NUnit.Framework;

namespace UnityTest
{
	[TestFixture]
	internal class CharacterTests
	{
        private GameObject gO = null;
        private Character character = null;

        [SetUp]
        public void SetUp() {
            gO = new GameObject("Foo Bar");
            gO.AddComponent<Character>();
            character = gO.GetComponent<Character>();
        }

        [TearDown]
        public void TearDown() {
            UnityEngine.Object.DestroyImmediate(gO);
            character = null;
        }

		[Test]
		public void CharacterConstructor()
		{
            Assert.IsNotNull(character);
            Assert.AreEqual(character.level, 1);
            Assert.AreEqual(character.xp, 0);
		}

		[Test]
		public void CharacterExperience()
		{
            Assert.AreEqual(character.xp, 0);
            character.GainExperience();
            Assert.AreEqual(character.xp, character.xprate.value);
		}

		[Test]
		public void CharacterLevelUp()
		{
            Assert.AreEqual(character.level, 1);

            character.GainExperience(1000);
            Assert.AreEqual(character.level, 2);

            character.GainExperience(5000);
            Assert.AreEqual(character.level, 3);
		}
    }
}
