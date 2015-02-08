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
        private GameObject gameObj;
        private GameManager gm;
        private WorkerManager wm;
        private GameData data = null;

        [SetUp]
        public void SetUp() {
            /*
             * IMPORTANT NOTE:
             * - For SharedResources, you *MUST* load from an asset,
             *   not create a new instance. The serializer checks if
             *   something is a subclass of SharedResource, and if it
             *   is, it tries to load directly from the asset. So if
             *   it doesn't exists, it will throw an exception.
             */

            data = ScriptableObject.Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Editor/Tests/Resources/TestingGameData.asset", typeof(GameData))) as GameData;


            gameObj = new GameObject("Game Manager");
            gm = gameObj.AddComponent<GameManager>();
            gm.Load(data);
            wm = gm.workerManager;

            Worker researchCzar = CreateWorker("RESEARCHER", 1000);
            Worker opinionCzar  = CreateWorker("OPINIONER",  2000);

            // Initialize new game stuff.
            data.company  = new Company("TESTINGCORP").Init();
                data.company.founders.Add(CreateFounder("STEVE", 100));
                data.company.baseSizeLimit       = 17;
                data.company.lastMonthRevenue    = 28517;
                data.company.quarterRevenue      = 12489;
                data.company.quarterCosts        = 184787;
                data.company.cash.baseValue      = 100000000;
                data.company.ResearchCzar        = researchCzar;
                data.company.OpinionCzar         = opinionCzar;
                data.company.opinion.baseValue   = 200;
                data.company.forgettingRate      = 10;
                data.company.publicity.baseValue = 300;
                data.company.office              = Office.Type.Office;

                data.company.perks               = new List<Perk>() {
                    CreatePerk(0),
                    CreatePerk(1)
                };

                data.company.markets             = new List<MarketManager.Market>() {
                    MarketManager.Market.Asia,
                    MarketManager.Market.Europe
                };

                data.company.companies           = new List<MiniCompany>() {
                    MiniCompany.Load("The Times"),
                    MiniCompany.Load("Carrot, Inc")
                };

                EffectSet es = new EffectSet();
                es.opinionEvent = new OpinionEvent(100, 400);
                es.Apply(data.company);

                for (int i=0;i<5;i++) {
                    Worker worker = CreateWorker("WORKER"+i, i*10);
                    worker.offMarketTime = i;
                    worker.recentPlayerOffers = i;
                    wm.HireWorker(worker);
                }

                for (int i=0;i<5;i++) {
                    data.company.products.Add(CreateProduct());
                }

            data.board    = new TheBoard();
                data.board.happiness = 20;

            data.research  = 500;
            data.technology = Technology.Load("3D Printing");

            GameEvent ev = new GameEvent("TESTEVENT", 1f);
            data.eventsPool.Add(ev);

            GameEvent sev = new GameEvent("TESTSPECIALEVENT", 1f);
            data.specialEventsPool.Add(sev);

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
            UnityEngine.Object.DestroyImmediate(gameObj);
            gm = null;
            data = null;
        }

        [Test]
        public void TestResourcesResolveCorrectly() {
            // Just a sanity check to ensure that resources are loaded once.
            ProductType pt = ProductType.Load("Social Network");
            ProductType pt_ = ProductType.Load("Social Network");
            Assert.AreEqual(pt.GetInstanceID(), pt_.GetInstanceID());

            // Another sanity check.
            ProductType pt__ = data.unlocked.productTypes[0];
            Assert.AreEqual(pt.GetInstanceID(), pt__.GetInstanceID());

            // Create an AI company who also has this product type.
            AICompany oc = ScriptableObject.CreateInstance<AICompany>().Init();
            oc.unlocked.productTypes.Add(pt);
            data.otherCompanies.Add(oc);

            // Save and re-load.
            string filepath = "/tmp/the_founder_test_saving.dat";
            GameData.Save(data, filepath);
            GameData gd = GameData.Load(filepath);

            // The two product types should be the same instance.
            ProductType pt_ai = gd.otherCompanies[0].unlocked.productTypes[0];
            ProductType pt_hu = gd.unlocked.productTypes[0];
            Assert.AreEqual(pt_ai.GetInstanceID(), pt_hu.GetInstanceID());
            Assert.AreEqual(pt_ai.GetInstanceID(), data.unlocked.productTypes[0].GetInstanceID());
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
            Assert.AreEqual(gd.company.office,             data.company.office);
            Assert.AreEqual(gd.company.cash.value,         data.company.cash.value);
            Assert.AreEqual(gd.company.baseSizeLimit,      data.company.baseSizeLimit);
            Assert.AreEqual(gd.company.infrastructure,     data.company.infrastructure);
            Assert.AreEqual(gd.company.lastMonthRevenue,   data.company.lastMonthRevenue);
            Assert.AreEqual(gd.company.quarterRevenue,     data.company.quarterRevenue);
            Assert.AreEqual(gd.company.quarterCosts,       data.company.quarterCosts);
            Assert.AreEqual(gd.company.research.value,     data.company.research.value);
            Assert.AreEqual(gd.company.opinion.value,      data.company.opinion.value);
            Assert.AreEqual(gd.company.publicity.value,    data.company.publicity.value);
            Assert.AreEqual(gd.company.forgettingRate,     data.company.forgettingRate);
            Assert.AreEqual(gd.company.markets,            data.company.markets);
            CompareWorkers(gd.company.ResearchCzar,        data.company.ResearchCzar);
            CompareWorkers(gd.company.OpinionCzar,         data.company.OpinionCzar);

            for (int i=0; i<gd.company.perks.Count; i++) {
                Perk p = gd.company.perks[i];
                Perk p_ = data.company.perks[i];

                Assert.AreEqual(p.name, p_.name);
                Assert.AreEqual(p.upgradeLevel, p_.upgradeLevel);
            }

            for (int i=0; i<gd.company.companies.Count; i++) {
                MiniCompany mc = gd.company.companies[i];
                MiniCompany mc_ = data.company.companies[i];

                Assert.AreEqual(mc.name, mc_.name);
                Assert.AreEqual(mc.baseCost, mc_.baseCost);
                Assert.AreEqual(mc.revenue, mc_.revenue);
            }

            Assert.AreEqual(gd.eventsPool[0].name,         data.eventsPool[0].name);
            Assert.AreEqual(gd.specialEventsPool[0].name,  data.specialEventsPool[0].name);

            for (int i=0;i<gd.company.OpinionEvents.Count;i++) {
                CompareOpinionEvent(data.company.OpinionEvents[i], gd.company.OpinionEvents[i]);
            }

            Assert.AreEqual(gd.company.workers.Count,      data.company.workers.Count);
            for (int i=0;i<gd.company.workers.Count;i++) {
                Worker w  = data.company.workers[i];
                Worker w_ = gd.company.workers[i];

                CompareWorkers(w, w_);
            }

            Assert.AreEqual(gd.company.products.Count, data.company.products.Count);
            for (int i=0;i<gd.company.products.Count;i++) {
                Product p  = data.company.products[i];
                Product p_ = gd.company.products[i];

                Assert.AreEqual(p.name,               p_.name);
                Assert.AreEqual(p.progress,           p_.progress);
                Assert.AreEqual(p.disabled,           p_.disabled);
                Assert.AreEqual(p.state,              p_.state);
                Assert.AreEqual(p.timeSinceLaunch,    p_.timeSinceLaunch);
                Assert.AreEqual(p.revenueEarned,      p_.revenueEarned);
                Assert.AreEqual(p.requiredProgress,   p_.requiredProgress);
                Assert.AreEqual(p.lastRevenue,        p_.lastRevenue);
                Assert.AreEqual(p.points,             p_.points);

                Assert.AreEqual(p.design.value,       p_.design.value);
                Assert.AreEqual(p.marketing.value,    p_.marketing.value);
                Assert.AreEqual(p.engineering.value,  p_.engineering.value);

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

            CompareUnlockSets(gd.unlocked,               data.unlocked);

            Assert.AreEqual(gd.board.happiness,          data.board.happiness);

            // These should be the _same instance_!
            Assert.AreEqual(gd.technology, data.technology);
            Assert.IsTrue(gd.research == data.research);
		}

        private void CompareOpinionEvent(OpinionEvent oe, OpinionEvent oe_) {
            Assert.AreEqual(oe.opinion.value, oe_.opinion.value);
            Assert.AreEqual(oe.publicity.value, oe_.publicity.value);
        }

        private void CompareEffectSets(EffectSet es, EffectSet es_) {
            List<StatBuff> wes = es.workerEffects;
            List<StatBuff> wes_ = es_.workerEffects;
            Assert.AreEqual(wes.Count, wes_.Count);
            for (int j=0;j<wes.Count;j++) {
                StatBuff sb  = wes[j];
                StatBuff sb_ = wes_[j];

                Assert.AreEqual(sb.name,  sb_.name);
                Assert.AreEqual(sb.value, sb_.value);
            }

            Assert.AreEqual(es.cash, es_.cash);
            Assert.AreEqual(es.research.value, es_.research.value);
            Assert.AreEqual(es.aiCompany, es_.aiCompany);
            Assert.AreEqual(es.gameEvent, es_.gameEvent);

            List<ProductEffect> pes = es.productEffects;
            List<ProductEffect> pes_ = es_.productEffects;
            Assert.AreEqual(pes.Count, pes_.Count);
            for (int j=0;j<pes.Count;j++) {
                ProductEffect pe  = pes[j];
                ProductEffect pe_ = pes_[j];

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
            pt.requiredVerticals = new List<Vertical>() { v };
            List<ProductType> pts = new List<ProductType>() { pt };

            Product product = ScriptableObject.CreateInstance<Product>();
            product.Init(pts, 0, 0, 0, data.company);
            product.requiredProgress = 100000;

            product.Develop(RandFloat(), data.company);

            float r = Random.Range(0,1);
            if (r > 0.33) {
                product.Launch();
                product.Revenue(5, data.company);

                if (Random.Range(0,1) > 0.5) {
                    product.disabled = true;
                }
            } else if (r > 0.66) {
                product.Shutdown();
            }

            return product;
        }

        private Perk CreatePerk(int i) {
            Perk p = ScriptableObject.CreateInstance<Perk>();
            p.name = string.Format("PERK{0}", i);
            p.upgradeLevel = i;
            return p;
        }

        private MiniCompany CreateMiniCompany(int i) {
            MiniCompany mc = ScriptableObject.CreateInstance<MiniCompany>();
            mc.name = string.Format("MC{0}", i);
            mc.baseCost = i*1000;
            mc.revenue = i*1000;
            return mc;
        }

        private EffectSet CreateEffectSet() {
            EffectSet e = new EffectSet();

            e.cash = RandFloat();

            e.workerEffects = new List<StatBuff>();
            StatBuff wb = new StatBuff("Happiness", RandFloat());
            e.workerEffects.Add(wb);

            StatBuff wb_ = new StatBuff("Productivity", RandFloat());
            e.workerEffects.Add(wb_);

            ProductType pt = ProductType.Load("Social Network");
            Vertical v     = Vertical.Load("Information");
            e.unlocks.productTypes.Add(pt);
            e.unlocks.verticals.Add(v);

            e.productEffects = new List<ProductEffect>();
            ProductEffect pe = new ProductEffect("Design");
            pe.productTypes.Add(pt);
            pe.buff = new StatBuff("Design", RandFloat());
            e.productEffects.Add(pe);

            return e;
        }

        private float RandFloat() {
            return Random.Range(10, 50);
        }

        private void CompareWorkers(Worker w, Worker w_) {
            Assert.AreEqual(w.name,                     w_.name);
            Assert.AreEqual(w.salary,                   w_.salary);
            Assert.AreEqual(w.offMarketTime,            w_.offMarketTime);
            Assert.AreEqual(w.recentPlayerOffers,       w_.recentPlayerOffers);
            Assert.AreEqual(w.happiness.baseValue,      w_.happiness.baseValue);
            Assert.AreEqual(w.productivity.baseValue,   w_.productivity.baseValue);
            Assert.AreEqual(w.charisma.baseValue,       w_.charisma.baseValue);
            Assert.AreEqual(w.creativity.baseValue,     w_.creativity.baseValue);
            Assert.AreEqual(w.creativity.baseValue,     w_.creativity.baseValue);
        }
    }
}
