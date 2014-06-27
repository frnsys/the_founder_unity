using UnityEngine;
using System;
using System.Threading;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

namespace UnityTest
{
	[TestFixture]
	internal class EventTypeTests
	{
        private GameEventType gET = null;

        [SetUp]
        public void SetUp() {
            gET = new GameEventType("Security", 1f);
        }

        [TearDown]
        public void TearDown() {
            gET = null;
        }

		[Test]
		public void EventTypeConstructor()
		{
            Assert.IsNotNull(gET);
            Assert.AreEqual(gET.name, "Security");
            Assert.AreEqual(gET.probability, 1f);
		}

		[Test]
		public void SetParityAdjustsOtherParity()
		{
            gET.good = 0.12f;
            Assert.AreEqual(gET.good, 0.12f);
            Assert.AreEqual(gET.bad, 1f - 0.12f);
		}

		[Test]
		public void SetSeverityAdjustsOtherSeverities()
		{
            gET.good_minor = 0.12f;
            Assert.AreEqual(gET.good_major, 0.792f, 0.001f);
            Assert.AreEqual(gET.good_catastrophic, 0.088f, 0.001f);
		}
    }
}
