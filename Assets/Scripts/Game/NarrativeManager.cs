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

    [System.Serializable]
    public struct OnboardingState {
        public bool PRODUCTS_OPENED;
        public bool PERKS_UNLOCKED;
        public bool VERTICALS_UNLOCKED;
        public bool LOCATIONS_UNLOCKED;
        public bool SPECIALPROJECTS_UNLOCKED;
        public bool RESEARCH_OPENED;
        public bool HYPE_MINIGAME;
        public bool HIRED_EMPLOYEE;
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

    public enum OBS {
        START,
        GAME_INTRO,
        OPENED_HYPE,
        NEW_PRODUCT,
        OPENED_NEW_PRODUCT,
        STARTED_PRODUCT,
        THE_MARKET,
        THE_MARKET_DONE,
        OPENED_RECRUITING,
        OPENED_HIRING,
        GAME_GOALS,
        RESEARCH
    }

    // Setup the starting game state for onboarding.
    public void InitializeOnboarding() {
        // Listen to some events.
        Company.BeganProduct += BeganProduct;
        Company.WorkerHired += WorkerHired;
        Company.Synergy += Synergy;
        Product.Completed += CompletedProduct;
        Product.Launched += LaunchedProduct;
        Promo.Completed += PromoCompleted;
        GameEvent.EventTriggered += OnEvent;
        UnlockSet.Unlocked += OnUnlocked;
        TheMarket.Started += OnMarketStarted;
        TheMarket.Done += OnMarketDone;
        HypeMinigame.Done += OnHypeDone;
        UIOfficeManager.OfficeUpgraded += OfficeUpgraded;

        // Hide some menu and status bar items.
        UIManager uim = UIManager.Instance;
        uim.statusBar.hypeLabel.gameObject.SetActive(false);
        uim.statusBar.researchLabel.gameObject.SetActive(false);
        uim.menu.Deactivate("New Product");
        uim.menu.Deactivate("Accounting");
        uim.menu.Deactivate("Special Projects");
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
        // Product hints appear sporadically, and only after onboarding is finished.
        if (c == data.company && Random.value < 0.4f && data.obs == OBS.GAME_GOALS) {
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
                "Market share determines how much revenue your product will generate.",
                "This one will generate some revenue, but not nearly as much as it could have.",
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
                    string.Format("But note that some press can be negative. Bad publicity causes {0} against your company.", ConceptHighlight("outrage")),
                    "Consumers aren't going to buy your products if they disagree with your decisions.",
                    "Fortunately, consumers forget things over time, and hype can overwhelm outrage."
                });
            }, 2f));

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

    public void Intro() {
        data.obs = OBS.GAME_INTRO;
        MentorMessages(new string[] {
            "Welcome to your office! You're just starting out, so you'll work from your apartment for now.",
            "Right now it's just your cofounder in the office, but eventually you'll have a buzzing hive of talented employees.",
            string.Format("Before we dig into building a product, we have to {0} up your company!", ConceptHighlight("hype")),
            "That way, when you release a product, consumers will be clamoring to buy it.",
            string.Format("You can hype up company by launching {0} from the {1} below.", ConceptHighlight("promotional campaigns"), MenuHighlight("megaphone")),
            "Go a head and give it a try!"
        });
        UIManager uim = UIManager.Instance;
        uim.statusBar.hypeLabel.gameObject.SetActive(true);
        uim.menu.Activate("Communications");
    }

    void OnHypeDone() {
        if (Stage(OBS.NEW_PRODUCT)) {
            StartCoroutine(Delay(delegate(GameObject obj) {
                MentorMessages(new string[] {
                    "Great! Your company has some hype now.",
                    "Hype is critical to your products' success, and it fades out over time. So keep it up.",
                    string.Format("You're not much of a business if you haven't got anything to sell. Let's create a {0}.", ConceptHighlight("product")),
                    string.Format("To start creating a product, tap the {0} button below.", MenuHighlight("New Product"))
                });
                UIManager.Instance.menu.Activate("New Product");
            }, 1f));
        }
    }

    // Checks if it is appropriate to execute the specified stage,
    private bool Stage(OBS stage) {
        if (data.obs == stage - 1) {
            data.obs = stage;
            return true;
        }
        return false;
    }

    // Triggered whenever a window or tab is opened.
    public void OnScreenOpened(string name) {
        Debug.Log(name + " opened");
        switch(name) {

            case "Manage Communications":
                if (Stage(OBS.OPENED_HYPE)) {
                    MentorMessages(new string[] {
                        "Here you can choose from a few different kinds of promos to run.",
                        "For now you only have a couple to choose from, but more will become available over time."
                    });
                }
                break;

            case "New Product":
                if (Stage(OBS.OPENED_NEW_PRODUCT)) {
                    MentorMessages(new string[] {
                        string.Format("Products are created by combining two {0}.", ConceptHighlight("product types")),
                        string.Format("Some combinations work well and give {0}. Some don't.", ConceptHighlight("bonuses")),
                        string.Format("You will have to {0} and experiment with different combinations.", SpecialHighlight("innovate")),
                        "Right now you only have a few types available, but that will change over time.",
                        "Pick two product types and hit the button below to start developing the product."
                    });
                }
                break;

            case "Recruiting":
                if (Stage(OBS.OPENED_RECRUITING)) {
                    MentorMessages(new string[] {
                        "Here is where you can recruit some new candidates to hire.",
                        "The more employees you have, the more opportunities you have when developing new products.",
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

            case "Research":
                if (!data.ob.RESEARCH_OPENED) {
                    MentorMessages(new string[] {
                        string.Format("Here you can assign your employees as {0}.", ConceptHighlight("researchers")),
                        "A researcher's skill spends on their cleverness and productivity.",
                        "Once you assign a researcher, they will start generating research points for you!",
                        string.Format("You can spend these research points to purchase new {0}.", ConceptHighlight("technologies")),
                        string.Format("New technologies can unlock new {0}, {1}, and provide other bonuses.", ConceptHighlight("product types"), ConceptHighlight("special projects")),
                        "Don't neglect research! Stay ahead of the competition!"
                    });
                    data.ob.RESEARCH_OPENED = true;
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
                        "Now you are in the zone to build something great.",
                        "Collect as many product points as you can.",
                        "There are \n:DESIGN: [c][0078E1]design[-][/c],\n:ENGINEERING: [c][0078E1]engineering[-][/c], or\n:MARKETING: [c][0078E1]marketing[-][/c]\nproduct points available.",
                        "Larger globs are worth more points.",
                        "Your employees will take turns collecting points.",
                        string.Format("They will last a limited amount of time depending on their {0}.", ConceptHighlight("productivity")),
                        string.Format("Sometimes you will see {0} to help your employees out.", ConceptHighlight("powerups")),
                        "You may encounter \n:COFFEE: [c][0078E1]coffee[-][/c] or\n:INSIGHT: [c][0078E1]insight[-][/c]\npowerups.",
                        string.Format("The {0} your employees are, the more powerups you'll see.", ConceptHighlight("happier")),
                        string.Format("Finally, you may encounter {0} which will tire your employees out and cost you hard-earned points.", ConceptHighlight("hazards")),
                        "You may encounter \n:BLOCK: [c][0078E1]creative blocks[-][/c],\n:BUG: [c][0078E1]bugs[-][/c], or\n:OUTRAGE: [c][0078E1]outrage[-][/c]\nhazards.",
                        string.Format("The more {0} there is towards your company, the more likely you are to encounter hazards.", ConceptHighlight("outrage")),
                        string.Format("When all your employees have gone, your product will be released to {0}!", SpecialHighlight("The Market"))
                    });
                }, 1f));
            }
            Company.BeganProduct -= BeganProduct;
        }
    }

    void CompletedProduct(Product p, Company c) {
        if (c == data.company) {
            if (Stage(OBS.GAME_GOALS)) {
                StartCoroutine(Delay(delegate(GameObject obj) {
                    MentorMessages(new string[] {
                        "Now that you've got some experience with building products, [c][FC5656]The Board[-][/c] has asked me to explain their expectations for your company.",
                        string.Format("[c][FC5656]The Board[-][/c] sets {0} for you and requires that you [c][1A9EF2]expand your profits by 12% every year[-][/c].", ConceptHighlight("annual profit targets")),
                        "You [i]must[/i] hit these targets.",
                        "If [c][FC5656]The Board[-][/c] is unsatisfied your performance, they will [c][1A9EF2]dismiss you from the company[-][/c].",
                        "Making money is simple -  hype up your company and make more and better products!",
                        string.Format("Your {0} and {1} are shown in the bar below.", ConceptHighlight("current profit"), ConceptHighlight("target profit")),
                        string.Format("You can keep more detailed track of your profit and other accounting in the {0} menu item.", MenuHighlight("Accounting"))
                    });
                    UIManager.Instance.menu.Activate("Accounting");
                }, 3f));
            } else if (Stage(OBS.RESEARCH)) {
                StartCoroutine(Delay(delegate(GameObject obj) {
                    MentorMessages(new string[] {
                        string.Format("You've built a few products but that won't be enough to sustain long-term growth. You need to invest in cutting-edge {0}.", ConceptHighlight("research")),
                        string.Format("You can manage your Innovation Labs in the {0} button below.", MenuHighlight("Research")),
                        "There you can assign workers to conduct research.",
                        string.Format("They won't be available for product development, but over time they will generate {0}!", ConceptHighlight("research points"))
                    });
                    UIManager uim = UIManager.Instance;
                    uim.menu.Activate("Research");
                    uim.statusBar.researchLabel.gameObject.SetActive(true);
                }, 6f));
                Product.Completed -= CompletedProduct;
            } else if (c.products.Count > 2 && !data.ob.LOCATIONS_UNLOCKED) {
                MentorMessages(new string[] {
                    "It's time to start thinking about expanding to new locations.",
                    string.Format("{0} increase your access to {1}, making it easier to capture a larger market share.", ConceptHighlight("Locations"), ConceptHighlight("markets")),
                    "The more locations you have for a market, the more money you will make!",
                    "Some locations have special bonuses too.",
                    string.Format("Manage your locations in the {0} menu item.", MenuHighlight("Locations"))
                });
                UIManager.Instance.menu.Activate("Locations");
                data.ob.LOCATIONS_UNLOCKED = true;
            }
        }
    }

    void WorkerHired(AWorker w, Company c) {
        if (c == data.company) {
            if (!data.ob.HIRED_EMPLOYEE) {
                StartCoroutine(Delay(delegate(GameObject obj) {
                    MentorMessages(new string[] {
                        "Great, you have an employee now. See if you can build a new, better product."
                    });
                    UIManager.Instance.menu.Deactivate("New Product");
                    UIManager.Instance.menu.Activate("New Product");
                    data.ob.HIRED_EMPLOYEE = true;
                }, 1f));
            } else if (!data.ob.PERKS_UNLOCKED && c.workers.Count >= 3) {
                StartCoroutine(Delay(delegate(GameObject obj) {
                    MentorMessages(new string[] {
                        string.Format("Now that you have a few employees, you want to maximize their {0} and {1}.", ConceptHighlight("productivity"), ConceptHighlight("happiness")),
                        "Productive employees are easier to manage and happy employees can have valuable breakthroughs during product development and attract better talent.",
                        string.Format("A great way to accomplish this is through {0}. You can purchase and upgrade perks for your company through the {0} menu item.", ConceptHighlight("perks"), MenuHighlight("Perks"))
                    });
                    UIManager.Instance.menu.Activate("Perks");
                    data.ob.PERKS_UNLOCKED = true;
                }, 6f));
            }
        }
    }

    void PromoCompleted(Promo p) {
        if (!data.ob.HYPE_MINIGAME) {
            MentorMessages(new string[] {
                "Completing promotional campaigns gives you the opportunity to garner some allies in the media and hype up your company.",
                string.Format("{0} the puck and hit some {1} to get them on your side.", InteractHighlight("Flick"), ConceptHighlight("influencers")),
                "More influential influencers can, through their influence, cause a cascade effect and bring over others to your side!"
            });
            data.ob.HYPE_MINIGAME = true;
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
                    if (!data.ob.VERTICALS_UNLOCKED) {
                        MentorMessages(new string[] {
                            "Now that you've unlocked another vertical, you should consider saving up some capital to expand into it.",
                            string.Format("{0} provide access to new product types and technologies so you can {1} even further. Manage your verticals in the {2} menu item.", ConceptHighlight("Verticals"), SpecialHighlight("innovate"), MenuHighlight("Verticals"))
                        });
                        UIManager.Instance.menu.Activate("Verticals");
                        data.ob.VERTICALS_UNLOCKED = true;
                    }
                    break;
            }

        } else if (us.specialProjects.Count > 0 && !data.ob.SPECIALPROJECTS_UNLOCKED) {
            MentorMessages(new string[] {
                string.Format("Your first special project is available. {0} are one-off products which can have world-changing effects. In order to build one, you need to have built some prerequisite products beforehand.", ConceptHighlight("Special projects")),
                string.Format("Manage special projects in the {0} menu item.", MenuHighlight("Special Projects"))
            });
            UIManager.Instance.menu.Activate("Special Projects");
            data.ob.SPECIALPROJECTS_UNLOCKED = true;
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
        if (!data.ob.SYNERGY) {
            StartCoroutine(Delay(delegate(GameObject obj) {
                MentorMessages(new string[] {
                    string.Format("Wow! That product you just released is {0} with another one in The Market.", SpecialHighlight("synergetic")),
                    string.Format("Products with {0} make a lot more money when they're in The Market together.", SpecialHighlight("synergy")),
                    "Use your Entrepeneurial Sense and try to figure out what combinations work best!"
                });
            }, 6f));
            data.ob.SYNERGY = true;
            Company.Synergy -= Synergy;
        }
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

    private IEnumerator Delay(UIEventListener.VoidDelegate callback, float delay = 12f) {
        yield return StartCoroutine(GameTimer.Wait(delay));
        callback(null);
    }
}
