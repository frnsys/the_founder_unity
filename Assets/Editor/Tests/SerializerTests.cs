using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

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
                data.company.baseSizeLimit      = 17;
                data.company.lastMonthRevenue   = 28517;
                data.company.lastMonthCosts     = 14789;
                data.company.cash.baseValue     = 100000000;
                data.company.research.baseValue = 1000;

                for (int i=0;i<5;i++) {
                    Worker worker = CreateWorker("WORKER"+i, i*10);
                    data.company.HireWorker(worker);
                }

                for (int i=0;i<5;i++) {
                    data.company.products.Add(CreateProduct());
                }

                Item item = CreateItem();
                data.company.BuyItem(item);

            data.board    = new TheBoard();
                data.board.happiness = 20;

            data.research  = 500;
            data.technology = Technology.Load("3D Printing");

            // TO DO AI companies
            data.unlocked = new UnlockSet();
                ProductType pt = ProductType.Load("Social Network");
                Vertical vert  = Vertical.Load("Information");
                data.unlocked.productTypes.Add(pt);
                data.unlocked.verticals.Add(vert);

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
            Assert.AreEqual(gd.company.cash.value,         data.company.cash.value);
            Assert.AreEqual(gd.company.baseSizeLimit,      data.company.baseSizeLimit);
            Assert.AreEqual(gd.company.infrastructure,     data.company.infrastructure);
            Assert.AreEqual(gd.company.lastMonthRevenue,   data.company.lastMonthRevenue);
            Assert.AreEqual(gd.company.lastMonthCosts,     data.company.lastMonthCosts);
            Assert.IsTrue  (gd.company.featurePoints   ==  data.company.featurePoints);
            Assert.AreEqual(gd.company.lastMonthCosts,     data.company.lastMonthCosts);
            Assert.AreEqual(gd.company.research.value,     data.company.research.value);

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

                for (int j=0; j<p.productTypes.Count; j++) {
                    Assert.AreEqual(p.productTypes[j].name, p_.productTypes[j].name);
                    Assert.IsTrue(p.productTypes[j].requiredInfrastructure.Equals(p_.productTypes[j].requiredInfrastructure));
                }
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

                CompareEffectSets(f.bonuses, f_.bonuses);
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

                CompareEffectSets(m.effects, m_.effects);
            }


            CompareUnlockSets(gd.unlocked,               data.unlocked);

            Assert.AreEqual(gd.board.happiness,          data.board.happiness);

            // These should be the _same instance_!
            Assert.AreEqual(gd.technology, data.technology);
            Assert.IsTrue(gd.research == data.research);
		}

        private void CompareEffectSets(EffectSet es, EffectSet es_) {
            Assert.AreEqual(es.workers.Count, es_.workers.Count);
            for (int j=0;j<es.workers.Count;j++) {
                StatBuff sb  = es.workers[j];
                StatBuff sb_ = es_.workers[j];

                Assert.AreEqual(sb.name,  sb_.name);
                Assert.AreEqual(sb.value, sb_.value);
            }
            Assert.AreEqual(es.company.Count, es_.company.Count);
            for (int j=0;j<es.company.Count;j++) {
                StatBuff sb  = es.company[j];
                StatBuff sb_ = es_.company[j];

                Assert.AreEqual(sb.name,  sb_.name);
                Assert.AreEqual(sb.value, sb_.value);
            }

            Assert.AreEqual(es.products.Count, es_.products.Count);
            for (int j=0;j<es.products.Count;j++) {
                ProductEffect pe  = es.products[j];
                ProductEffect pe_ = es_.products[j];

                Assert.AreEqual(pe.buff.name,       pe_.buff.name);
                Assert.AreEqual(pe.buff.value,      pe_.buff.value);
                Assert.AreEqual(pe.productTypes[0].name, pe_.productTypes[0].name);
            }
            CompareUnlockSets(es.unlocks, es_.unlocks);

        }

        private void CompareUnlockSets(UnlockSet us, UnlockSet us_) {
            Assert.AreEqual(us.productTypes.Count,   us_.productTypes.Count);
            Assert.AreEqual(us.productTypes[0].name, us_.productTypes[0].name);

            Assert.AreEqual(us.verticals.Count,   us_.verticals.Count);
            Assert.AreEqual(us.verticals[0].name, us_.verticals[0].name);
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
            founder.bonuses                = CreateEffectSet();
            return founder;
        }

        private Product CreateProduct() {
            ProductType pt = ProductType.Load("Social Network");
            Vertical v     = Vertical.Load("Information");
            List<ProductType> pts = new List<ProductType>() { pt };

            Product product = ScriptableObject.CreateInstance<Product>();
            product.Init(pts);

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

        private EffectSet CreateEffectSet() {
            EffectSet e = new EffectSet();

            e.company.Add(new StatBuff("Cash",         RandFloat()));
            e.workers.Add(new StatBuff("Happiness",    RandFloat()));
            e.workers.Add(new StatBuff("Productivity", RandFloat()));

            ProductType pt = ProductType.Load("Social Network");
            Vertical v     = Vertical.Load("Information");
            e.unlocks.productTypes.Add(pt);
            e.unlocks.verticals.Add(v);

            ProductEffect pe = new ProductEffect();
            pe.productTypes.Add(pt);
            pe.buff = new StatBuff("Appeal", RandFloat());
            e.products.Add(pe);

            return e;
        }

        private Item CreateItem() {
            Item item = (Item)GameObject.Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Editor/Tests/Resources/TestItem.asset", typeof(Item)));
            item.effects = CreateEffectSet();
            return item;
        }

        private float RandFloat() {
            return Random.Range(10, 50);
        }
    }
}
