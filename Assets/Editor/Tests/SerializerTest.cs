using UnityEngine;
using UnityEditor;
using System.Threading;
using NUnit.Framework;

namespace UnityTest
{
	[TestFixture]
	internal class SerializerTests
	{
        private GameData data = null;

        [SetUp]
        public void SetUp() {
            data = AssetDatabase.LoadAssetAtPath("Assets/Editor/Tests/Resources/TestingGameData.asset", typeof(GameData)) as GameData;

            // Initialize new game stuff.
            data.company  = new Company("TESTINGCORP");
                data.company.founders.Add(CreateFounder("STEVE", 100));
                data.company.phase            = Company.Phase.Planetary;
                data.company.sizeLimit        = 17;
                data.company.lastMonthRevenue = 28517;
                data.company.lastMonthCosts   = 14789;
                data.company.cash.baseValue   = 100000000;
                data.company.consultancy      = ScriptableObject.CreateInstance<Consultancy>();
                    data.company.consultancy.name     = "RADCONSULTANTS";
                    data.company.consultancy.cost     = 145814;
                    data.company.consultancy.research = new Research(8,9,7);

                for (int i=0;i<5;i++) {
                    Worker worker = CreateWorker("WORKER"+i, i*10);
                    data.company.HireWorker(worker);
                }

                for (int i=0;i<5;i++) {
                    data.company.products.Add(CreateProduct());
                }

                Item item = AssetDatabase.LoadAssetAtPath("Assets/Editor/Tests/Resources/TestItem.asset", typeof(Item)) as Item;
                data.company.BuyItem(item);

            data.board    = new TheBoard();
                data.board.happiness = 20;

            data.research  = new Research(11,12,13);
            data.discovery = ScriptableObject.CreateInstance<Discovery>();
                data.discovery.name             = "my discovery";
                data.discovery.description      = "foobar";
                data.discovery.requiredResearch = new Research(4,5,6);

            // TO DO AI companies and unlock set

            data.month         = Month.March;
            data.year          = 14;
            data.week          = 3;
            data.lifetimeYear  = 100;
            data.lifetimeMonth = 6;
            data.lifetimeWeek  = 1;
        }

        [TearDown]
        public void TearDown() {
            data = null;
        }

		[Test]
		public void TestSerialize() {
            //Serializer.Serialized s = Serializer.Serialize(data);
            //GameData gd = Serializer.Deserialize<GameData>(s);

            // Save and re-load the data: check to make sure its consistent.
            string filepath = "/tmp/the_founder_test_saving.dat";
            GameData.Save(data, filepath);
            GameData gd = GameData.Load(filepath);


            // Everything should be *identical*.
            // This is maybe... overkill ~
            Assert.AreEqual(gd.month,                      data.month);
            Assert.AreEqual(gd.year,                       data.year);
            Assert.AreEqual(gd.week,                       data.week);

            Assert.AreEqual(gd.lifetimeYear,               data.lifetimeYear);
            Assert.AreEqual(gd.lifetimeMonth,              data.lifetimeMonth);
            Assert.AreEqual(gd.lifetimeWeek,               data.lifetimeWeek);

            Assert.AreEqual(gd.company.name,               data.company.name);
            Assert.AreEqual(gd.company.phase,              data.company.phase);
            Assert.AreEqual(gd.company.cash.value,         data.company.cash.value);
            Assert.AreEqual(gd.company.sizeLimit,          data.company.sizeLimit);
            Assert.AreEqual(gd.company.productPoints,      data.company.productPoints);
            Assert.AreEqual(gd.company.lastMonthRevenue,   data.company.lastMonthRevenue);
            Assert.AreEqual(gd.company.lastMonthCosts,     data.company.lastMonthCosts);
            Assert.IsTrue  (gd.company.featurePoints   ==  data.company.featurePoints);
            Assert.AreEqual(gd.company.lastMonthCosts,     data.company.lastMonthCosts);

            Assert.AreEqual(gd.company.consultancy.name,   data.company.consultancy.name);
            Assert.AreEqual(gd.company.consultancy.cost,   data.company.consultancy.cost);
            Assert.IsTrue(gd.company.consultancy.research == data.company.consultancy.research);

            Assert.AreEqual(gd.company.workers.Count,      data.company.workers.Count);
            for (int i=0;i<gd.company.workers.Count;i++) {
                Worker w  = data.company.workers[i];
                Worker w_ = gd.company.workers[i];

                Assert.AreEqual(w.name,                     w_.name);
                Assert.AreEqual(w.salary,                   w_.salary);
                Assert.AreEqual(w.happiness.baseValue,      w_.happiness.baseValue);
                Assert.AreEqual(w.productivity.baseValue,   w_.productivity.baseValue);
                Assert.AreEqual(w.charisma.baseValue,       w_.charisma.baseValue);
                Assert.AreEqual(w.creativity.baseValue,     w_.creativity.baseValue);
                Assert.AreEqual(w.creativity.baseValue,     w_.creativity.baseValue);
            }

            Assert.AreEqual(gd.company.products.Count, data.company.products.Count);
            for (int i=0;i<gd.company.products.Count;i++) {
                Product p  = data.company.products[i];
                Product p_ = gd.company.products[i];

                Assert.AreEqual(p.name,               p_.name);
                Assert.AreEqual(p.progress,           p_.progress);
                Assert.AreEqual(p.state,              p_.state);
                Assert.AreEqual(p.timeSinceLaunch,    p_.timeSinceLaunch);
                Assert.AreEqual(p.revenueEarned,      p_.revenueEarned);
                Assert.AreEqual(p.lastRevenue,        p_.lastRevenue);
                Assert.AreEqual(p.points,             p_.points);

                Assert.AreEqual(p.appeal.value,       p_.appeal.value);
                Assert.AreEqual(p.usability.value,    p_.usability.value);
                Assert.AreEqual(p.performance.value,  p_.performance.value);

                Assert.AreEqual(p.productType.name,   p_.productType.name);
                Assert.AreEqual(p.industry.name,      p_.industry.name);
                Assert.AreEqual(p.market.name,        p_.market.name);
            }

            Assert.AreEqual(gd.company.founders.Count, data.company.founders.Count);
            for (int i=0;i<gd.company.founders.Count;i++) {
                Founder f  = data.company.founders[i];
                Founder f_ = gd.company.founders[i];

                Assert.AreEqual(f.name,                     f_.name);
                Assert.AreEqual(f.salary,                   f_.salary);
                Assert.AreEqual(f.happiness.baseValue,      f_.happiness.baseValue);
                Assert.AreEqual(f.productivity.baseValue,   f_.productivity.baseValue);
                Assert.AreEqual(f.charisma.baseValue,       f_.charisma.baseValue);
                Assert.AreEqual(f.creativity.baseValue,     f_.creativity.baseValue);
                Assert.AreEqual(f.creativity.baseValue,     f_.creativity.baseValue);

                // TO DO check that bonuses are there.
            }

            Assert.AreEqual(gd.company.items.Count, data.company.items.Count);
            for (int i=0;i<gd.company.items.Count;i++) {
                Item m  = data.company.items[i];
                Item m_ = gd.company.items[i];

                Assert.AreEqual(m.name,                     m_.name);
                Assert.AreEqual(m.cost,                     m_.cost);
                Assert.AreEqual(m.description,              m_.description);
                Assert.AreEqual(m.duration,                 m_.duration);
                Assert.AreEqual(m.store,                    m_.store);

                // TO DO check that effects are there.
            }

            // NEED TO CHECK that references remain the same. E.g. if two companies have the same consultancy hired, that should be the same instance, not two different ones.

            Assert.AreEqual(gd.board.happiness,         data.board.happiness);

            Assert.AreEqual(gd.discovery.name,          data.discovery.name);
            Assert.AreEqual(gd.discovery.description,   data.discovery.description);
            Assert.IsTrue(gd.research == data.research);
            Assert.IsTrue(gd.discovery.requiredResearch == data.discovery.requiredResearch);

		}

        private Worker CreateWorker(string name, int stat) {
            Worker worker = ScriptableObject.CreateInstance<Worker>();
            worker.Init(name);
            worker.salary                 = stat;
            worker.happiness.baseValue    = stat;
            worker.productivity.baseValue = stat;
            worker.charisma.baseValue     = stat;
            worker.creativity.baseValue   = stat;
            worker.cleverness.baseValue   = stat;
            return worker;
        }

        private Founder CreateFounder(string name, int stat) {
            Founder founder = ScriptableObject.CreateInstance<Founder>();
            founder.Init(name);
            founder.salary                 = stat;
            founder.happiness.baseValue    = stat;
            founder.productivity.baseValue = stat;
            founder.charisma.baseValue     = stat;
            founder.creativity.baseValue   = stat;
            founder.cleverness.baseValue   = stat;
            founder.bonuses                = new EffectSet();
            return founder;
        }

        private Product CreateProduct() {
            ProductType pt = ProductType.Load("Social Network");
            Industry i     = Industry.Load("Space");
            Market m       = Market.Load("Millenials");

            Product product = ScriptableObject.CreateInstance<Product>();
            product.Init(pt, i, m);

            product.Develop(RandFloat(), RandFloat(), RandFloat(), RandFloat());

            float r = Random.Range(0,1);
            if (r > 0.33) {
                product.Launch();
                product.Revenue(5);
            } else if (r > 0.66) {
                product.Shutdown();
            }
            return product;
        }

        private float RandFloat() {
            return Random.Range(10, 50);
        }

    }
}
