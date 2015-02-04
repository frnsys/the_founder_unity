using UnityEngine;
using UnityEditor;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityTest
{
	[TestFixture]
	internal class PromoTests
	{
		[Test]
		public void Result_Difficulty() {
            Promo p = ScriptableObject.CreateInstance<Promo>();

            p.difficulty = 1;
            p.opinionEvent.opinion.value = 4;
            p.opinionEvent.publicity.value = 4;
            float creativity = 10;

            OpinionEvent result = p.CalculateResult(creativity);

            p.difficulty = 100;
            Assert.IsTrue(result.opinion.value > p.CalculateResult(creativity).opinion.value);

            p.difficulty = 0.0001f;
            Assert.IsTrue(result.opinion.value < p.CalculateResult(creativity).opinion.value);
		}
    }
}
