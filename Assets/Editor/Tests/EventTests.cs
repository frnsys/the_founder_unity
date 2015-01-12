using UnityEngine;
using System;
using System.Threading;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

namespace UnityTest
{
    internal class TestEventListener
    {
        public GameEvent gameEvent;
        public GameEvent triggeredEvent;

        public TestEventListener() {
            GameEvent.EventTriggered += OnEvent;
        }

        void OnEvent(GameEvent e) {
            triggeredEvent = e;
        }
    }

	[TestFixture]
	internal class EventTests
	{
        private GameEvent gE = null;

        [SetUp]
        public void SetUp() {
            gE = new GameEvent("Some event", 1f, false);
        }

        [TearDown]
        public void TearDown() {
            gE = null;
        }

		[Test]
		public void EventConstructor()
		{
            Assert.IsNotNull(gE);
            Assert.AreEqual(gE.name, "Some event");
		}

        [Test]
        public void EffectEvents()
        {
            CashEffect ce = new CashEffect();
            ce.cash = 1000f;
            gE.effects.Add(ce);

            // Our test listener to listen for and capture the event.
            TestEventListener eL = new TestEventListener();

            // Trigger the event.
            GameEvent.Trigger(gE);

            // Check if the event was captured.
            Assert.AreEqual(eL.triggeredEvent, gE);
            Assert.AreEqual(eL.triggeredEvent.effects.ofType<CashEffect>()[0].cash, 1000f);
        }
    }
}
