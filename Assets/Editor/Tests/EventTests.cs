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
        public GameEffect effect;

        public TestEventListener(GameEvent gameEvent) {
            gameEvent.EffectEvent += OnEffect;
        }

        void OnEffect(GameEffect e) {
            effect = e;
        }
    }

	[TestFixture]
	internal class EventTests
	{
        private GameEvent gE = null;

        [SetUp]
        public void SetUp() {
            gE = new GameEvent("Some event", 1f);
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
		public void EventRolling()
		{
            List<GameEvent> candidateEvents = new List<GameEvent>() {
                gE
            };

            List<GameEvent> happeningEvents = GameEvent.Roll(candidateEvents);

            Assert.AreEqual(happeningEvents[0].name, "Some event");
		}

        [Test]
        public void EffectEvents()
        {
            // Add an effect to the event.
            GameEffect effect = new GameEffect();
            effect.companyBuffs.Add(new StatBuff("Cash", 1000f));
            gE.effects.Add(effect);

            // Our test listener to listen for and capture the effect.
            TestEventListener eL = new TestEventListener(gE);

            // Trigger the event's effects.
            gE.Trigger();

            // Check if the effect was captured.
            Assert.AreEqual(eL.effect, effect);
            Assert.AreEqual(eL.effect.companyBuffs[0].value, 1000f);
        }
    }
}
