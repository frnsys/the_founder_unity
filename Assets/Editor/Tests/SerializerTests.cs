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

            // Initialize new game stuff.
            data.company  = new Company("TESTINGCORP").Init();
                data.company.founders.Add(CreateFounder("STEVE", 100));
                data.company.baseSizeLimit       = 17;
                data.company.lastAnnualRevenue   = 28517;
                data.company.annualRevenue       = 12489;
                data.company.annualCosts         = 184787;
                data.company.cash.baseValue      = 100000000;
                data.company.opinion             = 200;
                data.company.hype                = 300;
                data.company.office              = Office.Type.Office;

                data.company.perks               = new List<APerk>() {
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


                data.company.recruitments        = new List<int>() { 0,3,4 };
                data.company.promos              = new List<int>() { 0,2,5 };

                EffectSet es = new EffectSet();
                es.Apply(data.company);

                for (int i=0;i<5;i++) {
                    AWorker worker = CreateWorker("WORKER"+i, i*10);
                    worker.offMarketTime = i;
                    worker.leaveProb = i/10;
                    wm.HireWorker(worker);
                }

            data.board    = new TheBoard();
            data.board.happiness = 20;

            data.forgettingRate = 10;
            data.wageMultiplier = 10;
            data.spendingMultiplier = 10;
            data.economicStability = 10;

            GameEvent ev = ScriptableObject.CreateInstance<GameEvent>();
            AGameEvent aGe = new AGameEvent(ev);
            ev.name = "TESTEVENT";
            aGe.probability = 1f;
            data.eventsPool.Add(aGe);

            GameEvent sev = ScriptableObject.CreateInstance<GameEvent>();
            AGameEvent saGe = new AGameEvent(sev);
            sev.name = "TESTSPECIALEVENT";
            saGe.probability = 1f;
            data.specialEventsPool.Add(saGe);

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

            // Save and re-load.
            string filepath = "/tmp/the_founder_test_saving.dat";
            GameData.Save(data, filepath);
            GameData gd = GameData.Load(filepath);

            // The two product types should be the same instance.
            ProductType pt_hu = gd.unlocked.productTypes[0];
            Assert.AreEqual(pt_hu.GetInstanceID(), data.unlocked.productTypes[0].GetInstanceID());
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

            Assert.AreEqual(gd.forgettingRate,             data.forgettingRate);
            Assert.AreEqual(gd.wageMultiplier,             data.wageMultiplier);
            Assert.AreEqual(gd.spendingMultiplier,         data.spendingMultiplier);
            Assert.AreEqual(gd.economicStability,          data.economicStability);

            Assert.AreEqual(gd.company.name,               data.company.name);
            Assert.AreEqual(gd.company.office,             data.company.office);
            Assert.AreEqual(gd.company.cash.value,         data.company.cash.value);
            Assert.AreEqual(gd.company.baseSizeLimit,      data.company.baseSizeLimit);
            Assert.AreEqual(gd.company.lastAnnualRevenue,  data.company.lastAnnualRevenue);
            Assert.AreEqual(gd.company.annualRevenue,      data.company.annualRevenue);
            Assert.AreEqual(gd.company.annualCosts,        data.company.annualCosts);
            Assert.AreEqual(gd.company.opinion,            data.company.opinion);
            Assert.AreEqual(gd.company.hype,               data.company.hype);
            Assert.AreEqual(gd.company.markets,            data.company.markets);

            for (int i=0; i<gd.company.perks.Count; i++) {
                APerk p = gd.company.perks[i];
                APerk p_ = data.company.perks[i];

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

            Assert.AreEqual(gd.company.workers.Count,      data.company.workers.Count);
            for (int i=0;i<gd.company.workers.Count;i++) {
                AWorker w  = data.company.workers[i];
                AWorker w_ = gd.company.workers[i];

                CompareWorkers(w, w_);
            }

            Assert.AreEqual(gd.company.founders.Count, data.company.founders.Count);
            for (int i=0;i<gd.company.founders.Count;i++) {
                AWorker f  = data.company.founders[i];
                AWorker f_ = gd.company.founders[i];

                Assert.AreEqual(f.name,                     f_.name);
                Assert.AreEqual(f.salary,                   f_.salary);
                Assert.AreEqual(f.happiness,                f_.happiness);
                Assert.AreEqual(f.productivity,             f_.productivity);
                Assert.AreEqual(f.charisma,                 f_.charisma);
                Assert.AreEqual(f.creativity,               f_.creativity);
                Assert.AreEqual(f.creativity,               f_.creativity);
            }

            CompareUnlockSets(gd.unlocked,               data.unlocked);

            Assert.AreEqual(gd.board.happiness,          data.board.happiness);

            Assert.AreEqual(gd.company.recruitments, data.company.recruitments);
            Assert.AreEqual(gd.company.promos, data.company.promos);
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

        private AWorker CreateWorker(string name, int stat) {
            Worker w = (Worker)GameObject.Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Editor/Tests/Resources/TestWorker.asset", typeof(Worker)));
            AWorker worker = new AWorker(w);
            worker.salary                 = stat;
            worker.happiness              = stat;
            worker.productivity           = stat;
            worker.charisma               = stat;
            worker.creativity             = stat;
            worker.cleverness             = stat;
            return worker;
        }

        private AWorker CreateFounder(string name, int stat) {
            Worker w = (Worker)GameObject.Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Editor/Tests/Resources/TestWorker.asset", typeof(Worker)));
            AWorker founder = new AWorker(w);
            founder.salary                = stat;
            founder.happiness             = stat;
            founder.productivity          = stat;
            founder.charisma              = stat;
            founder.creativity            = stat;
            founder.cleverness            = stat;
            return founder;
        }

        private APerk CreatePerk(int i) {
            Perk perk = (Perk)GameObject.Instantiate(AssetDatabase.LoadAssetAtPath("Assets/Editor/Tests/Resources/TestPerk.asset", typeof(Perk)));
            perk.name = string.Format("PERK{0}", i);
            APerk p = new APerk(perk);
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

        private void CompareWorkers(AWorker w, AWorker w_) {
            Assert.AreEqual(w.name,                     w_.name);
            Assert.AreEqual(w.salary,                   w_.salary);
            Assert.AreEqual(w.offMarketTime,            w_.offMarketTime);
            Assert.AreEqual(w.leaveProb,                w_.leaveProb);
            Assert.AreEqual(w.happiness,                w_.happiness);
            Assert.AreEqual(w.productivity,             w_.productivity);
            Assert.AreEqual(w.charisma,                 w_.charisma);
            Assert.AreEqual(w.creativity,               w_.creativity);
            Assert.AreEqual(w.creativity,               w_.creativity);
        }
    }
}
