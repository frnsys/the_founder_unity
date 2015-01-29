using UnityEngine;
using UnityEditor;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace UnityTest
{
	[TestFixture]
	internal class CompanyTests
	{
        private GameObject gameObj;
        private GameData gd;
        private GameManager gm;

        private Company c;
        private Worker worker;
        private Item item;
        private Perk perk;
        private Location startLoc;

        private List<ProductType> pts;

        [SetUp]
        public void SetUp() {
            gameObj = new GameObject("Game Manager");
            gm = gameObj.AddComponent<GameManager>();
            gd = GameData.New("DEFAULTCORP");
            gm.Load(gd);

            c = new Company("Foo Inc").Init();

            // Create a starting location with some infrastructure capacity.
            startLoc = ScriptableObject.CreateInstance<Location>();
            startLoc.cost = 0;
            startLoc.capacity = new Infrastructure();
            startLoc.capacity[Infrastructure.Type.Datacenter] = 1;
            startLoc.capacity[Infrastructure.Type.Factory]    = 1;
            c.ExpandToLocation(startLoc);

            pts = new List<ProductType>() {
                ProductType.Load("Social Network")
            };

            worker = ScriptableObject.CreateInstance<Worker>();
            worker.Init("Franklin");

            item = (Item)GameObject.Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Editor/Tests/Resources/TestItem.asset", typeof(Item)));
            perk = (Perk)GameObject.Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Editor/Tests/Resources/TestPerk.asset", typeof(Perk)));
        }

        [TearDown]
        public void TearDown() {
            UnityEngine.Object.DestroyImmediate(gameObj);
            gm = null;
            worker = null;
            c = null;
            item = null;
        }

		[Test]
		public void CompanyConstructor() {
            Assert.IsNotNull(c);

            // Creates a name.
            Assert.AreEqual(c.name, "Foo Inc");
		}

		[Test]
		public void ManageWorkers() {
          Assert.AreEqual(c.workers.Count, 0);

          worker.creativity.baseValue = 20;

          // Prepare a product to check that hiring/firing workers automatically updates
          // progress required for developing products.
          ProductType pt = ScriptableObject.CreateInstance<ProductType>();
          pt.difficulty = 1;
          c.StartNewProduct(new List<ProductType> { pt }, 0, 0, 0);
          Product p = c.products[0];

          float requiredProgress = p.TotalProgressRequired(c);
          Assert.AreEqual(p.requiredProgress, requiredProgress);

          c.baseSizeLimit = 0;
          c.HireWorker(worker);
          Assert.AreEqual(c.workers.Count, 0);

          c.cash.baseValue = 2000;
          c.BuyItem(item);
          c.baseSizeLimit = 10;
          c.HireWorker(worker);
          Assert.AreEqual(c.workers.Count, 1);
          Assert.AreEqual(c.workers[0].happiness.value, 10);

          // The total progress required should be different now,
          // and it should be reflected on the product.
          // As a reminder, we don't constantly calculate TotalProgressRequired because
          // it is kind of expensive, so we only recalc it when a worker is hired or fired.
          float newRequiredProgress = p.TotalProgressRequired(c);
          Assert.AreNotEqual(requiredProgress, newRequiredProgress);
          Assert.AreEqual(p.requiredProgress, newRequiredProgress);

          worker.salary = 2000;
          c.FireWorker(worker);
          Assert.AreEqual(c.workers.Count, 0);
          Assert.AreEqual(worker.salary, 0);

          // Firing the worker should have brought the required progress back to the previous value.
          Assert.AreEqual(p.requiredProgress, requiredProgress);
      }

      [Test]
      public void ResearchCzar() {
          worker = ScriptableObject.CreateInstance<Worker>();
          worker.Init("Researcher");
          worker.cleverness.baseValue = 10000;

          Assert.AreNotEqual(worker.cleverness.value, c.research.baseValue);

          c.HireWorker(worker);
          Assert.AreEqual(c.workers.Count, 1);

          Assert.IsTrue(c.allWorkers.Contains(worker));

          c.ResearchCzar = worker;

          // Company's base research should be the research czar's cleverness.
          Assert.AreEqual(worker.cleverness.value, c.research.baseValue);

          // The research czar should not be counted among "all workers".
          Assert.IsFalse(c.allWorkers.Contains(worker));
      }

      [Test]
      public void OpinionCzar() {
          worker = ScriptableObject.CreateInstance<Worker>();
          worker.Init("Opinioner");
          worker.charisma.baseValue = 10000;

          Assert.AreNotEqual(worker.charisma.value, c.opinion.baseValue);

          c.HireWorker(worker);
          Assert.AreEqual(c.workers.Count, 1);

          Assert.IsTrue(c.allWorkers.Contains(worker));

          c.OpinionCzar = worker;

          // Company's base opinion should be the opinion czar's charisma.
          Assert.AreEqual(worker.charisma.value, c.opinion.baseValue);

          // The opinion czar should not be counted among "all workers".
          Assert.IsFalse(c.allWorkers.Contains(worker));
      }

		[Test]
		public void AggregateWorkerStats() {
            Founder founder = ScriptableObject.CreateInstance<Founder>();
            founder.Init("Jobs");
            founder.charisma.baseValue = 10;
            founder.creativity.baseValue = 20;
            founder.cleverness.baseValue = 30;
            founder.happiness.baseValue = 40;
            founder.productivity.baseValue = 50;
            c.founders.Add(founder);

            worker.charisma.baseValue = 1;
            worker.creativity.baseValue = 2;
            worker.cleverness.baseValue = 3;
            worker.happiness.baseValue = 4;
            worker.productivity.baseValue = 5;
            c.HireWorker(worker);

            Assert.AreEqual(c.AggregateWorkerStat("Charisma"), 11);
            Assert.AreEqual(c.AggregateWorkerStat("Creativity"), 22);
            Assert.AreEqual(c.AggregateWorkerStat("Cleverness"), 33);
            Assert.AreEqual(c.AggregateWorkerStat("Happiness"), 44);
            Assert.AreEqual(c.AggregateWorkerStat("Productivity"), 55);
        }

		[Test]
        public void ExpandToVertical() {
            c.cash.baseValue = 0;
            Vertical vert = ScriptableObject.CreateInstance<Vertical>();
            vert.cost = 2000;

            Assert.IsFalse(c.ExpandToVertical(vert));

            c.cash.baseValue = 2000;
            Assert.IsTrue(c.ExpandToVertical(vert));
            Assert.IsTrue(c.verticals.Contains(vert));
        }

		[Test]
        public void ExpandToLocation() {
            c.cash.baseValue = 0;
            Location loc = ScriptableObject.CreateInstance<Location>();
            loc.cost = 2000;
            loc.market = MarketManager.Market.Antarctica;

            EffectSet es = new EffectSet();
            es.cash = 5000;
            loc.effects = es;

            Assert.IsFalse(c.ExpandToLocation(loc));

            c.cash.baseValue = 2000;
            Assert.IsTrue(c.ExpandToLocation(loc));
            Assert.IsTrue(c.locations.Select(l => l.name).Contains(loc.name));
            Assert.IsTrue(c.markets.Contains(loc.market));
            Assert.AreEqual(c.cash.value, 5000);
        }

		[Test]
		public void PayMonthly() {
            // For this test to work, startCash has to be enough for all these purchases.
            // Otherwise the purchases don't go through.
            float startCash = 200000;
            c.cash.baseValue = startCash;

            Infrastructure i = new Infrastructure();
            i[Infrastructure.Type.Datacenter] = 1;
            c.BuyInfrastructure(i, startLoc);

            Location loc = ScriptableObject.CreateInstance<Location>();
            loc.cost = 100;
            c.ExpandToLocation(loc);

            worker.salary = 500;

            // Location rent is calculated twice because it's paid on purchase, and then again as monthly rent.
            // Same for infrastructure and worker salary.
            float paid = worker.hiringFee + worker.monthlyPay + c.researchInvestment + i.cost + i.cost + loc.cost + loc.cost;

            c.HireWorker(worker);

            c.PayMonthly();
            Assert.AreEqual(c.cash.baseValue, startCash - paid);
        }

        [Test]
        public void ManageInfrastructure() {
            int baseDatacenterCapacity = startLoc.capacity[Infrastructure.Type.Datacenter];
            Infrastructure zeroInf = new Infrastructure();

            // All capacity should be available.
            // No infrastructure is currently being used.
            Assert.IsTrue(c.availableInfrastructureCapacity.Equals(c.infrastructureCapacity));
            Assert.IsTrue(c.usedInfrastructure.Equals(zeroInf));

            Infrastructure i = new Infrastructure();
            i[Infrastructure.Type.Datacenter] = baseDatacenterCapacity + 1;

            Location newLoc = ScriptableObject.CreateInstance<Location>();
            newLoc.name = "Belize";
            newLoc.cost = 0;
            newLoc.capacity = new Infrastructure();
            newLoc.capacity[Infrastructure.Type.Datacenter] = 1;
            c.ExpandToLocation(newLoc);

            // Can't buy the infrastructure, not enough cash.
            c.cash.baseValue = 0;
            Assert.IsFalse(c.BuyInfrastructure(i, newLoc));

            // Can't buy the infrastructure, not enough capacity.
            c.cash.baseValue = i.cost;
            Assert.IsFalse(c.BuyInfrastructure(i, newLoc));

            // Can buy the infrastructure: enough cash + capacity.
            i[Infrastructure.Type.Datacenter] = 1;
            Assert.IsTrue(c.BuyInfrastructure(i, newLoc));

            // The available capacity should now be less than the total capacity.
            Assert.IsFalse(c.availableInfrastructureCapacity.Equals(c.infrastructureCapacity));

            // The entireity of the available infrastructure should only be the one set we added.
            // All of the company's infrastructure should be available.
            // None of it is being used.
            Assert.IsTrue(c.availableInfrastructure.Equals(i));
            Assert.IsTrue(c.availableInfrastructure.Equals(c.infrastructure));
            Assert.IsTrue(c.usedInfrastructure.Equals(zeroInf));

            ProductType pt = ScriptableObject.CreateInstance<ProductType>();
            Infrastructure required = new Infrastructure();
            required[Infrastructure.Type.Datacenter] = baseDatacenterCapacity;
            pt.requiredInfrastructure = required;
            c.StartNewProduct(new List<ProductType> { pt }, 0, 0, 0);

            // All infrastructure is being used now,
            // so none of it is available.
            // The amount used should equal the amount the product needed.
            Assert.IsFalse(c.usedInfrastructure.Equals(zeroInf));
            Assert.IsTrue(c.availableInfrastructure.Equals(zeroInf));
            Assert.AreEqual(c.usedInfrastructure[Infrastructure.Type.Datacenter], baseDatacenterCapacity);

            c.ShutdownProduct(c.products[0]);

            // Now that the product is retired, it shouldn't count towards used infrastructure.
            Assert.IsTrue(c.usedInfrastructure.Equals(zeroInf));
            Assert.IsTrue(c.availableInfrastructure.Equals(c.infrastructure));

            int oldDatacenterCapacity = c.infrastructureCapacity[Infrastructure.Type.Datacenter];

            Location loc = ScriptableObject.CreateInstance<Location>();
            Infrastructure cap = new Infrastructure();
            cap[Infrastructure.Type.Datacenter] = 10;
            loc.capacity = cap;
            loc.cost = 0;
            c.ExpandToLocation(loc);

            // Adding a location should have increased the capacity by the amount the location gives.
            Assert.AreEqual(c.infrastructureCapacity[Infrastructure.Type.Datacenter], oldDatacenterCapacity + 10);

            c.DestroyInfrastructure(i, newLoc);

            // Destroying the infrastructure we had should have cleared up all capacity.
            // No infrastructure should be available now.
            Assert.IsTrue(c.availableInfrastructureCapacity.Equals(c.infrastructureCapacity));
            Assert.IsTrue(c.availableInfrastructure.Equals(zeroInf));
        }

        [Test]
        public void ActiveEffects() {
            Assert.IsTrue(c.BuyItem(item));

            int last = c.activeEffects.Count - 1;
            Assert.IsTrue(c.activeEffects[last].Equals(item.effects));
        }


        // ===============================================
        // Product Management ============================
        // ===============================================

        [Test]
        public void StartNewProduct() {
            c.cash.baseValue = 2000;
            c.BuyItem(item);

            Infrastructure i = new Infrastructure();
            i[Infrastructure.Type.Datacenter] = 1;
            i[Infrastructure.Type.Factory] = 1;
            c.BuyInfrastructure(i, startLoc);

            c.StartNewProduct(pts, 0, 0, 0);
            Product p = c.products[0];
            Assert.AreEqual(c.developingProducts[0], p);

            // Assure the infrastructure is properly used up.
            Assert.AreEqual(c.availableInfrastructure, c.infrastructure - pts[0].requiredInfrastructure);

            // Assure the progress is properly set when the company starts the product.
            Assert.AreEqual(p.requiredProgress, p.TotalProgressRequired(c));

            Assert.AreEqual(c.products.Count, 1);

            // Creating a new product should not apply existing items.
            Assert.AreEqual(p.design.value, 0);
        }

		[Test]
		public void DevelopProduct() {
            c.HireWorker(worker);

            worker.productivity.baseValue = 10;
            worker.charisma.baseValue = 10;
            worker.creativity.baseValue = 10;
            worker.cleverness.baseValue = 10;

            Product p = ScriptableObject.CreateInstance<Product>();
            p.Init(pts, 0, 0, 0, c);
            c.DevelopProduct(p);

            Assert.IsTrue(p.progress > 0);
        }

		[Test]
		public void HarvestProduct() {
            c.cash.baseValue = 2000;
            c.HireWorker(worker);

            worker.productivity.baseValue = 100;
            worker.charisma.baseValue = 100;
            worker.creativity.baseValue = 100;
            worker.cleverness.baseValue = 100;

            c.StartNewProduct(pts, 0, 0, 0);
            Product p = c.products[0];
            c.DevelopProduct(p);

            p.Launch();
            Assert.AreEqual(c.activeProducts[0], p);

            // Fake the product's market share:
            p.marketShare = 1;

            c.HarvestProducts(2);

            Assert.IsTrue(c.cash.baseValue > 2000);
        }

        [Test]
        public void ShutdownProduct() {
            c.cash.baseValue = 2000;
            c.BuyItem(item);

            c.StartNewProduct(pts, 0, 0, 0);
            Product p = c.products[0];
            p.requiredProgress = 0;
            c.DevelopProduct(p);
            Assert.AreEqual(c.activeProducts[0], p);
            Assert.AreEqual(p.design.value, 10);

            c.ShutdownProduct(p);

            Assert.AreEqual(p.state, Product.State.RETIRED);
            Assert.AreEqual(p.design.value, 0);
            Assert.AreEqual(c.availableInfrastructure, c.infrastructure);
        }

        // ===============================================
        // Perk Management ===============================
        // ===============================================

		[Test]
		public void BuyPerk_CanAfford() {
            c.cash.baseValue = perk.cost;
            float startCash = c.cash.value;
            float startResearch = c.research.value;


            Assert.IsTrue(c.BuyPerk(perk));

            Assert.AreEqual(c.cash.baseValue, startCash - perk.cost);
            Assert.AreEqual(c.perks.Count, 1);
            Assert.AreEqual(c.perks[0].upgradeLevel, 0);
            Assert.AreEqual(c.research.value, startResearch + perk.effects.research.value);
        }

		[Test]
		public void UpgradePerk_CanAfford() {
            c.cash.baseValue = perk.cost;
            Assert.IsTrue(c.BuyPerk(perk));

            c.cash.baseValue = perk.next.cost;
            float startCash = c.cash.value;
            float startResearch = c.research.value;
            Assert.IsTrue(c.UpgradePerk(perk));
            Assert.AreEqual(c.perks.Count, 1);
            Assert.AreEqual(c.perks[0].upgradeLevel, 1);
            Assert.AreEqual(c.research.value, startResearch + perk.effects.research.value);
        }

        // ===============================================
        // Item Management ===============================
        // ===============================================

		[Test]
		public void BuyItem_CanAfford() {
            c.cash.baseValue = 2000;
            c.StartNewProduct(pts, 0, 0, 0);
            Product p = c.products[0];
            p.Launch();

            c.HireWorker(worker);

            Assert.IsTrue(c.BuyItem(item));
            Assert.AreEqual(c.cash.baseValue, 1500);
            Assert.AreEqual(c.items.Count, 1);
            Assert.AreEqual(p.design.value, 10);
            Assert.AreEqual(worker.happiness.value, 10);
            Assert.AreEqual(worker.productivity.value, 20);

            // Item should be removed from worker.
            c.FireWorker(worker);
            Assert.AreEqual(worker.happiness.value, 0);
            Assert.AreEqual(worker.productivity.value, 0);
        }

		[Test]
		public void BuyItem_CannotAfford() {
            c.cash.baseValue = 200;
            Assert.IsFalse(c.BuyItem(item));
            Assert.AreEqual(c.cash.baseValue, 200);
        }

        [Test]
        public void RemoveItem() {
            c.cash.baseValue = 2000;
            c.StartNewProduct(pts, 0, 0, 0);
            Product p = c.products[0];
            c.HireWorker(worker);

            c.BuyItem(item);
            c.RemoveItem(item);

            Assert.AreEqual(c.items.Count, 0);
            Assert.AreEqual(p.design.value, 0);
            Assert.AreEqual(worker.happiness.value, 0);
        }

        [Test]
        public void OpinionEvents() {
            c.opinion.baseValue   = 200;
            c.publicity.baseValue = 100;
            c.forgettingRate      = 10;

            EffectSet es = new EffectSet();
            es.opinionEvent = new OpinionEvent(100, 400);
            es.Apply(c);

            Assert.AreEqual(c.opinion.value, 300);
            Assert.AreEqual(c.publicity.value, 500);

            c.ForgetOpinionEvents();

            Assert.AreEqual(c.opinion.value, 290);
            Assert.AreEqual(c.publicity.value, 500);
        }
    }
}
