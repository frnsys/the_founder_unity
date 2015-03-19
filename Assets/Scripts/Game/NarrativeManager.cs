/*
 * The narrative manager handles the progression of the story.
 * These are usually one-off or special events.
 */

using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class NarrativeManager : Singleton<NarrativeManager> {
    private GameData data;
    private OnboardingState ob;

    [System.Serializable]
    public struct OnboardingState {
        public bool PRODUCTS_OPENED;
        public bool INFRASTRUCTURE_OPENED;
        public bool PERKS_UNLOCKED;
        public bool VERTICALS_UNLOCKED;
        public bool LOCATIONS_UNLOCKED;
        public bool SPECIALPROJECTS_UNLOCKED;
        public bool HYPE_MINIGAME;
        public bool SYNERGY;
    }

    // Disable the constructor.
    protected NarrativeManager() {}

    private GameObject mentorMessagePrefab;

    void Awake() {
        mentorMessagePrefab = Resources.Load("UI/Narrative/Mentor Message") as GameObject;
    }

    void OnEnable() {
        UIWindow.WindowOpened += OnScreenOpened;
    }

    void OnDisable() {
        UIWindow.WindowOpened -= OnScreenOpened;
    }

    public void Load(GameData d) {
        data = d;
    }

    // A message from your mentor.
    public UIMentor MentorMessage(string message) {
        UIEventListener.VoidDelegate callback = delegate(GameObject obj) {
            obj.GetComponent<UIMentor>().Hide();
        };
        return MentorMessage(message, callback);
    }

    // A message from your mentor with a callback after tap.
    public UIMentor MentorMessage(string message, UIEventListener.VoidDelegate callback) {
        GameObject alerts = UIRoot.list[0].transform.Find("Alerts").gameObject;

        GameObject msg = NGUITools.AddChild(alerts, mentorMessagePrefab);
        UIMentor mentor = msg.GetComponent<UIMentor>();
        mentor.message = message;

        UIEventListener.Get(mentor.box).onClick += delegate(GameObject obj) {
            callback(msg);
        };

        return mentor;
    }

    // A list of messages to be shown in sequence (on tap).
    public void MentorMessages(string[] messages) {
        UIEventListener.VoidDelegate callback = delegate(GameObject obj) {};
        MentorMessages(messages, callback);
    }

    public void MentorMessages(string[] messages, UIEventListener.VoidDelegate callback) {
        int i = 0;

        // Back button action.
        UIEventListener.VoidDelegate back = delegate(GameObject obj) {
            if (i > 0) {
                i--;
                obj.transform.parent.GetComponent<UIMentor>().message = messages[i];
                if (i == 0)
                    obj.SetActive(false);
            }
        };

        UIEventListener.VoidDelegate afterEach = delegate(GameObject obj) {
            if (i < messages.Length - 1) {
                i++;
                obj.GetComponent<UIMentor>().message = messages[i];
            } else {
                obj.GetComponent<UIMentor>().Hide();
                callback(obj);
            }

            // Show & setup back button if necessary.
            GameObject backButton = obj.transform.Find("Back").gameObject;
            if (i > 0) {
                backButton.SetActive(true);
                if (UIEventListener.Get(backButton).onClick == null) {
                    UIEventListener.Get(backButton).onClick += back;
                }
            } else {
                backButton.SetActive(false);
            }
        };

        MentorMessage(messages[0], afterEach);
    }


    /*
     * ==========================================
     * Onboarding ===============================
     * ==========================================
     */

    private enum OBS {
        START,
        GAME_INTRO,
        OPENED_NEW_PRODUCT,
        STARTED_PRODUCT,
        COMPLETED_PRODUCT,
        THE_MARKET,
        THE_MARKET_DONE,
        OPENED_RECRUITING,
        OPENED_HIRING,
        HIRED_EMPLOYEE,
        INFRASTRUCTURE,
        RESEARCH,
        GAME_GOALS,
    }
    private OBS obs = OBS.START;

    // Setup the starting game state for onboarding.
    public void InitializeOnboarding() {
        obs = OBS.START;

        // Listen to some events.
        Company.BeganProduct += BeganProduct;
        Company.WorkerHired += WorkerHired;
        Company.BoughtInfrastructure += InfrastructureBought;
        Company.Synergy += Synergy;
        Product.Completed += CompletedProduct;
        Product.Launched += LaunchedProduct;
        Promo.Completed += PromoCompleted;
        GameEvent.EventTriggered += OnEvent;
        UnlockSet.Unlocked += OnUnlocked;
        TheMarket.Started += OnMarketStarted;
        TheMarket.Done += OnMarketDone;
        UIOfficeManager.OfficeUpgraded += OfficeUpgraded;

        // Hide some menu and status bar items.
        UIManager uim = UIManager.Instance;
        uim.statusBar.hypeLabel.gameObject.SetActive(false);
        uim.statusBar.researchLabel.gameObject.SetActive(false);
        uim.menu.Deactivate("New Product");
        uim.menu.Deactivate("Accounting");
        uim.menu.Deactivate("Special Projects");
        uim.menu.Deactivate("Infrastructure");
        uim.menu.Deactivate("Existing Products");
        uim.menu.Deactivate("Locations");
        uim.menu.Deactivate("Verticals");
        uim.menu.Deactivate("Acquisitions");
        uim.menu.Deactivate("Lobbying");
        uim.menu.Deactivate("Recruiting");
        uim.menu.Deactivate("Employees");
        uim.menu.Deactivate("Perks");
        uim.menu.Deactivate("Research");
        uim.menu.Deactivate("Communications");

        // Show the game intro.
        Intro();
    }

    void LaunchedProduct(Product p, Company c, float score) {
        ProductRecipe r = p.Recipe;
        if (c == data.company) {
            if (score < 0.6f) {
                switch(r.primaryFeature) {
                    case ProductRecipe.Feature.Design:
                        StartCoroutine(Delay(delegate(GameObject obj) {
                            MentorMessages(new string[] {
                                "Hmm...that last product probably could have been designed better."
                            });
                        }, 20f));
                        return;
                    case ProductRecipe.Feature.Engineering:
                        StartCoroutine(Delay(delegate(GameObject obj) {
                            MentorMessages(new string[] {
                                "Hmm...that last product probably could have been engineered better."
                            });
                        }, 20f));
                        return;
                    case ProductRecipe.Feature.Marketing:
                        StartCoroutine(Delay(delegate(GameObject obj) {
                            MentorMessages(new string[] {
                                "Hmm...that last product probably could have been marketed better."
                            });
                        }, 20f));
                        return;
                }
            } else if (p.marketShare < 0.6f) {
                StartCoroutine(Delay(delegate(GameObject obj) {
                    MentorMessages(new string[] {
                        "Hmm...that last product could have had more reach. Consider expanding to new locations, or build more hype for the company!"
                    });
                }, 20f));
            }
        }
    }

    void OnMarketStarted() {
        if (Stage(OBS.THE_MARKET)) {
            MentorMessages(new string[] {
                string.Format("Now that your product is completed, it is released into {0}.", ConceptHighlight("The Market")),
                string.Format("{0} will release products like the ones you create to edge out your profits.", ConceptHighlight("Competitors")),
                "Consumers will flock to the products they think are the best.",
                "Naturally, if your products are better, you'll have the upper hand.",
                "Let's see how they respond."
            });
            TheMarket.Started -= OnMarketStarted;
        }
    }
    void OnMarketDone() {
        if (Stage(OBS.THE_MARKET_DONE)) {
            MentorMessages(new string[] {
                string.Format("Hmph. Consumers aren't really into it. That percentage you saw was the {0} of your product.", ConceptHighlight("market share")),
                "Your product could be better.",
                "To make better products you need to assemble a talented team.",
                string.Format("Search for candidates by opening {0} in the menu.", MenuHighlight("Recruiting"))
            });
            UIManager.Instance.menu.Activate("Recruiting");
            TheMarket.Done -= OnMarketDone;
        }
    }

    void OnEvent(GameEvent ev) {
        if (ev.name == "New Company on the Scene") {
            StartCoroutine(Delay(delegate(GameObject obj) {
                MentorMessages(new string[] {
                    "Congratulations! This is your first write-up in a major publication.",
                    string.Format("This kind of mention has driven up the {0} for your company.", ConceptHighlight("hype")),
                    "Hype is central to your company's success. A hyped company's products sell much better.",
                    string.Format("But hype is always deflating. Keep hyping your company by launching {0} from the {1} button below.", ConceptHighlight("promotional campaigns"), MenuHighlight("MarComm")),
                    string.Format("But note that some press can be negative. {0} hurts your company's image.", ConceptHighlight("Bad publicity")),
                    "Consumers aren't going to buy your products if they disagree with your decisions.",
                    "Fortunately, consumers forget things over time, and hype can counteract bad publicity."
                });
                UIManager uim = UIManager.Instance;
                uim.statusBar.hypeLabel.gameObject.SetActive(true);
                uim.menu.Activate("Communications");
            }));

        } else if (ev.name == "RIVALCORP Founded") {
            StartCoroutine(Delay(delegate(GameObject obj) {
                MentorMessages(new string[] {
                    "Uh oh. Looks like you have some enemies.",
                    "Competitors will also poach your employees. This kind of activity can drive wages up. This is a lose-lose for everyone - other companies can be cooperative when it comes to dealing with this."
                });
            }));
        } else if (ev.name == "Data access") {
            StartCoroutine(Delay(delegate(GameObject obj) {
                MentorMessages(new string[] {
                    "It looks like you're on the government's radar now.",
                    "This presents a great opportunity.",
                    string.Format("With some contacts in government, you can start {0} them.", ConceptHighlight("lobbying")),
                    string.Format("You can access this through the {0} menu item.", MenuHighlight("Lobbying"))
                });
                UIManager.Instance.menu.Activate("Lobbying");
            }));
        }
    }
    private IEnumerator Delay(UIEventListener.VoidDelegate callback, float delay = 12f) {
        yield return StartCoroutine(GameTimer.Wait(delay));
        callback(null);
    }

    public void Intro() {
        obs = OBS.GAME_INTRO;
        MentorMessages(new string[] {
            "Welcome to your office! You're just starting out, so you'll work from your apartment for now.",
            "Right now it's just your cofounder in the office, but eventually you'll have a buzzing hive of talented employees.",
            string.Format("You're not much of a business if haven't got anything to sell. Let's create a {0}.", ConceptHighlight("product")),
            string.Format("To start creating a product, tap the {0} button below.", MenuHighlight("New Product"))
        });
        UIManager.Instance.menu.Activate("New Product");
    }

    // Checks if it is appropriate to execute the specified stage,
    private bool Stage(OBS stage) {
        if (obs == stage - 1) {
            obs = stage;
            return true;
        }
        return false;
    }

    // Triggered whenever a window or tab is opened.
    public void OnScreenOpened(string name) {
        Debug.Log(name + " opened");
        switch(name) {

            case "New Product":
                if (Stage(OBS.OPENED_NEW_PRODUCT)) {
                    MentorMessages(new string[] {
                        string.Format("Products are created by combining two {0}.", ConceptHighlight("product types")),
                        string.Format("Some combinations work well and give {0}. Some don't.", ConceptHighlight("bonuses")),
                        string.Format("You will have to {0} and experiment with different combinations.", SpecialHighlight("innovate")),
                        "Right now you only have a few types available, but that will change over time.",
                        string.Format("Products require different kinds of {0} to support their growth.", ConceptHighlight("infrastructure")),
                        "They might require\n:DATACENTER: [c][0078E1]datacenters[-][/c],\n:FACTORY: [c][0078E1]factories[-][/c],\n:LAB: [c][0078E1]labs[-][/c], or\n:STUDIO: [c][0078E1]studios[-][/c].",
                        "All product types have some minimum necessary infrastructure before you can use them.",
                        "You have some infrastructure to start, shown at the bottom of the screen, so no need to worry now.",
                        "Pick two product types and hit the button below to start developing the product."
                    });

                } else if (Stage(OBS.INFRASTRUCTURE)) {
                    MentorMessages(new string[] {
                        "It looks like you don't have enough infrastructure to start a new product!",
                        string.Format("You can buy more infrastructure in the {0} menu item. There's a cost to setup the infrastructure, and then a rental cost every month after.", MenuHighlight("Infrastructure")),
                        string.Format("You can also shutdown products in the {0} menu item to reclaim their infrastructure.", MenuHighlight("Existing Products"))
                    });
                    UIManager.Instance.menu.Activate("Infrastructure");
                    UIManager.Instance.menu.Activate("Existing Products");
                }
                break;

            case "Recruiting":
                if (Stage(OBS.OPENED_RECRUITING)) {
                    MentorMessages(new string[] {
                        "Here is where you can recruit some new candidates to hire.",
                        "There are a few different recruiting methods which vary in cost and quality of candidate.",
                        "Give it a try!"
                    });
                }
                break;

            case "Hiring":
                if (Stage(OBS.OPENED_HIRING)) {
                    MentorMessages(new string[] {
                        "Here are the candidates from your recruiting effort.",
                        "To hire an candidate, you must give them an offer they find acceptable. You have three tries before they take an offer elsewhere.",
                        string.Format("The {0} an candidate is willing to accept is affected by a few things.", ConceptHighlight("minimum salary")),
                        "Generally, more skilled employees will expect more money.",
                        "Hiring competition amongst companies can drive salary expectations up.",
                        "However, if your employees are exceptionally happy, candidates will be willing to take a lower salary.",
                        "Don't forget - you're fighting to keep a high profit margin - so negotiate with that in mind!"
                    });
                }
                break;

            case "Products":
                if (!ob.PRODUCTS_OPENED) {
                    MentorMessages(new string[] {
                        "This is where you manage your products.",
                        "You can shutdown in-market products to reclaim the infrastructure they use and put it towards new products.",
                        "Shutdown products stop generating revenue for you. But their bonus effects are permanent!"
                    });
                    ob.PRODUCTS_OPENED = true;
                }
                break;

            case "Infrastructure":
                if (!ob.INFRASTRUCTURE_OPENED) {
                    MentorMessages(new string[] {
                        "Here's where you can manage your infrastructure.",
                        "You pay for infrastructure on a per-month basis.",
                        "You have a limit to how much infrastructure you can have at a given time, but you can swap out infrastructure when you need it.",
                        "Just note that infrastructure has a purchasing fee in addition to its monthly cost.",
                        "Infrastructure highlighted in green is currently being used by products, so you can't swap those out until they are no longer being used."
                    });
                    ob.INFRASTRUCTURE_OPENED = true;
                }
                break;

            default:
                break;
        }
    }

    void BeganProduct(Product p, Company c) {
        if (c == data.company) {
            if (Stage(OBS.STARTED_PRODUCT)) {
                StartCoroutine(Delay(delegate(GameObject obj) {
                    MentorMessages(new string[] {
                        "Great! You've started developing your first product.",
                        "The spinning sphere at the top is the undeveloped product.",
                        "You need to launch the value your employees produce towards the product.",
                        "Employees can produce\n:CREATIVITY: [c][0078E1]design[-][/c],\n:CLEVERNESS: [c][0078E1]engineering[-][/c], or\n:CHARISMA: [c][0078E1]marketing[-][/c]\npoints for your products.",
                        "Employees generate points depending on their skills. For instance, more creative employees generate more creativity points. More points = larger globs.",
                        string.Format("The rate at which they produce these points depends on their {0}.", ConceptHighlight("productivity")),
                        string.Format("{0} an employee to capture the value they've produced.", InteractHighlight("tap")),
                        string.Format("Employees get {0} from working. If you don't let them rest, they will take extra time recovering.", ConceptHighlight("tired")),
                        string.Format("As products get more difficult, you will start to encounter {0} which inhibit continued development.", ConceptHighlight("walls")),
                        "Each wall has some hit points and corresponds to a particular skill.",
                        string.Format(":CREATIVITY: [c][0078E1]design[-][/c] walls are tough and require a lot of {0} to break through.", ConceptHighlight("creativity")),
                        string.Format(":CLEVERNESS: [c][0078E1]engineering[-][/c] walls require a bit of {0}, but can tie your employees up with bugs.", ConceptHighlight("cleverness")),
                        string.Format(":CHARISMA: [c][0078E1]marketing[-][/c] walls need to be charmed quickly with {0}, becase they will regenerate their health.", ConceptHighlight("charisma")),
                        string.Format("Finally, {0} employees occasionally generate special\n:BREAKTHROUGH: [c][0078E1]breakthroughs[-][/c] which don't add any points to your product, but are especially potent against walls.", ConceptHighlight("happy")),
                        "The bar below indicates how far along product development is. When it fills up, your product will be released to The Market!"
                    });
                }, 2f));
            }
            Company.BeganProduct -= BeganProduct;
        }
    }

    void CompletedProduct(Product p, Company c) {
        if (c == data.company) {
            if (Stage(OBS.COMPLETED_PRODUCT)) {
                StartCoroutine(Delay(delegate(GameObject obj) {
                    MentorMessages(new string[] {
                        "Congratulations! You've completed your first product.",
                        string.Format("It will start generating {0}, depending on its final :DESIGN: [c][0078E1]design[-][/c], :ENGINEERING: [c][0078E1]engineering[-][/c], and :MARKETING: [c][0078E1]marketing[-][/c] values.", ConceptHighlight("revenue")),
                        "After the product has run its course, it will get pulled from The Market and free up the infrastructure it was using.",
                        "After you finish a product, you will be taken to The Market to see how it will perform.",
                        "Let's take a look."
                    });
                }, 3f));
            } else if (Stage(OBS.GAME_GOALS)) {
                StartCoroutine(Delay(delegate(GameObject obj) {
                    MentorMessages(new string[] {
                        "Now that you've got some experience with building products, [c][FC5656]The Board[-][/c] has asked me to explain their expectations for your company.",
                        string.Format("[c][FC5656]The Board[-][/c] sets {0} for you and requires that you [c][1A9EF2]expand your profits by 12% every year[-][/c].", ConceptHighlight("annual profit targets")),
                        "You [i]must[/i] hit these targets.",
                        "If [c][FC5656]The Board[-][/c] is unsatisfied your performance, they will [c][1A9EF2]dismiss you from the company[-][/c].",
                        "Making money is simple -  just make better and better products!",
                        string.Format("Your {0} and {1} are shown in the bar below.", ConceptHighlight("current profit"), ConceptHighlight("target profit")),
                        string.Format("You can keep more detailed track of your profit and other accounting in the {0} menu item.", MenuHighlight("Accounting"))
                    });
                    UIManager.Instance.menu.Activate("Accounting");
                }, 3f));
            } else if (Stage(OBS.RESEARCH)) {
                StartCoroutine(Delay(delegate(GameObject obj) {
                    MentorMessages(new string[] {
                        string.Format("You've built a few products but that won't be enough to sustain long-term growth. You need to invest in cutting-edge {0}.", ConceptHighlight("research")),
                        string.Format("You can manage your research budget in the {0} button below, which influences how many research points you generate.", MenuHighlight("Research")),
                        "There you can spend your research points to purchase new technologies. New technologies can unlock new product types, special projects, and provide other bonuses.",
                        "Don't neglect research! Stay ahead of the competition!"
                    });
                    UIManager uim = UIManager.Instance;
                    uim.statusBar.researchLabel.gameObject.SetActive(true);
                    uim.menu.Activate("Research");
                }, 12f));
                Product.Completed -= CompletedProduct;
            }
        }
    }

    void WorkerHired(Worker w, Company c) {
        if (c == data.company) {
            if (Stage(OBS.HIRED_EMPLOYEE)) {
                StartCoroutine(Delay(delegate(GameObject obj) {
                    MentorMessages(new string[] {
                        "Great, you have an employee now. See if you can build a new, better product."
                    });
                }, 6f));
            } else if (!ob.PERKS_UNLOCKED && c.workers.Count >= 3) {
                StartCoroutine(Delay(delegate(GameObject obj) {
                    MentorMessages(new string[] {
                        string.Format("Now that you have a few employees, you want to maximize their {0} and {1}.", ConceptHighlight("productivity"), ConceptHighlight("happiness")),
                        "Productive employees are easier to manage and happy employees can have valuable breakthroughs during product development and attract better talent.",
                        string.Format("A great way to accomplish this is through {0}. You can purchase and upgrade perks for your company through the {0} menu item.", ConceptHighlight("perks"), MenuHighlight("Perks"))
                    });
                    UIManager.Instance.menu.Activate("Perks");
                    ob.PERKS_UNLOCKED = true;
                }, 6f));
            }
        }
    }

    void PromoCompleted(Promo p) {
        if (!ob.HYPE_MINIGAME) {
            MentorMessages(new string[] {
                "Completing promotional campaigns gives you the opportunity to garner some allies in the media and hype up your company.",
                string.Format("{0} the puck and hit some {1} to get them on your side. More influential influencers can, through their influence, cause a cascade effect and bring over others to your side!", InteractHighlight("Flick"), ConceptHighlight("influencers"))
            });
            ob.HYPE_MINIGAME = true;
        }
    }

    void OnUnlocked(UnlockSet us) {
        if (us.verticals.Count > 0) {
            switch (us.verticals[0].name) {
                case "Finance":
                    MentorMessages(new string[] {
                        "Financial products can be extremely useful in your growth strategy.",
                        string.Format("Through credit cards and other financial schemes, you can fund consumption well beyond consumers' means. Financial products will typically increase {0}, thus making all your products more profitable!", ConceptHighlight("consumer spending")),
                    });
                    break;
                case "Defense":
                    MentorMessages(new string[] {
                        string.Format("Building defense products may seem unethical, but they generally lead to lucrative government contract {0} which are invaluable for funding your continued expansion.", ConceptHighlight("cash bonuses"))
                    });
                    break;
                case "Entertainment":
                    MentorMessages(new string[] {
                        "Promotional campaigns are great, but the most efficient way to manage public perception is through entertainment and media companies.",
                        string.Format("Entertainment products help consumers forget the dreariness or difficulty of their lives. Fortunately, these distractions also help them {0} about your company's transgressions more quickly.", ConceptHighlight("forget"))
                    });
                    break;
                default:
                    if (!ob.VERTICALS_UNLOCKED) {
                        MentorMessages(new string[] {
                            "Now that you've unlocked another vertical, you should consider saving up some capital to expand into it.",
                            string.Format("{0} provide access to new product types and technologies so you can {1} even further. Manage your verticals in the {2} menu item.", ConceptHighlight("Verticals"), SpecialHighlight("innovate"), MenuHighlight("Verticals"))
                        });
                        UIManager.Instance.menu.Activate("Verticals");
                        ob.VERTICALS_UNLOCKED = true;
                    }
                    break;
            }

        } else if (us.specialProjects.Count > 0 && !ob.SPECIALPROJECTS_UNLOCKED) {
            MentorMessages(new string[] {
                string.Format("Your first special project is available. {0} are one-off products which can have world-changing effects. In order to build one, you need to have built some prerequisite products beforehand.", ConceptHighlight("Special projects")),
                string.Format("Manage special projects in the {0} menu item.", MenuHighlight("Special Projects"))
            });
            UIManager.Instance.menu.Activate("Special Projects");
            ob.SPECIALPROJECTS_UNLOCKED = true;
        }
    }

    void InfrastructureBought(Company c, Infrastructure i) {
        if (c.availableInfrastructureCapacity == 0 && !ob.LOCATIONS_UNLOCKED) {
            MentorMessages(new string[] {
                "It looks like you've run out space for new infrastructure!",
                "That's ok - if you need more room for infrastructure, you can expand to new locations.",
                string.Format("{0} also provide capacity for more infrastructure. Some locations have special bonuses too.", ConceptHighlight("Locations")),
                string.Format("They also allow you to increase your share of existing {0} or establish a foothold in new ones.", ConceptHighlight("markets")),
                "The more locations you have for a market, the more money you will make!",
                string.Format("Manage your locations in the {0} menu item.", MenuHighlight("Locations"))
            });
            UIManager.Instance.menu.Activate("Locations");
            ob.LOCATIONS_UNLOCKED = true;
            Company.BoughtInfrastructure -= InfrastructureBought;
        }
    }

    void OfficeUpgraded(Office o) {
        if (o.type == Office.Type.Campus) {
            MentorMessages(new string[] {
                "Your company is impressively large now! But it could still be larger.",
                string.Format("It's harder to {0} on your own, but with all of your capital you can {1} other companies now. Manage these purchases through the {2}", SpecialHighlight("innovate"), ConceptHighlight("aquire"), MenuHighlight("Acquisitions"))
            });
            UIManager.Instance.menu.Activate("Acquisitions");
        }
    }

    void Synergy() {
        if (ob.SYNERGY == false) {
            StartCoroutine(Delay(delegate(GameObject obj) {
                MentorMessages(new string[] {
                    string.Format("Wow! That product you just released is {0} with another one in The Market.", SpecialHighlight("synergetic")),
                    string.Format("Products with {0} make a lot more money when they're in The Market together.", SpecialHighlight("synergy")),
                    "Use your Entrepeneurial Sense and try to figure out what combinations work best!"
                });
            }, 6f));
            ob.SYNERGY = true;
            Company.Synergy -= Synergy;
        }
    }

    public string MenuHighlight(string s) {
        return string.Format("[c][4B2FF8]{0}[-][/c]", s);
    }
    public string InteractHighlight(string s) {
        return string.Format(":INTERACT: [c][1CD05E]{0}[-][/c]", s);
    }
    public string SpecialHighlight(string s) {
        string[] colors = new string[] {
            "FC5656", "FFC800", "78E09E", "79ECDD", "4B2FF8", "FD7EFF"
        };
        List<string> result = new List<string>();
        for (int i=0; i<s.Length; i++) {
            result.Add(string.Format("[{0}]{1}[-]", colors[i % colors.Length], s[i]));
        }
        return string.Format("[c]{0}[/c]", string.Concat(result.ToArray()));
    }
    public string ConceptHighlight(string s) {
        return string.Format("[c][0078E1]{0}[-][/c]", s);
    }

    public void GameLost() {
        MentorMessages(new string[] {
            "Appalled by your inability to maintain the growth they are legally entitled to, the board has forced your resignation. You lose.",
            "But you don't really lose. You have secured your place in a class shielded from any real consequence or harm. You'll be fine. You could always found another company.",
            "GAME OVER."
        }, delegate(GameObject obj) {
            Application.LoadLevel("MainMenu");
        });
    }

    public void GameWon() {
        MentorMessages(new string[] {
            "Slowly, over the years, the vast network of sensors and extremities of some unknown project were constructed and distributed, the public clamoring desperately for each shiny piece.",
            "Each piece - each gadget and every app - was always quietly listening and watching and collecting data in their pockets and on their bodies and in their heads.",
            "It only took the brilliant innovation of The Founder to unify this fabric of disparate technology into its grand, unified destiny, dubbed \"The Founder AI\".",
            "The Founder AI would generate the most detailed and accurate of consumer taste profiles, down to impossibly accurate predictions of behavior in every sphere of life.",
            "In its ultimate efficiency and rationality, and with the world's automated infrastructure at its reigns, it would allocate resources, command employees, manage campaigns, and so much more, at global optima previously only theorized.",
            "Profit could be indisputably maximized by the complex interactions of intricate mathematical models capturing even the smallest aspects of life and market society.",
            "On the inaugural day of The Founder AI, you receive an email.",
            "You are being let go.",
            "Yes, you, The Founder, are an obstruction to the company's continued expansion.",
            "And so you melt into the impoverished population, finally betrayed by the oppressive logic, forever left to wander the terrible world it has shaped.",
            "GAME OVER."
        }, delegate(GameObject obj) {
            Application.LoadLevel("MainMenu");
        });
    }
}
