using System;
using System.Threading;
using NUnit.Framework;

namespace UnityTest
{
	[TestFixture]
	internal class StatTests
	{
        private Stat stat = null;
        private StatModifier mod = null;

        private float statbasevalue = 10f;

        [SetUp]
        public void SetUp() {
            stat = new Stat("test stat", statbasevalue);
        }

        [TearDown]
        public void TearDown() {
            stat = null;
            mod = null;
        }

		[Test]
		public void StatConstructor()
		{
            Assert.IsNotNull(stat);
            Assert.AreEqual(stat.baseValue, statbasevalue);
            Assert.AreEqual(stat.name, "test stat");
		}

        [Test]
        public void StatModifierConstructor_WithoutDefaults()
        {
            mod = new StatModifier("test stat", 20f, 5, ModifierType.multiply);
            Assert.IsNotNull(mod);
            Assert.AreEqual(mod.value, 20f);
            Assert.AreEqual(mod.name, "test stat");
            Assert.AreEqual(mod.duration, 5);
            Assert.AreEqual(mod.type, ModifierType.multiply);
        }

        [Test]
        public void StatModifierConstructor_WithDefaults()
        {
            mod = new StatModifier("test stat", 20f);
            Assert.IsNotNull(mod);
            Assert.AreEqual(mod.value, 20f);
            Assert.AreEqual(mod.name, "test stat");
            Assert.AreEqual(mod.duration, 0);
        }

        [Test]
        public void FinalValue_WithoutModifiers()
        {
            Assert.AreEqual(stat.finalValue, statbasevalue);
        }

        [Test]
        public void FinalValue_WithModifiers()
        {
            mod = new StatModifier("test stat", 20f);
            Assert.AreEqual(stat.finalValue, statbasevalue);

            stat.modifiers.Add(mod);
            Assert.AreEqual(stat.finalValue, 30f);
        }

        [Test]
        public void FinalValue_WithMultipleModifiers()
        {
            mod = new StatModifier("test stat", 20f);
            StatModifier mod2 = new StatModifier("test stat", 10f);

            stat.modifiers.Add(mod);
            stat.modifiers.Add(mod2);
            Assert.AreEqual(stat.finalValue, 40f);
        }

        [Test]
        public void FinalValue_WithMultiplyingModifiers()
        {
            float multiple = 2f;
            mod = new StatModifier("test stat", multiple, 0, ModifierType.multiply);
            Assert.AreEqual(stat.finalValue, statbasevalue);

            stat.modifiers.Add(mod);
            Assert.AreEqual(stat.finalValue, statbasevalue * multiple);
        }

        [Test]
        public void FinalValue_WithModifierOrderOfOperations()
        {
            float multiple = 2f;
            mod = new StatModifier("test stat", multiple, 0, ModifierType.multiply);
            StatModifier mod2 = new StatModifier("test stat", 20f, 0, ModifierType.add);

            // Adding the multiplication mod first,
            // but we expect the addition one to be calculated first.
            stat.modifiers.Add(mod);
            stat.modifiers.Add(mod2);
            Assert.AreEqual(stat.finalValue, (20f + statbasevalue) * multiple);
        }

        [Test]
        public void StatModifier_Temporary()
        {
            mod = new StatModifier("test stat", 20f, 100);
            stat.modifiers.Add(mod);
            Assert.AreEqual(stat.finalValue, 30f);
            System.Threading.Thread.Sleep(50);
            Assert.AreEqual(stat.finalValue, 30f);
            System.Threading.Thread.Sleep(51);
            Assert.AreEqual(stat.finalValue, statbasevalue);
        }
    }
}
