/*
 * The narrative manager handles the progression of the story.
 * These are usually one-off or special events.
 */

using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class NarrativeManager : Singleton<NarrativeManager> {
    private GameData data;
    private OnboardingState ob;

    [System.Serializable]
    public struct OnboardingState {
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
    public UIMentor MentorMessage(string message, bool interrupt = false) {
        UIEventListener.VoidDelegate callback = delegate(GameObject obj) {
            obj.GetComponent<UIMentor>().Hide();
        };
        return MentorMessage(message, callback, interrupt);
    }

    // A message from your mentor with a callback after tap.
    public UIMentor MentorMessage(string message, UIEventListener.VoidDelegate callback, bool interrupt = false) {
        GameObject alerts = UIRoot.list[0].transform.Find("Alerts").gameObject;

        GameObject msg = NGUITools.AddChild(alerts, mentorMessagePrefab);
        UIMentor mentor = msg.GetComponent<UIMentor>();
        mentor.message = message;

        if (interrupt)
            mentor.Interrupt();

        UIEventListener.Get(mentor.box).onClick += delegate(GameObject obj) {
            callback(msg);
        };

        return mentor;
    }

    // A list of messages to be shown in sequence (on tap).
    public void MentorMessages(string[] messages, bool interrupt = false) {
        UIEventListener.VoidDelegate callback = delegate(GameObject obj) {};
        MentorMessages(messages, callback, interrupt);
    }

    public void MentorMessages(string[] messages, UIEventListener.VoidDelegate callback, bool interrupt = false) {
        int i = 0;
        UIEventListener.VoidDelegate afterEach = delegate(GameObject obj) {
            if (i < messages.Length - 1) {
                i++;
                obj.GetComponent<UIMentor>().message = messages[i];
            } else {
                obj.GetComponent<UIMentor>().Hide();
                callback(obj);
            }
        };
        MentorMessage(messages[0], afterEach, interrupt);
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
        uim.menu.Deactivate("Special Projects");
        uim.menu.Deactivate("Infrastructure");
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
            MentorMessages(new string[] {
                "Congratulations! This is your first write-up in a major publication.",
                "This kind of mention has driven up the hype for your company.",
                "Hype is central to your company's success. A hyped company's products sell much better.",
                "But hype is always deflating. Keep hyping your company and shape it's public image by launching promotional campaigns from the [c][4B2FF8]Communications[-][/c] menu item.",
                "But note that some press can be negative, and hurt your company's image. Consumers aren't going to buy your products if they disagree with your decisions. Consumers forget things over time, but promotional campaigns can also help expedite that process."
            }, true);
            UIManager uim = UIManager.Instance;
            uim.statusBar.hypeLabel.gameObject.SetActive(true);
            uim.menu.Activate("Communications");

        } else if (ev.name == "RIVALCORP Founded") {
            MentorMessages(new string[] {
                "Now seems like a good time to mention that you have some competition.",
                "Competitors will copy your successful products and steal market share from away from you. If you keep your products better than them, you won't have to worry.",
                "Competitors will also poach your employees. This kind of activity can drive wages up. This is a lose-lose for everyone - other companies can be cooperative when it comes to dealing with this."
            }, true);
        }
    }

    public void Intro() {
        obs = OBS.GAME_INTRO;
        MentorMessages(new string[] {
            "Welcome to your office! You're just starting out, so you'll work from your apartment for now.",
            "Right now it's just your cofounder in the office, but eventually you'll have a buzzing hive of talented employees.",
            "[c][FC5656]The Board[-][/c] sets quarterly profit targets for you and requires that you [c][1A9EF2]expand your profits by 12% every quarter[-][/c]. You [i]must[/i] hit these targets.",
            "If [c][FC5656]The Board[-][/c] is unsatisfied your performance, they will dismiss you from the company.",
            "You're not much of a business if haven't got anything to sell. Let's create a product.",
            "Open the menu up and select [c][4B2FF8]New Product[-][/c]."
        }, true);
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
                    }, true);

                } else if (Stage(OBS.INFRASTRUCTURE)) {
                    MentorMessages(new string[] {
                        "Products require different kinds of infrastructure to support their growth.",
                        "They might require [c][82D6FD]datacenters[-][/c], [c][82D6FD]factories[-][/c], [c][82D6FD]labs[-][/c], or [c][82D6FD]studios[-][/c]. All product types have some minimum necessary infrastructure before you can use them.",
                        "As products grow in the market, they will use more infrastructure. If there isn't any infrastructure available, products will stop growing and you'll be leaving money on the table.",
                        "You can buy more infrastructure in the [c][4B2FF8]Infrastructure[-][/c] menu item. There's a cost to setup the infrastructure, and then a rental cost every month after.",
                        "The amount of infrastructure you can buy is limited, but you can increase this limit by expanding to new locations - some locations are better for certain infrastructure.",
                        "Remember that you can shutdown products in the [c][4B2FF8]Products[-][/c] menu item to reclaim their infrastructure."
                    }, true);
                    UIManager.Instance.menu.Activate("Infrastructure");
                }
                break;

            case "Recruiting":
                if (Stage(OBS.OPENED_RECRUITING)) {
                    MentorMessages(new string[] {
                        "Here is where you can recruit some new candidates to hire.",
                        "There are a few different recruiting methods which vary in cost and quality of candidate.",
                        "For now you should probably keep costs low and go by word-of-mouth. Give it a try. You'll be notified about the candidates when recruiting has finished."
                    }, true);
                    UIManager.Instance.menu.Activate("Recruiting");
                }
                break;

            case "Hiring":
                if (Stage(OBS.OPENED_HIRING)) {
                    MentorMessages(new string[] {
                        "Here are the candidates from your recruiting effort.",
                        "To hire an candidate, you must give them an offer they find acceptable. You have three tries before they take an offer elsewhere.",
                        "The minimum salary an candidate is willing to accept is affected by a few things. If your employees are happier than they are, candidates will be willing to take a lower salary. General global salary averages and their current salary, if they are employed, will also have an impact.",
                        "Don't forget - you're fighting to keep a high profit margin - so negotiate with that in mind!"
                    }, true);
                }
                break;

            default:
                break;
        }
    }

    void BeganProduct(Product p, Company c) {
        if (c == data.company) {
            if (Stage(OBS.STARTED_PRODUCT)) {
                MentorMessages(new string[] {
                    "Great! You've started developing your first product.",
                    "You need employees at their desks so they can work on the product.",
                    "Productive workers will diligently head to their desk, but others must be nudged. [c][56FB92]double-tap[-][/c] an employee to get them to go to their desk.",
                    "To develop the product, capture the value your employees produce by [c][56FB92]tapping[-][/c] on the icons that appear above them.",
                    "Employees can produce [c][82D6FD]design[-][/c], [c][82D6FD]engineering[-][/c], or [c][82D6FD]marketing[-][/c] points for your products. Certain products rely more on heavily on some of these features. Happy employees may have [c][FC5656]breakthroughs[-][/c], in which case they produce all three.",
                    "Try to get bonus multipliers by chaining feature points together!"
                }, true);
            }
            Company.BeganProduct -= BeganProduct;
        }
    }

    void CompletedProduct(Product p, Company c) {
        if (c == data.company) {
            if (Stage(OBS.COMPLETED_PRODUCT)) {
                MentorMessages(new string[] {
                    "Congratulations! You've completed your first product.",
                    "It will start generating revenue, depending on its final design, engineering, and marketing values.",
                    "Next to the product's revenue is the market share of the product. Products make more money if they have a larger share of the market. You can increase your market share by expanding to new locations and building better products.",
                    "To make better products you need to assemble a talented team.",
                    "You can search for candidates by opening [c][4B2FF8]Recruiting[-][/c] in the menu.",
                }, true);
            } else if (Stage(OBS.OTHER_PRODUCT_ASPECTS)) {
                MentorMessages(new string[] {
                    "There are a few other factors which can affect a product's in-market performance.",
                    "All products will be affected by the state of the economy. During downturns, consumers spend less and so your products will generate less revenue. In boom times, the opposite is true.",
                    "Sometimes you may have a brilliant product combination, but lack the necessary technology to really make it work. In this case, the product just won't perform as well - research the missing technology and try again.",
                    "Finally, some products compliment each other when they are in the market together. The synergy of these products will cause them both to sell a lot better. You'll have to experiment to see what works!"
                }, true);
            } else if (Stage(OBS.RESEARCH)) {
                MentorMessages(new string[] {
                    "You've built a few products but that won't be enough to sustain long-term growth. You need to invest in cutting-edge research.",
                    "You can manage your research budget in the [c][4B2FF8]Accounting[-][/c] menu item, which influences how much research points you generate.",
                    "Spend research points to purchase new technologies in the [c][4B2FF8]Research[-][/c] menu item. New technologies can unlock new product types, special projects, and provide other bonuses. Stay ahead of the competition!"
                }, true);
                UIManager uim = UIManager.Instance;
                uim.statusBar.researchLabel.gameObject.SetActive(true);
                uim.menu.Activate("Research");
                Product.Completed -= CompletedProduct;
            }
        }
    }

    void WorkerHired(Worker w, Company c) {
        if (c == data.company && !ob.PERKS_UNLOCKED && c.workers.Count >= 2) {
            MentorMessages(new string[] {
                "Now that you have a few employees, you want to maximize their productivity and happiness.",
                "Productive employees are easier to manage and happy employees can have valuable breakthroughs during product development and attract better talent.",
                "A great way to accomplish this is through perks. You can purchase and upgrade perks for your company through the [c][4B2FF8]Perks[-][/c] menu item."
            }, true);
            UIManager.Instance.menu.Activate("Perks");
            ob.PERKS_UNLOCKED = true;
        }
    }

    void PromoCompleted(Promo p) {
        if (!ob.HYPE_MINIGAME) {
            MentorMessages(new string[] {
                "Completing promotional campaigns gives you the opportunity to garner some allies in the media. The more you have, the better your public image, and the greater the hype around your company.",
                "[c][56FB92]Flick[-][/c] the puck and hit some influencers to get them on your side. More influential influencers can, through their influence, cause a cascade effect and bring over others to your side!"
            }, true);
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
                    }, true);
                    break;
                case "Defense":
                    MentorMessages(new string[] {
                        "Building defense products may seem unethical, but they generally lead to lucrative government contract lump-sums which are invaluable for funding your continued expansion."
                    }, true);
                    break;
                case "Entertainment":
                    MentorMessages(new string[] {
                        "Promotional campaigns are great, but the most efficient way to manage public perception is through entertainment and media companies.",
                        "Entertainment products help consumers forget the dreariness or difficulty of their lives. Fortunately, these distractions also help them forget about your company's transgressions more quickly."
                    }, true);
                    break;
                default:
                    if (!ob.VERTICALS_UNLOCKED) {
                        MentorMessages(new string[] {
                            "Now that you've unlocked another vertical, you should consider saving up some capital to expand into it.",
                            string.Format("Verticals provide access to new product types and technologies so you can {0} even further. Manage your verticals in the [c][4B2FF8]Vertical[-][/c] menu item.", innovate)
                        }, true);
                        UIManager.Instance.menu.Activate("Verticals");
                        ob.VERTICALS_UNLOCKED = true;
                    }
                    break;
            }

        } else if (us.locations.Count > 0 && !ob.LOCATIONS_UNLOCKED) {
            MentorMessages(new string[] {
                "A new location is available for you to expand to. Locations allow you to increase your share of existing markets or establish a foothold in new ones, and also provide capacity for more infrastructure. Some locations have special bonuses too. Manage your locations in the [c][4B2FF8]Location[-][/c] menu item."
            }, true);
            UIManager.Instance.menu.Activate("Locations");
            ob.LOCATIONS_UNLOCKED = true;

        } else if (us.specialProjects.Count > 0 && !ob.SPECIALPROJECTS_UNLOCKED) {
            MentorMessages(new string[] {
                "Your first special project is available. Special projects are one-off products which can have world-changing effects. In order to build one, you need to have built some prerequisite products beforehand. Manage special projects in the [c][4B2FF8]Special Projects[-][/c] menu item."
            }, true);
            UIManager.Instance.menu.Activate("Special Projects");
            ob.SPECIALPROJECTS_UNLOCKED = true;
        }
    }

    void OfficeUpgraded(Office o) {
        if (o.type == Office.Type.Campus) {
            MentorMessages(new string[] {
                "Your company is impressively large now! But it could still be larger.",
                "It's harder to innovate on your own, but with all of your capital you can buy up other companies now. Manage these purchases through the [c][4B2FF8]Acquisitions[-][/c]"
            }, true);
            UIManager.Instance.menu.Activate("Acquisitions");
        }
    }

    string innovate = "[c][FFE587]i[-][4B2FF8]n[-]n[79ECDD]o[-][FC5656]v[-][EFB542]a[-][78E09E]t[-][E0FFFB]e[-][/c]";
}
