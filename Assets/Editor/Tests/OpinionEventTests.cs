using UnityEngine;
using UnityEditor;
using System;
using System.Threading;
using NUnit.Framework;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace UnityTest
{
	[TestFixture]
	internal class OpinionEventTests
	{
        [SetUp]
        public void SetUp() {
        }

        [TearDown]
        public void TearDown() {
        }

		[Test]
		public void ForgettingPositive() {
            OpinionEvent oe = new OpinionEvent(10, 0);
            oe.Forget(1);
            Assert.AreEqual(oe.opinion.value, 9);

            oe.Forget(100);
            Assert.AreEqual(oe.opinion.value, 0);
		}

		[Test]
		public void ForgettingNegative() {
            OpinionEvent oe = new OpinionEvent(-10, 0);
            oe.Forget(1);
            Assert.AreEqual(oe.opinion.value, -9);

            oe.Forget(100);
            Assert.AreEqual(oe.opinion.value, 0);
		}

    }
}
