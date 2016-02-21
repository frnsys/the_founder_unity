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
    private Mentor mentor;

    [System.Serializable]
    public struct OnboardingState {
        public bool INTRO;
        public bool PRODUCT_FAILED;
        public bool PRODUCT_CREATED_1;
        public bool PRODUCT_CREATED_2;
        public bool EMPLOYEE_HIRED;
        public bool HYPE_OPINION_UNLOCKED;
        public bool THE_BOARD_UNLOCKED;
        public bool PERKS_UNLOCKED;
        public bool VERTICALS_UNLOCKED;
        public bool LOCATIONS_UNLOCKED;
        public bool RESEARCH_UNLOCKED;
        public bool LOBBYING_UNLOCKED;
        public bool PRODUCTTYPES_UNLOCKED;
        public bool ACQUISITIONS_UNLOCKED;
        public bool SPECIALPROJECTS_UNLOCKED;
        public bool HIRING_OPENED;
        public bool RECRUITING_OPENED;
        public bool PROMOS_OPENED;
        public bool RESEARCH_OPENED;
        public bool SELECT_PRODUCTTYPES_OPENED;
    }

    // Disable the constructor.
    protected NarrativeManager() {}

    void Awake() {
        mentor = new Mentor();
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

    // Setup the starting game state for onboarding.
    public void InitializeOnboarding() {
        // Hide some menu and status bar items.
        UIManager uim = UIManager.Instance;

        if (!data.ob.SPECIALPROJECTS_UNLOCKED) {
            uim.menu.Deactivate("Special Projects");
        }

        if (!data.ob.LOCATIONS_UNLOCKED) {
            uim.menu.Deactivate("Locations");
        }

        if (!data.ob.PERKS_UNLOCKED) {
            Company.WorkerHired += WorkerHired;
            uim.menu.Deactivate("Perks");
        }

        if (!data.ob.VERTICALS_UNLOCKED) {
            uim.menu.Deactivate("Verticals");
        }

        if (!data.ob.EMPLOYEE_HIRED) {
            uim.menu.Deactivate("Employees");
        }

        if (!data.ob.LOBBYING_UNLOCKED) {
            uim.menu.Deactivate("Lobbying");
        }

        if (!data.ob.ACQUISITIONS_UNLOCKED) {
            UIOfficeManager.OfficeUpgraded += OfficeUpgraded;
            uim.menu.Deactivate("Acquisitions");
        }

        if (!data.ob.RESEARCH_UNLOCKED) {
            uim.menu.Deactivate("Research");
        }

        if (!data.ob.THE_BOARD_UNLOCKED) {
            uim.menu.Deactivate("Accounting");
        }

        if (!data.ob.PRODUCTTYPES_UNLOCKED) {
            uim.menu.Deactivate("Product Types");
        }

        if (!data.ob.PRODUCT_CREATED_1 || !data.ob.PRODUCT_CREATED_2) {
            MainGame.ProductCreated += OnProductCreated;
        }

        if (!data.ob.INTRO) {
            Intro();
            UIManager.Instance.menu.Deactivate("Recruiting");
        }

        if (!data.ob.PRODUCT_FAILED) {
            MainGame.ProductFailed += OnProductFailed;
        }

        if (!data.ob.HYPE_OPINION_UNLOCKED) {
            uim.Interlude.Hide("Hype");
            uim.Interlude.Hide("Opinion");
        }
        if (!data.ob.THE_BOARD_UNLOCKED) {
            uim.Interlude.Hide("Board");
            uim.Interlude.Hide("Profit");
        }

        MainGame.Done += OnYearEnded;
        GameEvent.EventTriggered += OnEvent;
        UnlockSet.Unlocked += OnUnlocked;
    }

    public void Intro() {
        mentor.Messages(new string[] {
            "Welcome to your office! You're just starting out, so you'll work from your apartment for now.",
            "Right now it's just your cofounder in the office, but eventually you'll have a buzzing hive of talented employees.",
            string.Format("Let's start out the year and make some {0}.", ConceptHighlight("products")),
            string.Format("To start creating a product, tap the {0} button below.", MenuHighlight("Start the Year"))
        });
        UIManager.Instance.menu.Activate("Start Year");
        data.ob.INTRO = true;
    }

    void OnYearEnded() {
        switch (data.year) {
            case 1:
                StartCoroutine(Delay(delegate(GameObject obj) {
                    mentor.Messages(new string[] {
                        "That's the end of the year!",
                        string.Format("The {0} shows you your performance for the year.", ConceptHighlight("annual report")),
                        "Between years you can manage your company.",
                        "You can hire new employees, purchase office perks, expand to new locations, and more.",
                        "These will improve your company's competitiveness.",
                        "Try hiring some new employees."
                    });
                    UIManager.Instance.menu.Activate("Recruiting");
                }, 2f));
                break;
            case 2:
                StartCoroutine(Delay(delegate(GameObject obj) {
                    mentor.Messages(new string[] {
                        string.Format("In order to {0}, you need access to more product types.", SpecialHighlight("innovate")),
                        string.Format("You can purchase more product types in the {0} menu item.", MenuHighlight("Product Types")),
                        "Some product types have prerequisites before they are available for purchase."
                    });
                    UIManager.Instance.menu.Activate("Product Types");
                    data.ob.PRODUCTTYPES_UNLOCKED = true;
                }, 3f));
                break;
            case 3:
                StartCoroutine(Delay(delegate(GameObject obj) {
                    mentor.Messages(new string[] {
                        string.Format("Now that you've got some experience with building products, it's time to introduce you to {0}.", ConceptHighlight("The Board")),
                        string.Format("The Board sets {0} for each year, which you are expected to meet.", ConceptHighlight("profit targets")),
                        "These profit targets grow by 12% each successive year.",
                        "If The Board is unsatisfied with your performance, you will be dismissed.",
                        string.Format("You can keep more detailed track of your performance and other accounting in the {0} menu item.", MenuHighlight("Accounting"))
                    });
                    UIManager.Instance.menu.Activate("Accounting");
                    UIManager.Instance.Interlude.Show("Profit");
                    UIManager.Instance.Interlude.Show("Board");
                    data.ob.THE_BOARD_UNLOCKED = true;
                }, 3f));
                break;
            case 4:
                UIManager.Instance.menu.Activate("Research");
                data.ob.RESEARCH_UNLOCKED = true;

                StartCoroutine(Delay(delegate(GameObject obj) {
                    mentor.Messages(new string[] {
                        "It's time to start thinking about expanding to new locations.",
                        string.Format("{0} increase your access to {1}, increasing your {2}.", ConceptHighlight("Locations"), ConceptHighlight("markets"), ConceptHighlight("market share")),
                        "Your market share greatly impacts your the revenue your products generate, so keep expanding!",
                        "Locations also expand the game board size - every five new locations adds a row (up to a maximum).",
                        "Some locations have special bonuses too.",
                        string.Format("Manage your locations in the {0} menu item.", MenuHighlight("Locations"))
                    });
                    UIManager.Instance.menu.Activate("Locations");
                    data.ob.LOCATIONS_UNLOCKED = true;
                }, 3f));
                break;
        }
    }

    void OnEvent(GameEvent ev) {
        if (ev.name == "New Company on the Scene") {
            StartCoroutine(Delay(delegate(GameObject obj) {
                mentor.Messages(new string[] {
                    "Congratulations! This is your first write-up in a major publication.",
                    string.Format("This kind of mention has driven up the :HYPE: {0} for your company.", ConceptHighlight("hype")),
                    string.Format("But note that some press can be negative. Bad publicity causes negative {0} towards your company.", ConceptHighlight("opinion")),
                    string.Format("Negative public opinion makes :OUTRAGE: {0} more likely.", ConceptHighlight("outrage")),
                    string.Format("But it's ok - the public {0} things over time.", ConceptHighlight("forgets")),
                    "And some products help them forget faster."
                });
                    UIManager.Instance.Interlude.Show("Hype");
                    UIManager.Instance.Interlude.Show("Opinion");
                data.ob.HYPE_OPINION_UNLOCKED = true;
            }, 2f));

        } else if (ev.name == "RIVALCORP Founded") {
            StartCoroutine(Delay(delegate(GameObject obj) {
                mentor.Messages(new string[] {
                    "Uh oh. Looks like you have some enemies.",
                    string.Format("Competitors will steal precious {0} from you,", ConceptHighlight("market share")),
                    "and they will also poach your employees.",
                    "This kind of activity can drive wages up.",
                    "This is a lose-lose for everyone, but other companies can be cooperative when it comes to dealing with this."
                });
            }));
        } else if (ev.name == "Data access") {
            StartCoroutine(Delay(delegate(GameObject obj) {
                mentor.Messages(new string[] {
                    "It looks like you're on the government's radar now.",
                    "This presents a great opportunity.",
                    string.Format("With some contacts in government, you can start {0} them.", ConceptHighlight("lobbying")),
                    string.Format("You can access this through the {0} menu item.", MenuHighlight("Lobbying"))
                });
                UIManager.Instance.menu.Activate("Lobbying");
                data.ob.LOBBYING_UNLOCKED = true;
            }));
        }
    }

    // Triggered whenever a window or tab is opened.
    public void OnScreenOpened(string name) {
        switch(name) {
            case "Recruiting":
                if (!data.ob.RECRUITING_OPENED) {
                    mentor.Messages(new string[] {
                        "Here is where you can recruit some new candidates to hire.",
                        "The more employees you have, the more opportunities you have when developing new products.",
                        "There are a few different recruiting methods which vary in cost and quality of candidate.",
                        "Give it a try!"
                    });
                    data.ob.RECRUITING_OPENED = true;
                }
                break;

            case "Hiring":
                if (!data.ob.HIRING_OPENED) {
                    mentor.Messages(new string[] {
                        "Here are the candidates from your recruiting effort.",
                        "To hire an candidate, you must give them an offer they find acceptable.",
                        string.Format("You can {0} with candidates to get a good deal.", ConceptHighlight("negotiate")),
                        "Ask them questions to learn their concerns,",
                        "then persuade them with your company's benefits.",
                        "They'll take a lower salary if you can convince them.",
                        "If negotiations drag on or don't look good for them, they may leave the table."
                    });
                    data.ob.HIRING_OPENED = true;
                }
                break;

            case "Research":
                if (!data.ob.RESEARCH_OPENED) {
                    mentor.Messages(new string[] {
                        string.Format("Welcome to your {0}!", SpecialHighlight("Innovation Lab")),
                        string.Format("Here you can purchase new {0}.", ConceptHighlight("technologies")),
                        string.Format("New technologies can unlock new {0}, {1}, and provide other bonuses.", ConceptHighlight("product types"), ConceptHighlight("special projects")),
                        string.Format("Some technologies have prerequisite technologies that you must purchase first."),
                        "Don't neglect research! Stay ahead of the competition!"
                    });
                    data.ob.RESEARCH_OPENED = true;
                }
                break;

            case "Promos":
                if (!data.ob.PROMOS_OPENED) {
                    mentor.Messages(new string[] {
                        "Here is where you can select a particular promotion to run.",
                        "You can also purchase new promotions here."
                    });
                    data.ob.PROMOS_OPENED = true;
                }
                break;

            case "Select Product Types":
                if (!data.ob.SELECT_PRODUCTTYPES_OPENED) {
                    mentor.Messages(new string[] {
                        "We have a lot of product types available to us, but we need to focus during the year.",
                        "Choose five product types to concentrate on this coming year."
                    });
                    data.ob.SELECT_PRODUCTTYPES_OPENED = true;
                }
                break;

            default:
                break;
        }
    }

    void OnProductCreated() {
        if (!data.ob.PRODUCT_CREATED_1) {
            StartCoroutine(Delay(delegate(GameObject obj) {
                mentor.Messages(new string[] {
                    "Great! You've launched your first product.",
                    "When you launch a product, you earn some money depending on the quality of the product.",
                    "The quality of the product greatly depends on your employees' skills.",
                    string.Format("Employees contribute \n:DESIGN: {0},\n:ENGINEERING: {1}, or\n:MARKETING: {2}\nskills.",
                        ConceptHighlight("design"),
                        ConceptHighlight("engineering"),
                        ConceptHighlight("marketing")),
                    string.Format("But it also depends on other factors such as your {0}.", ConceptHighlight("market share")),
                    "Some product type combinations do much better than others,",
                    "and some will even give permanent bonuses.",
                    string.Format("You'll have to {0} to discover them!", SpecialHighlight("innovate"))
                });
                data.ob.PRODUCT_CREATED_1 = true;
            }, 1f));
        } else if (!data.ob.PRODUCT_CREATED_2) {
            StartCoroutine(Delay(delegate(GameObject obj) {
                mentor.Messages(new string[] {
                    "Note the bar at the bottom of the screen.",
                    "This tells you how many turns you have remaining this year.",
                    "When you create a product, you use a turn.",
                    "Some other actions also cost turns.",
                    string.Format("The number of turns available depends on your employee :PRODUCTIVITY: {0}.", ConceptHighlight("productivity"))
                });
                data.ob.PRODUCT_CREATED_2 = true;
            }, 1f));
            MainGame.ProductCreated -= OnProductCreated;
        }
    }

    void OnProductFailed() {
        StartCoroutine(Delay(delegate(GameObject obj) {
            mentor.Messages(new string[] {
                "You just failed to launch a product.",
                "Some combinations of products are harder to create, so sometimes your employees mess up.",
                "They require higher levels of design, engineering, or marketing.",
                string.Format("A failed product leaves a :BUG: {0} behind, which takes up precious space.", ConceptHighlight("bug")),
                string.Format("You can debug it by {0} it, at the expense of a turn.", InteractHighlight("double-tap"))
            });
        }, 1f));
        MainGame.ProductFailed -= OnProductFailed;
    }

    void WorkerHired(AWorker w, Company c) {
        if (c == data.company) {
            if (!data.ob.EMPLOYEE_HIRED) {
                StartCoroutine(Delay(delegate(GameObject obj) {
                    mentor.Messages(new string[] {
                        "Great, you have an employee now. See if you can build a new, better product."
                    });
                    UIManager uim = UIManager.Instance;
                    uim.menu.Deactivate("Start Year");
                    uim.menu.Activate("Start Year");
                    uim.menu.Activate("Employees");
                    data.ob.EMPLOYEE_HIRED = true;
                }, 1f));
            } else if (!data.ob.PERKS_UNLOCKED && c.workers.Count >= 3) {
                StartCoroutine(Delay(delegate(GameObject obj) {
                    mentor.Messages(new string[] {
                        string.Format("Now that you have a few employees, you want to maximize their {0} and {1}.", ConceptHighlight("productivity"), ConceptHighlight("happiness")),
                        "Productive employees are easier to manage and happy employees can have valuable breakthroughs during product development and attract better talent.",
                        string.Format("A great way to accomplish this is through {0}. You can purchase and upgrade perks for your company through the {0} menu item.", ConceptHighlight("perks"), MenuHighlight("Perks"))
                    });
                    UIManager.Instance.menu.Activate("Perks");
                    data.ob.PERKS_UNLOCKED = true;
                    Company.WorkerHired -= WorkerHired;
                }, 6f));
            }
        }
    }

    void OnUnlocked(UnlockSet us) {
        if (us.verticals.Count > 0) {
            switch (us.verticals[0].name) {
                case "Finance":
                    mentor.Messages(new string[] {
                        "Financial products can be extremely useful in your growth strategy.",
                        string.Format("Through credit cards and other financial schemes, you can fund consumption well beyond consumers' means. Financial products will typically increase {0}, thus making all your products more profitable!", ConceptHighlight("consumer spending")),
                    });
                    break;
                case "Defense":
                    mentor.Messages(new string[] {
                        string.Format("Building defense products may seem unethical, but they generally lead to lucrative government contract {0} which are invaluable for funding your continued expansion.", ConceptHighlight("cash bonuses"))
                    });
                    break;
                case "Entertainment":
                    mentor.Messages(new string[] {
                        "Promotional campaigns are great, but the most efficient way to manage public perception is through entertainment and media companies.",
                        string.Format("Entertainment products help consumers forget the dreariness or difficulty of their lives. Fortunately, these distractions also help them {0} about your company's transgressions more quickly.", ConceptHighlight("forget"))
                    });
                    break;
                default:
                    if (!data.ob.VERTICALS_UNLOCKED) {
                        mentor.Messages(new string[] {
                            "Now that you've unlocked another vertical, you should consider saving up some capital to expand into it.",
                            string.Format("{0} provide access to new product types and technologies so you can {1} even further. Manage your verticals in the {2} menu item.", ConceptHighlight("Verticals"), SpecialHighlight("innovate"), MenuHighlight("Verticals"))
                        });
                        UIManager.Instance.menu.Activate("Verticals");
                        data.ob.VERTICALS_UNLOCKED = true;
                    }
                    break;
            }

        } else if (us.specialProjects.Count > 0 && !data.ob.SPECIALPROJECTS_UNLOCKED) {
            mentor.Messages(new string[] {
                string.Format("Your first special project is available. {0} are one-off products which can have world-changing effects. In order to build one, you need to have built some prerequisite products beforehand.", ConceptHighlight("Special projects")),
                string.Format("Manage special projects in the {0} menu item.", MenuHighlight("Special Projects"))
            });
            UIManager.Instance.menu.Activate("Special Projects");
            data.ob.SPECIALPROJECTS_UNLOCKED = true;
        }
    }

    void OfficeUpgraded(Office o) {
        if (o.type == Office.Type.Campus) {
            mentor.Messages(new string[] {
                "Your company is impressively large now! But it could still be larger.",
                string.Format("It's harder to {0} on your own, but with all of your capital you can {1} other companies now. Manage these purchases through the {2}", SpecialHighlight("innovate"), ConceptHighlight("aquire"), MenuHighlight("Acquisitions"))
            });
            UIManager.Instance.menu.Activate("Acquisitions");
            data.ob.ACQUISITIONS_UNLOCKED = true;
            UIOfficeManager.OfficeUpgraded -= OfficeUpgraded;
        }
    }

    public void GameLost() {
        mentor.Messages(new string[] {
            "Appalled by your inability to maintain the growth they are legally entitled to, the board has forced your resignation. You lose.",
            "But you don't really lose. You have secured your place in a class shielded from any real consequence or harm. You'll be fine. You could always found another company.",
            "GAME OVER."
        }, delegate(GameObject obj) {
            Application.LoadLevel("MainMenu");
        });
    }

    public void GameWon() {
        mentor.Messages(new string[] {
            "Slowly, over the years, the vast network of sensors and extremities of some unknown project were constructed and distributed, the public clamoring desperately for each shiny piece.",
            "Each piece - each gadget and every app - was always quietly listening and watching and collecting data in their pockets and on their bodies and in their heads.",
            "It only took the brilliant innovation of The Founder to unify this fabric of disparate technology into its grand, unified destiny, dubbed \"The Founder AI\".",
            "The Founder AI would generate the most detailed and accurate of consumer taste profiles, down to impossibly accurate predictions of behavior in every sphere of life.",
            "In its ultimate efficiency and rationality, and with the world's automated infrastructure at its reigns, it would allocate resources, command employees, manage campaigns, and so much more, at global optima previously only theorized.",
            "Profit could be indisputably maximized by the complex interactions of intricate mathematical models capturing even the smallest aspects of life and market society.",
            "On the inaugural day of The Founder AI, you receive an email.",
            "You are being let go.",
            "You, The Founder, are an obstruction to the company's continued expansion.",
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
