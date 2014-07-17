using UnityEngine;
using System;
using System.Threading;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

namespace UnityTest
{
	[TestFixture]
	internal class ActionTests
	{
        private EventAction ea = null;
        private GameEvent outcome = null;

        [SetUp]
        public void SetUp() {
            outcome = new GameEvent("Some event", 1f, false);
            List<GameEvent> outcomes = new List<GameEvent>() {
                outcome
            };
            ea = new EventAction("Some action", outcomes, 0);
        }

        [TearDown]
        public void TearDown() {
            ea = null;
            outcome = null;
        }

		[Test]
		public void ActionConstructor()
		{
            Assert.IsNotNull(ea);
            Assert.AreEqual(ea.name, "Some action");
		}

        [Test]
        public void ActionResolveImmediately()
        {
            ea.delay = 0;

            // Our test listener to listen for and capture the event.
            TestEventListener eL = new TestEventListener();

            ea.Execute();

            // Check if the event was captured.
            Assert.AreEqual(eL.triggeredEvent, outcome);
        }

        [Test]
        public void ActionResolveDelayed()
        {
            ea.delay = 100;

            // Our test listener to listen for and capture the event.
            TestEventListener eL = new TestEventListener();

            ea.Execute();

            System.Threading.Thread.Sleep(50);
            Assert.AreEqual(eL.triggeredEvent, null);

            System.Threading.Thread.Sleep(60);
            // Check if the event was captured.
            Assert.AreEqual(eL.triggeredEvent, outcome);
        }
    }
}
