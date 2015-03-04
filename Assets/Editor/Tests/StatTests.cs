using UnityEngine;
using System;
using System.Threading;
using NUnit.Framework;

namespace UnityTest
{
	[TestFixture]
	internal class StatTests
	{
        private Stat stat = null;
        private StatBuff buff = null;

        private float statbasevalue = 10f;

        [SetUp]
        public void SetUp() {
            stat = new Stat("test stat", statbasevalue);
        }

        [TearDown]
        public void TearDown() {
            stat = null;
            buff = null;
        }

		[Test]
		public void StatConstructor() {
            Assert.IsNotNull(stat);
            Assert.AreEqual(stat.baseValue, statbasevalue);
            Assert.AreEqual(stat.name, "test stat");
		}

        [Test]
        public void FinalValue_Withoutbuffs() {
            Assert.AreEqual(stat.value, statbasevalue);
        }

        [Test]
        public void FinalValue_Withbuffs() {
            buff = new StatBuff("test stat", 20f);
            Assert.AreEqual(stat.value, statbasevalue);

            stat.ApplyBuff(buff);
            Assert.AreEqual(stat.value, 30f);
        }

        [Test]
        public void FinalValue_WithMultiplebuffs() {
            buff = new StatBuff("test stat", 20f);
            StatBuff buff2 = new StatBuff("test stat", 10f);

            stat.ApplyBuff(buff);
            stat.ApplyBuff(buff2);
            Assert.AreEqual(stat.value, 40f);
        }
    }
}
