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
        private GameObject gameObj;
        private GameData gd;
        private EventManager em;

        private GameEvent gE = null;

        [SetUp]
        public void SetUp() {
            gE = new GameEvent("Some event", 1f);

            gameObj = new GameObject("Event Manager");
            em = gameObj.AddComponent<EventManager>();
            gd = GameData.New("DEFAULTCORP");
            em.Load(gd);
        }

        [TearDown]
        public void TearDown() {
            UnityEngine.Object.DestroyImmediate(gameObj);
            gE = null;
            em = null;
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
            gE.effects.cash = 1000f;

            // Our test listener to listen for and capture the event.
            TestEventListener eL = new TestEventListener();

            // Trigger the event.
            GameEvent.Trigger(gE);

            // Check if the event was captured.
            Assert.AreEqual(eL.triggeredEvent, gE);
            Assert.AreEqual(eL.triggeredEvent.effects.cash, 1000f);
        }

        [Test]
        public void Tick() {
            gE.delay = 10;
            em.Add(gE);

            em.Tick();

            Assert.AreEqual(gE.delay, 9);
        }

        [Test]
        public void TickResolve() {
            gE.delay = 1;
            gE.probability = 1;
            em.Add(gE);

            // Our test listener to listen for and capture the event.
            TestEventListener eL = new TestEventListener();

            em.Tick();

            Assert.AreEqual(eL.triggeredEvent, gE);
            Assert.IsFalse(gd.eventsPool.Contains(gE));
        }

        [Test]
        public void TickResolveMultiple() {
            gE.delay = 1;
            gE.probability = 1;
            em.Add(gE);

            GameEvent gE_ = new GameEvent("Some event", 1f);
            gE_.delay = 1;
            gE_.probability = 1;
            em.Add(gE_);

            Assert.AreEqual(gd.eventsPool.Count, 2);

            // Our test listener to listen for and capture the event.
            TestEventListener eL = new TestEventListener();

            em.Tick();

            // One of the events should have been triggered.
            List<GameEvent> ges = new List<GameEvent>() { gE, gE_ };
            Assert.IsTrue(ges.Contains(eL.triggeredEvent));

            // There should only be one event left.
            Assert.AreEqual(gd.eventsPool.Count, 1);

            // That event should have an increased delay.
            Assert.IsTrue(gd.eventsPool[0].delay > 0);
        }

        [Test]
        public void Conditions() {
            GameEvent.Condition pc = new GameEvent.Condition();
            pc.value = 20;
            pc.greater = true;
            pc.type = GameEvent.Condition.Type.Publicity;

            GameEvent.Condition pc_ = new GameEvent.Condition();
            pc_.value = 40;
            pc_.greater = true;
            pc_.type = GameEvent.Condition.Type.Publicity;

            gE.conditions = new GameEvent.Condition[] { pc, pc_ };

            Company c = new Company("Foo Inc").Init();
            c.publicity.baseValue = 0;

            Assert.IsFalse(gE.ConditionsSatisfied(c));

            c.publicity.baseValue = 30;
            Assert.IsFalse(gE.ConditionsSatisfied(c));

            c.publicity.baseValue = 50;
            Assert.IsTrue(gE.ConditionsSatisfied(c));
        }

        [Test]
        public void SpecialEvents() {
            GameEvent.Condition pc = new GameEvent.Condition();
            pc.value = 20;
            pc.greater = true;
            pc.type = GameEvent.Condition.Type.Publicity;
            gE.conditions = new GameEvent.Condition[] { pc };

            gd.specialEventsPool.Add(gE);
            gd.company = new Company("Foo Inc").Init();

            em.EvaluateSpecialEvents();

            // Our test listener to listen for and capture the event.
            TestEventListener eL = new TestEventListener();

            Assert.AreEqual(eL.triggeredEvent, null);
            Assert.IsTrue(gd.specialEventsPool.Contains(gE));

            gd.company.publicity.baseValue = 40;

            em.EvaluateSpecialEvents();

            Assert.AreEqual(eL.triggeredEvent, gE);
            Assert.IsFalse(gd.specialEventsPool.Contains(gE));
        }
    }
}
