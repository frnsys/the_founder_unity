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
        public bool PERKS_UNLOCKED;
        public bool VERTICALS_UNLOCKED;
        public bool LOCATIONS_UNLOCKED;
        public bool SPECIALPROJECTS_UNLOCKED;
        public bool HYPE_MINIGAME;
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
        OPENED_RECRUITING,
        OPENED_HIRING,
        INFRASTRUCTURE,
        RESEARCH,
        OTHER_PRODUCT_ASPECTS
    }
    private OBS obs = OBS.START;

    // Setup the starting game state for onboarding.
    public void InitializeOnboarding() {
        obs = OBS.START;

        // Listen to some events.
        Company.BeganProduct += BeganProduct;
        Product.Completed += CompletedProduct;
        Company.WorkerHired += WorkerHired;
        Promo.Completed += PromoCompleted;
        GameEvent.EventTriggered += OnEvent;
        UnlockSet.Unlocked += OnUnlocked;
        UIOfficeManager.OfficeUpgraded += OfficeUpgraded;

        // Hide some menu and status bar items.
        UIManager uim = UIManager.Instance;
        uim.statusBar.hypeLabel.gameObject.SetActive(false);
        uim.statusBar.researchLabel.gameObject.SetActive(false);
        uim.menu.Deactivate("New Product");
        uim.menu.Deactivate("Accounting");
        uim.menu.Deactivate("Special Projects");
        uim.menu.Deactivate("Infrastructure");
        uim.menu.Deactivate("Products");
        uim.menu.Deactivate("Locations");
        uim.menu.Deactivate("Verticals");
        uim.menu.Deactivate("Acquisitions");
        uim.menu.Deactivate("Recruiting");
        uim.menu.Deactivate("Employees");
        uim.menu.Deactivate("Perks");
        uim.menu.Deactivate("Research");
        uim.menu.Deactivate("Communications");

        // Show the game intro.
        Intro();
    }

    void OnEvent(GameEvent ev) {
        if (ev.name == "First Product Launched") {
            StartCoroutine(Delay(delegate(GameObject obj) {
                MentorMessages(new string[] {
                    "Congratulations! This is your first write-up in a major publication.",
                    "This kind of mention has driven up the hype for your company.",
                    "Hype is central to your company's success. A hyped company's products sell much better.",
                    "But hype is always deflating. Keep hyping your company and shape it's public image by launching promotional campaigns from the [c][4B2FF8]Communications[-][/c] menu item.",
                    "But note that some press can be negative, and hurt your company's image.",
                    "Consumers aren't going to buy your products if they disagree with your decisions.",
                    "Consumers forget things over time, and promotional campaigns can also help expedite that process."
                });
                UIManager uim = UIManager.Instance;
                uim.statusBar.hypeLabel.gameObject.SetActive(true);
                uim.menu.Activate("Communications");
            }));

        } else if (ev.name == "RIVALCORP Founded") {
            StartCoroutine(Delay(delegate(GameObject obj) {
                MentorMessages(new string[] {
                    "Now seems like a good time to mention that you have some competition.",
                    "Competitors will copy your successful products and steal market share from away from you. If you keep your products better than them, you won't have to worry.",
                    "Competitors will also poach your employees. This kind of activity can drive wages up. This is a lose-lose for everyone - other companies can be cooperative when it comes to dealing with this."
                });
            }));
        }
    }
    private IEnumerator Delay(UIEventListener.VoidDelegate callback, float delay = 12f) {
        yield return new WaitForSeconds(delay);
        callback(null);
    }

    public void Intro() {
        obs = OBS.GAME_INTRO;
        MentorMessages(new string[] {
            "Welcome to your office! You're just starting out, so you'll work from your apartment for now.",
            "Right now it's just your cofounder in the office, but eventually you'll have a buzzing hive of talented employees.",
            "[c][FC5656]The Board[-][/c] sets annual profit targets for you and requires that you [c][1A9EF2]expand your profits by 12% every year[-][/c]. You [i]must[/i] hit these targets.",
            "If [c][FC5656]The Board[-][/c] is unsatisfied your performance, they will dismiss you from the company.",
            "You can keep track of your profit and other accounting in the [c][4B2FF8]Accounting[-][/c] menu item.",
            "You're not much of a business if haven't got anything to sell. Let's create a product.",
            "Open the menu up and select [c][4B2FF8]New Product[-][/c]."
        });
        UIManager.Instance.menu.Activate("New Product");
        UIManager.Instance.menu.Activate("Accounting");
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
                        "Products are created by combining two product types.",
                        "Some combinations work well and give bonuses. Some don't.",
                        string.Format("You will have to {0} and experiment with different combinations.", innovate),
                        "Right now you only have a few types available, but that will change over time.",
                        "Pick two product types and hit the button below to start developing the product."
                    });

                } else if (Stage(OBS.INFRASTRUCTURE)) {
                    MentorMessages(new string[] {
                        "Products require different kinds of infrastructure to support their growth.",
                        "They might require\n:DATACENTER: [c][82D6FD]datacenters[-][/c],\n:FACTORY: [c][82D6FD]factories[-][/c],\n:LAB: [c][82D6FD]labs[-][/c], or\n:STUDIO: [c][82D6FD]studios[-][/c].",
                        "All product types have some minimum necessary infrastructure before you can use them.",
                        "As products grow in the market, they will use more infrastructure. If there isn't any infrastructure available, products will stop growing and you'll be leaving money on the table.",
                        "You can buy more infrastructure in the [c][4B2FF8]Infrastructure[-][/c] menu item. There's a cost to setup the infrastructure, and then a rental cost every month after.",
                        "The amount of infrastructure you can buy is limited, but you can increase this limit by expanding to new locations - some locations are better for certain infrastructure.",
                        "Remember that you can shutdown products in the [c][4B2FF8]Products[-][/c] menu item to reclaim their infrastructure."
                    });
                    UIManager.Instance.menu.Activate("Infrastructure");
                    UIManager.Instance.menu.Activate("Products");
                }
                break;

            case "Recruiting":
                if (Stage(OBS.OPENED_RECRUITING)) {
                    MentorMessages(new string[] {
                        "Here is where you can recruit some new candidates to hire.",
                        "There are a few different recruiting methods which vary in cost and quality of candidate.",
                        "For now you should probably keep costs low and go by word-of-mouth. Give it a try. You'll be notified about the candidates when recruiting has finished."
                    });
                }
                break;

            case "Hiring":
                if (Stage(OBS.OPENED_HIRING)) {
                    MentorMessages(new string[] {
                        "Here are the candidates from your recruiting effort.",
                        "To hire an candidate, you must give them an offer they find acceptable. You have three tries before they take an offer elsewhere.",
                        "The minimum salary an candidate is willing to accept is affected by a few things. If your employees are happier than they are, candidates will be willing to take a lower salary. General global salary averages and their current salary, if they are employed, will also have an impact.",
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
                        "You need employees at their desks so they can work on the product.",
                        "Productive workers will diligently head to their desk, but others must be nudged. [c][56FB92]double-tap[-][/c] an employee to get them to go to their desk.",
                        "To develop the product, capture the value your employees produce by [c][56FB92]tapping[-][/c] on the icons that appear above them.",
                        "Employees can produce\n:DESIGN: [c][82D6FD]design[-][/c],\n:ENGINEERING: [c][82D6FD]engineering[-][/c], or\n:MARKETING: [c][82D6FD]marketing[-][/c]\npoints for your products.",
                        "Certain products rely more on heavily on some of these features. Happy employees may have :BREAKTHROUGH: [c][FC5656]breakthroughs[-][/c], in which case they produce all three.",
                        "Try to get bonus multipliers by chaining feature points together!"
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
                        "It will start generating revenue, depending on its final design, engineering, and marketing values.",
                        "Next to the product's revenue is the market share of the product. Products make more money if they have a larger share of the market.",
                        "You can increase your market share by expanding to new locations and building better products.",
                        "To make better products you need to assemble a talented team.",
                        "You can search for candidates by opening [c][4B2FF8]Recruiting[-][/c] in the menu.",
                    });
                    UIManager.Instance.menu.Activate("Recruiting");
                }, 3f));
            } else if (Stage(OBS.OTHER_PRODUCT_ASPECTS)) {
                StartCoroutine(Delay(delegate(GameObject obj) {
                    MentorMessages(new string[] {
                        "There are a few other factors which can affect a product's in-market performance.",
                        "All products will be affected by the state of the economy. During downturns, consumers spend less and so your products will generate less revenue. In boom times, the opposite is true.",
                        "Sometimes you may have a brilliant product combination, but lack the necessary technology to really make it work. In this case, the product just won't perform as well - research the missing technology and try again.",
                        "Finally, some products compliment each other when they are in the market together. The synergy of these products will cause them both to sell a lot better. You'll have to experiment to see what works!"
                    });
                }, 6f));
            } else if (Stage(OBS.RESEARCH)) {
                StartCoroutine(Delay(delegate(GameObject obj) {
                    MentorMessages(new string[] {
                        "You've built a few products but that won't be enough to sustain long-term growth. You need to invest in cutting-edge research.",
                        "You can manage your research budget in the [c][4B2FF8]Accounting[-][/c] menu item, which influences how much research points you generate.",
                        "Spend research points to purchase new technologies in the [c][4B2FF8]Research[-][/c] menu item. New technologies can unlock new product types, special projects, and provide other bonuses. Stay ahead of the competition!"
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
        if (c == data.company && !ob.PERKS_UNLOCKED && c.workers.Count >= 2) {
            StartCoroutine(Delay(delegate(GameObject obj) {
                MentorMessages(new string[] {
                    "Now that you have a few employees, you want to maximize their productivity and happiness.",
                    "Productive employees are easier to manage and happy employees can have valuable breakthroughs during product development and attract better talent.",
                    "A great way to accomplish this is through perks. You can purchase and upgrade perks for your company through the [c][4B2FF8]Perks[-][/c] menu item."
                });
                UIManager.Instance.menu.Activate("Perks");
                ob.PERKS_UNLOCKED = true;
            }, 6f));
        }
    }

    void PromoCompleted(Promo p) {
        if (!ob.HYPE_MINIGAME) {
            MentorMessages(new string[] {
                "Completing promotional campaigns gives you the opportunity to garner some allies in the media. The more you have, the better your public image, and the greater the hype around your company.",
                "[c][56FB92]Flick[-][/c] the puck and hit some influencers to get them on your side. More influential influencers can, through their influence, cause a cascade effect and bring over others to your side!"
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
                        "Through credit cards and other financial schemes, you can fund consumption well beyond consumers' means. Financial products will typically increase consumer spending, thus making all your products more profitable!",
                    });
                    break;
                case "Defense":
                    MentorMessages(new string[] {
                        "Building defense products may seem unethical, but they generally lead to lucrative government contract lump-sums which are invaluable for funding your continued expansion."
                    });
                    break;
                case "Entertainment":
                    MentorMessages(new string[] {
                        "Promotional campaigns are great, but the most efficient way to manage public perception is through entertainment and media companies.",
                        "Entertainment products help consumers forget the dreariness or difficulty of their lives. Fortunately, these distractions also help them forget about your company's transgressions more quickly."
                    });
                    break;
                default:
                    if (!ob.VERTICALS_UNLOCKED) {
                        MentorMessages(new string[] {
                            "Now that you've unlocked another vertical, you should consider saving up some capital to expand into it.",
                            string.Format("Verticals provide access to new product types and technologies so you can {0} even further. Manage your verticals in the [c][4B2FF8]Vertical[-][/c] menu item.", innovate)
                        });
                        UIManager.Instance.menu.Activate("Verticals");
                        ob.VERTICALS_UNLOCKED = true;
                    }
                    break;
            }

        } else if (us.locations.Count > 0 && !ob.LOCATIONS_UNLOCKED) {
            MentorMessages(new string[] {
                "A new location is available for you to expand to. Locations allow you to increase your share of existing markets or establish a foothold in new ones, and also provide capacity for more infrastructure. Some locations have special bonuses too. Manage your locations in the [c][4B2FF8]Location[-][/c] menu item."
            });
            UIManager.Instance.menu.Activate("Locations");
            ob.LOCATIONS_UNLOCKED = true;

        } else if (us.specialProjects.Count > 0 && !ob.SPECIALPROJECTS_UNLOCKED) {
            MentorMessages(new string[] {
                "Your first special project is available. Special projects are one-off products which can have world-changing effects. In order to build one, you need to have built some prerequisite products beforehand. Manage special projects in the [c][4B2FF8]Special Projects[-][/c] menu item."
            });
            UIManager.Instance.menu.Activate("Special Projects");
            ob.SPECIALPROJECTS_UNLOCKED = true;
        }
    }

    void OfficeUpgraded(Office o) {
        if (o.type == Office.Type.Campus) {
            MentorMessages(new string[] {
                "Your company is impressively large now! But it could still be larger.",
                "It's harder to innovate on your own, but with all of your capital you can buy up other companies now. Manage these purchases through the [c][4B2FF8]Acquisitions[-][/c]"
            });
            UIManager.Instance.menu.Activate("Acquisitions");
        }
    }

    string innovate = "[c][FC5656]i[-][FFC800]n[-][78E09E]n[-][79ECDD]o[-][4B2FF8]v[-][FD7EFF]a[-][FC5656]t[-][FFC800]e[-][/c]";

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
