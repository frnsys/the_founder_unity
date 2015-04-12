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
        private GameManager gm;

        private GameEvent gE = null;

        [SetUp]
        public void SetUp() {
            gE = ScriptableObject.CreateInstance<GameEvent>();
            gE.name = "Some event";

            gameObj = new GameObject("Game Manager");
            gm = gameObj.AddComponent<GameManager>();
            gd = GameData.New("DEFAULTCORP");
            gm.Load(gd);
            em = gm.eventManager;
        }

        [TearDown]
        public void TearDown() {
            UnityEngine.Object.DestroyImmediate(gameObj);
            gE = null;
            em = null;
            gm = null;
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
            AGameEvent aGe = new AGameEvent(gE);
            aGe.delay = 10;
            em.Add(aGe);

            em.Tick();

            Assert.AreEqual(aGe.countdown, 9);
        }

        [Test]
        public void TickResolve() {
            AGameEvent aGe = new AGameEvent(gE);
            aGe.delay = 1;
            aGe.probability = 1;
            em.Add(aGe);

            // Our test listener to listen for and capture the event.
            TestEventListener eL = new TestEventListener();

            em.Tick();

            Assert.AreEqual(eL.triggeredEvent, gE);
            Assert.IsFalse(gd.eventsPool.Contains(aGe));
        }

        [Test]
        public void TickResolveMultiple() {
            AGameEvent aGe = new AGameEvent(gE);
            aGe.delay = 1;
            aGe.probability = 1;
            em.Add(aGe);

            GameEvent gE_ = ScriptableObject.CreateInstance<GameEvent>();
            gE_.name = "Some event";
            AGameEvent aGe_ = new AGameEvent(gE_);
            aGe_.probability = 1f;
            aGe_.delay = 1;
            em.Add(aGe_);

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

            gE.conditions = new List<GameEvent.Condition>() { pc, pc_ };

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
            gE.conditions = new List<GameEvent.Condition>() { pc };

            AGameEvent aGe = new AGameEvent(gE);
            gd.specialEventsPool.Add(aGe);
            gd.company = new Company("Foo Inc").Init();

            em.EvaluateSpecialEvents();
            Assert.IsTrue(gd.specialEventsPool.Contains(aGe));

            // Note we don't test that the event has triggered
            // because there is a 45s delay.
            // But the event is removed before it is triggered
            // so that it doesn't trigger multiple times.
            gd.company.publicity.baseValue = 40;
            em.EvaluateSpecialEvents();
            Assert.IsFalse(gd.specialEventsPool.Contains(aGe));

        }
    }
}
