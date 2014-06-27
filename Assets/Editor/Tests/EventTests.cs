using UnityEngine;
using System;
using System.Threading;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

namespace UnityTest
{
	[TestFixture]
	internal class EventTests
	{
        private GameEventType gET = null;
        private GameEvent gE = null;

        [SetUp]
        public void SetUp() {
            gET = new GameEventType("Security", 1f);
            gE = new GameEvent("Some event");
        }

        [TearDown]
        public void TearDown() {
            gET = null;
            gE = null;
        }

		[Test]
		public void EventConstructor()
		{
            Assert.IsNotNull(gE);
            Assert.AreEqual(gE.name, "Some event");
		}

		[Test]
		public void EventRolling()
		{
            // Tweak the probabilities so we for sure get the
            // event we're testing for.
            gET.bad = 1f;
            gET.bad_catastrophic = 1f;

            List<GameEventType> gETs = new List<GameEventType>() {
                gET
            };

            List<GameEvent> gEs = GameEvent.Roll(gETs);

            Assert.AreEqual(gEs[0].name, "YOU WERE HACKED");
		}
    }
}
