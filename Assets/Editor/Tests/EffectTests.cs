using UnityEngine;
using System;
using System.Linq;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;

namespace UnityTest
{
    [TestFixture]
    internal class EffectTests
    {
        private GameObject gameObj;
        private GameData gd;
        private GameManager gm;

        private Company c;

        [SetUp]
        public void SetUp() {
            gameObj = new GameObject("Game Manager");
            gm = gameObj.AddComponent<GameManager>();
            gd = GameData.New("DEFAULTCORP");
            gm.Load(gd);

            c = new Company("Foo Inc").Init();
        }

        [TearDown]
        public void TearDown() {
            UnityEngine.Object.DestroyImmediate(gameObj);
            gm = null;
            c = null;
        }

        [Test]
        public void Cash() {
            CashEffect e = new CashEffect();
            e.cash = 2000;

            float start = c.cash.baseValue;
            e.Apply(c);
            Assert.AreEqual(c.cash.baseValue, start + e.cash);
        }

        [Test]
        public void Research() {
            ResearchEffect e = new ResearchEffect();
            e.buff.value = 2000;

            float start = c.research.value;
            e.Apply(c);
            Assert.AreEqual(c.research.value, start + e.buff.value);
        }

        [Test]
        public void Opinion() {
            OpinionEffect e = new OpinionEffect();
            e.opinionEvent.opinion.value   = 2000;
            e.opinionEvent.publicity.value = 4000;

            float start_o = c.opinion.value;
            float start_p = c.publicity.value;
            e.Apply(c);
            Assert.AreEqual(c.opinion.value,   start_o + e.opinionEvent.opinion.value);
            Assert.AreEqual(c.publicity.value, start_p + e.opinionEvent.publicity.value);
        }

        [Test]
        public void Event() {
            EventEffect e = new EventEffect();
            GameEvent ev  = ScriptableObject.CreateInstance<GameEvent>();
            ev.name       = "TESTEVENT";
            e.gameEvent   = ev;
            e.delay       = 2000;
            e.probability = 0.5f;

            e.Apply(c);
            GameEvent ev_ = gd.eventsPool.Where(evn => evn.name == ev.name).First();
            Assert.AreEqual(ev_.name, ev.name);
            Assert.AreEqual(ev_.delay, e.delay);
            Assert.AreEqual(ev_.probability, e.probability);
        }

        [Test]
        public void Worker() {
            WorkerEffect e = new WorkerEffect();
            e.buff = new StatBuff("Happiness", 100);

            Worker w = ScriptableObject.CreateInstance<Worker>().Init("TESTWORKER");
            c.HireWorker(w);
            Assert.IsTrue(c.workers.Contains(w));

            float start = w.happiness.value;

            e.Apply(c);
            Assert.AreEqual(w.happiness.value, start + e.buff.value);
        }

        [Test]
        public void Product() {
            ProductEffect e = new ProductEffect();
            e.buff = new StatBuff("Design", 100);

            ProductType pt = ScriptableObject.CreateInstance<ProductType>();
            List<ProductType> pts = new List<ProductType>() { pt };

            c.StartNewProduct(pts, 0, 0, 0);
            Product p = c.products[0];
            p.requiredProgress = 0;

            float start = p.design.value;

            e.Apply(c);

            // Have to manually add it to active effects,
            // this is normally handled by the EffectSet the effect is a part of.
            c.activeEffects.Add(e);

            // The effect should not apply until the product is launched.
            Assert.IsTrue(p.developing);
            Assert.AreEqual(start, p.design.value);

            c.DevelopProduct(p);

            // The effect should after the product is launched.
            Assert.IsTrue(p.launched);
            Assert.AreEqual(p.design.value, start + e.buff.value);

            // Test applying the effect to an already launched product.
            e = new ProductEffect();
            e.buff = new StatBuff("Engineering", 100);

            start = p.engineering.value;

            e.Apply(c);
            Assert.AreEqual(p.engineering.value, start + e.buff.value);
        }
    }
}