using System;
using System.Threading;
using NUnit.Framework;

namespace UnityTest
{
	[TestFixture]
	internal class AttributeTests
	{
        private Attr attribute = null;
        private AttrModifier mod = null;

        private float attrBaseValue = 10f;

        [SetUp]
        public void SetUp() {
            attribute = new Attr("test attribute", attrBaseValue);
        }

        [TearDown]
        public void TearDown() {
            attribute = null;
            mod = null;
        }

		[Test]
		public void AttrConstructor ()
		{
            Assert.IsNotNull(attribute);
            Assert.AreEqual(attribute.baseValue, attrBaseValue);
            Assert.AreEqual(attribute.name, "test attribute");
		}

        [Test]
        public void AttrModifierConstructor_WithoutDefaults()
        {
            mod = new AttrModifier("test attribute", 20f, 5, ModifierType.multiply);
            Assert.IsNotNull(mod);
            Assert.AreEqual(mod.value, 20f);
            Assert.AreEqual(mod.name, "test attribute");
            Assert.AreEqual(mod.duration, 5);
            Assert.AreEqual(mod.type, ModifierType.multiply);
        }

        [Test]
        public void AttrModifierConstructor_WithDefaults()
        {
            mod = new AttrModifier("test attribute", 20f);
            Assert.IsNotNull(mod);
            Assert.AreEqual(mod.value, 20f);
            Assert.AreEqual(mod.name, "test attribute");
            Assert.AreEqual(mod.duration, 0);
        }

        [Test]
        public void FinalValue_WithoutModifiers()
        {
            Assert.AreEqual(attribute.finalValue, attrBaseValue);
        }

        [Test]
        public void FinalValue_WithModifiers()
        {
            mod = new AttrModifier("test attribute", 20f);
            Assert.AreEqual(attribute.finalValue, attrBaseValue);

            attribute.modifiers.Add(mod);
            Assert.AreEqual(attribute.finalValue, 30f);
        }

        [Test]
        public void FinalValue_WithMultipleModifiers()
        {
            mod = new AttrModifier("test attribute", 20f);
            AttrModifier mod2 = new AttrModifier("test attribute", 10f);

            attribute.modifiers.Add(mod);
            attribute.modifiers.Add(mod2);
            Assert.AreEqual(attribute.finalValue, 40f);
        }

        [Test]
        public void FinalValue_WithMultiplyingModifiers()
        {
            float multiple = 2f;
            mod = new AttrModifier("test attribute", multiple, 0, ModifierType.multiply);
            Assert.AreEqual(attribute.finalValue, attrBaseValue);

            attribute.modifiers.Add(mod);
            Assert.AreEqual(attribute.finalValue, attrBaseValue * multiple);
        }

        [Test]
        public void FinalValue_WithModifierOrderOfOperations()
        {
            float multiple = 2f;
            mod = new AttrModifier("test attribute", multiple, 0, ModifierType.multiply);
            AttrModifier mod2 = new AttrModifier("test attribute", 20f, 0, ModifierType.add);

            // Adding the multiplication mod first,
            // but we expect the addition one to be calculated first.
            attribute.modifiers.Add(mod);
            attribute.modifiers.Add(mod2);
            Assert.AreEqual(attribute.finalValue, (20f + attrBaseValue) * multiple);
        }

        [Test]
        public void AttrModifier_Temporary()
        {
            mod = new AttrModifier("test attribute", 20f, 100);
            attribute.modifiers.Add(mod);
            Assert.AreEqual(attribute.finalValue, 30f);
            System.Threading.Thread.Sleep(50);
            Assert.AreEqual(attribute.finalValue, 30f);
            System.Threading.Thread.Sleep(51);
            Assert.AreEqual(attribute.finalValue, attrBaseValue);
        }
    }
}
