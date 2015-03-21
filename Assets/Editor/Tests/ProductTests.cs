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
	internal class ProductTests
	{
        private GameObject gameObj;
        private GameData gd;
        private GameManager gm;

        private Product p = null;
        private ProductRecipe pr = null;
        private ProductType pt;
        private ProductType pt_;
        private List<ProductType> pts;
        private Company c;

        [SetUp]
        public void SetUp() {
            pt  = ScriptableObject.Instantiate(ProductType.Load("Social Network")) as ProductType;
            pt_ = ScriptableObject.Instantiate(ProductType.Load("Virtual Reality")) as ProductType;

            gameObj = new GameObject("Game Manager");
            gm = gameObj.AddComponent<GameManager>();
            gd = GameData.New("DEFAULTCORP");
            gm.Load(gd);

            c = gd.company;
            pts = new List<ProductType>() { pt, pt_ };

            Location startLoc = ScriptableObject.CreateInstance<Location>();
            startLoc.cost = 0;
            c.ExpandToLocation(startLoc);

            p = ScriptableObject.CreateInstance<Product>();
            p.Init(pts, 0, 0, 0, c);
            pr = ProductRecipe.LoadFromTypes(pts);
        }

        [TearDown]
        public void TearDown() {
            UnityEngine.Object.DestroyImmediate(gameObj);
            c = null;
            gm = null;
            p = null;
            pr = null;
            pt = null;
            pt_ = null;
            pts = null;
        }

		[Test]
		public void ProductConstructor() {
            Assert.IsNotNull(p);

            // Creates a name.
            Assert.AreNotEqual(p.name, "");
		}

		[Test]
		public void Develop() {
            Assert.AreEqual(p.state, Product.State.DEVELOPMENT);
            Assert.AreEqual(p.progress, 0);

            p.Develop(100000, c);

            Assert.AreEqual(p.progress, 100000/p.requiredProgress);
            Assert.AreEqual(p.state, Product.State.LAUNCHED);
        }

		[Test]
		public void Develop_Disabled() {
            Assert.AreEqual(p.state, Product.State.DEVELOPMENT);
            Assert.AreEqual(p.progress, 0);

            p.disabled = true;
            p.Develop(100000, c);

            Assert.AreEqual(p.progress, 0);
        }

		[Test]
		public void Revenue_NotLaunched() {
            Assert.AreNotEqual(p.state, Product.State.LAUNCHED);
            Assert.AreEqual(p.Revenue(10, c), 0);
        }

		[Test]
		public void Revenue_Launched() {
            p.design.baseValue = 100;
            p.marketing.baseValue = 100;
            p.engineering.baseValue = 100;

            p.Launch(c);

            Assert.IsTrue(p.Revenue(4, c) > 0);
        }

		[Test]
		public void MarketShare_PublicOpinion() {
            p.design.baseValue = 100;
            p.marketing.baseValue = 100;
            p.engineering.baseValue = 100;
            p.Launch(c);

            Product p_ = ScriptableObject.CreateInstance<Product>();
            p_.Init(pts, 100, 100, 100, c);
            c.opinion.baseValue = -1000;
            p_.Launch(c);

            Assert.IsTrue(p.marketShare > p_.marketShare);
        }

		[Test]
		public void MarketShare_MarketReach() {
            p.design.baseValue = 100;
            p.marketing.baseValue = 100;
            p.engineering.baseValue = 100;
            p.Launch(c);

            Product p_ = ScriptableObject.CreateInstance<Product>();
            p_.Init(pts, 100, 100, 100, c);

            Location newLoc = ScriptableObject.CreateInstance<Location>();
            newLoc.cost = 0;
            c.ExpandToLocation(newLoc);
            p_.Launch(c);

            Assert.IsTrue(p.marketShare < p_.marketShare);
        }

		[Test]
		public void MarketShare_TechPenalty() {
            p.design.baseValue = 100;
            p.marketing.baseValue = 100;
            p.engineering.baseValue = 100;
            p.Launch(c);

            Product p_ = ScriptableObject.CreateInstance<Product>();
            pr.requiredTechnologies.Add(ScriptableObject.CreateInstance<Technology>());
            p_.Init(pts, 100, 100, 100, c);
            p_.Launch(c);

            Assert.IsTrue(p.marketShare > p_.marketShare);
        }

		[Test]
		public void MarketShare_Hype() {
            p.design.baseValue = 100;
            p.marketing.baseValue = 100;
            p.engineering.baseValue = 100;
            p.Launch(c);

            Product p_ = ScriptableObject.CreateInstance<Product>();
            p_.Init(pts, 100, 100, 100, c);
            c.publicity.baseValue = 10;
            p_.Launch(c);

            Assert.IsTrue(p.marketShare < p_.marketShare);
        }

		[Test]
		public void Revenue_ConsumerSpending() {
            p.design.baseValue = 100;
            p.marketing.baseValue = 100;
            p.engineering.baseValue = 100;

            p.Launch(c);

            gm.spendingMultiplier = 1;
            float r = p.Revenue(4, c);

            gm.spendingMultiplier = 10;
            float r_ = p.Revenue(4, c);

            Assert.IsTrue(r_ > r);
        }

		[Test]
		public void Revenue_Disabled() {
            p.design.baseValue = 100;
            p.marketing.baseValue = 100;
            p.engineering.baseValue = 100;

            p.Launch(c);

            p.disabled = true;
            Assert.AreEqual(p.Revenue(4, c), 0);
        }

		[Test]
		public void Shutdown() {
            p.Shutdown();

            Assert.AreEqual(p.state, Product.State.RETIRED);
        }

        [Test]
        public void RequiredVerticals() {
            ProductType pt_  = ScriptableObject.CreateInstance<ProductType>();
            Vertical vert_   = ScriptableObject.CreateInstance<Vertical>();
            pt_.requiredVerticals = new List<Vertical>() { vert_ };

            ProductType pt__ = ScriptableObject.CreateInstance<ProductType>();
            Vertical vert__  = ScriptableObject.CreateInstance<Vertical>();
            pt__.requiredVerticals = new List<Vertical>() { vert__ };

            Product prod = ScriptableObject.CreateInstance<Product>();
            prod.Init( new List<ProductType> { pt_, pt__ }, 0, 0, 0, c);

            Assert.AreEqual(prod.requiredVerticals, new List<Vertical>() { vert_, vert__ });
        }

        [Test]
        public void RequiredInfrastructure() {
            ProductType pt_   = ScriptableObject.CreateInstance<ProductType>();
            Infrastructure i_ = new Infrastructure();
            i_[Infrastructure.Type.Datacenter] = 10;
            pt_.requiredInfrastructure = i_;

            ProductType pt__   = ScriptableObject.CreateInstance<ProductType>();
            Infrastructure i__ = new Infrastructure();
            i__[Infrastructure.Type.Studio] = 3;
            pt__.requiredInfrastructure = i__;

            Product prod = ScriptableObject.CreateInstance<Product>();
            prod.Init( new List<ProductType> { pt_, pt__ }, 0, 0, 0, c);

            Infrastructure i = new Infrastructure();
            i[Infrastructure.Type.Datacenter] = 10;
            i[Infrastructure.Type.Studio] = 3;
            Assert.IsTrue(prod.requiredInfrastructure.Equals(i));
        }

        [Test]
        public void ProductEffectsApplyOnlyOnce() {
            EffectSet e = new EffectSet();
            e.research = new StatBuff("Research", 2000);
            pr.effects = e;
            float start = c.research.value;

            Product.Completed += gm.OnProductCompleted;

            // Complete the project so that the effects are applied.
            c.StartNewProduct(pts, 0, 0, 0);
            c.products[0].Develop(100000, c);
            Assert.AreEqual(c.products[0].state, Product.State.LAUNCHED);
            Assert.AreEqual(c.research.value, start + e.research.value);

            // Complete a duplicate product.
            c.StartNewProduct(pts, 0, 0, 0);
            c.products[1].Develop(100000, c);
            Assert.AreEqual(c.products[1].state, Product.State.LAUNCHED);

            // Effect should have only been applied once.
            Assert.AreEqual(c.research.value, start + e.research.value);
        }
    }
}
