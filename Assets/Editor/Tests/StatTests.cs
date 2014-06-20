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
		public void StatConstructor()
		{
            Assert.IsNotNull(stat);
            Assert.AreEqual(stat.baseValue, statbasevalue);
            Assert.AreEqual(stat.name, "test stat");
		}

        [Test]
        public void StatBuffConstructor_WithoutDefaults()
        {
            buff = new StatBuff("test stat", 20f, 5, BuffType.multiply);
            Assert.IsNotNull(buff);
            Assert.AreEqual(buff.value, 20f);
            Assert.AreEqual(buff.name, "test stat");
            Assert.AreEqual(buff.duration, 5);
            Assert.AreEqual(buff.type, BuffType.multiply);
        }

        [Test]
        public void StatBuffConstructor_WithDefaults()
        {
            buff = new StatBuff("test stat", 20f);
            Assert.IsNotNull(buff);
            Assert.AreEqual(buff.value, 20f);
            Assert.AreEqual(buff.name, "test stat");
            Assert.AreEqual(buff.duration, 0);
        }

        [Test]
        public void FinalValue_Withoutbuffs()
        {
            Assert.AreEqual(stat.value, statbasevalue);
        }

        [Test]
        public void FinalValue_Withbuffs()
        {
            buff = new StatBuff("test stat", 20f);
            Assert.AreEqual(stat.value, statbasevalue);

            stat.buffs.Add(buff);
            Assert.AreEqual(stat.value, 30f);
        }

        [Test]
        public void FinalValue_WithMultiplebuffs()
        {
            buff = new StatBuff("test stat", 20f);
            StatBuff buff2 = new StatBuff("test stat", 10f);

            stat.buffs.Add(buff);
            stat.buffs.Add(buff2);
            Assert.AreEqual(stat.value, 40f);
        }

        [Test]
        public void FinalValue_WithMultiplyingbuffs()
        {
            float multiple = 2f;
            buff = new StatBuff("test stat", multiple, 0, BuffType.multiply);
            Assert.AreEqual(stat.value, statbasevalue);

            stat.buffs.Add(buff);
            Assert.AreEqual(stat.value, statbasevalue * multiple);
        }

        [Test]
        public void FinalValue_WithBuffOrderOfOperations()
        {
            float multiple = 2f;
            buff = new StatBuff("test stat", multiple, 0, BuffType.multiply);
            StatBuff buff2 = new StatBuff("test stat", 20f, 0, BuffType.add);

            // Adding the multiplication buff first,
            // but we expect the addition one to be calculated first.
            stat.buffs.Add(buff);
            stat.buffs.Add(buff2);
            Assert.AreEqual(stat.value, (20f + statbasevalue) * multiple);
        }

        [Test]
        public void StatBuff_Temporary()
        {
            buff = new StatBuff("test stat", 20f, 100);
            stat.buffs.Add(buff);
            Assert.AreEqual(stat.value, 30f);
            System.Threading.Thread.Sleep(50);
            Assert.AreEqual(stat.value, 30f);
            System.Threading.Thread.Sleep(51);
            Assert.AreEqual(stat.value, statbasevalue);
        }
    }

	[TestFixture]
	internal class StatsTests
    {
        private GameObject gO = null;
        private Stats stats = null;

        [SetUp]
        public void SetUp() {
            gO = new GameObject("HasStats");
            gO.AddComponent<Stats>();
            stats = gO.GetComponent<Stats>();
        }

        [TearDown]
        public void TearDown() {
            UnityEngine.Object.DestroyImmediate(gO);
            stats = null;
        }

        [Test]
        public void LoadBuffs_UnloadBuffs() {
            Stat stat = new Stat("Health", 10f);
            stats.stats.Add(stat);
            Assert.AreEqual(stats.Get("Health").value, 10f);

            GameObject item = new GameObject();
            item.AddComponent<StatBuffs>();
            StatBuffs buffs = item.GetComponent<StatBuffs>();

            StatBuff buff = new StatBuff("Health", 10f, 0);
            buffs.buffs.Add(buff);

            // "Equip" the item.
            stats.LoadBuffs(buffs);

            Assert.AreEqual(stats.Get("Health").value, 20f);

            stats.UnloadBuffs(buffs);

            Assert.AreEqual(stats.Get("Health").value, 10f);

            UnityEngine.Object.DestroyImmediate(item);
            buffs = null;
        }
    }
}
