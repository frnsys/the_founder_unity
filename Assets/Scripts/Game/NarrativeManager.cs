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
        public bool FIRST_PRODUCT_LAUNCHED;
        public bool FIRST_WORKER_HIRED;
        public bool MARKET_UNLOCKED;
        public bool RESEARCH_UNLOCKED;
        public bool PRODUCT_COMBOS_UNLOCKED;
        public bool RESEARCH_OPENED;
        public bool COMMS_UNLOCKED;
        public bool VERTICALS_UNLOCKED;
        public bool WORKER_LIMIT_REACHED;
    }

    // Disable the constructor.
    protected NarrativeManager() {}

    // The 3 starting cofounders you can choose from.
    public List<Founder> cofounders;

    private GameObject mentorMessagePrefab;

    void Awake() {
        mentorMessagePrefab = Resources.Load("UI/Narrative/Mentor Message") as GameObject;
        cofounders = new List<Founder>(Resources.LoadAll<Founder>("Founders/Cofounders"));
    }

    void OnEnable() {
        UIWindow.WindowOpened += OnScreenOpened;
        UIWindow.TabOpened    += OnScreenOpened;
    }

    void OnDisable() {
        UIWindow.WindowOpened -= OnScreenOpened;
        UIWindow.TabOpened    -= OnScreenOpened;
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

    public AICompany SelectCofounder(Founder cofounder) {
        // Add the cofounder to the player's company.
        data.company.founders.Add(cofounder);

        // Apply their bonuses.
        GameManager.Instance.ApplyEffectSet(cofounder.bonuses);

        // The cofounders you didn't pick start their own rival company.
        AICompany aic = ScriptableObject.CreateInstance<AICompany>();
        aic.name = "RIVAL CORP.";
        aic.founders = cofounders.Where(c => c != cofounder).ToList();
        return aic;
    }



    /*
     * ==========================================
     * Onboarding ===============================
     * ==========================================
     */

    private enum OBS {
        START,
        COFOUNDER_INTRO,
        COFOUNDER_SELECT,
        COFOUNDER_OUTRO,
        GAME_INTRO,
        OPENED_PRODUCTS,
        OPENED_NEW_PRODUCT,
        OPENED_COMPANY,
        OPENED_INFRASTRUCTURE,
        OPENED_PRODUCTS_W_INFRA,
        OPENED_NEW_PRODUCT_W_INFRA,
        OPENED_NEW_PRODUCT_POINTS,
        STARTED_PRODUCT,
        MANAGE_PRODUCTS,
        OPENED_EMPLOYEES,
        OPENED_HIRE_EMPLOYEE,
        END
    }
    private OBS obs = OBS.START;

    // Setup the starting game state for onboarding.
    public void InitializeOnboarding() {
        obs = OBS.START;

        // Listen to some events.
        Product.Completed += OnProductCompleted;
        GameEvent.EventTriggered += OnEvent;
        Company.WorkerHired += OnWorkerHired;

        // Show the game intro.
        Intro();
    }

    void OnEvent(GameEvent ev) {
        if (data.company.OpinionEvents.Count > 0 && data.company.opinion.value < 0 && !ob.COMMS_UNLOCKED) {
            MentorMessages(new string[] {
                "Yikes. People aren't too happy with our company. The public opinion towards our company can hurt how well our products perform in the market.",
                "Fortunately, we have the techniques of PUBLIC RELATIONS to fend off egregious defamatory remarks.",
                "We can run our own promotions and advertisements to spin public opinion. You can manage these in the 'Communications' window."
            }, true);
            ob.COMMS_UNLOCKED = true;
        }
    }

    void OnWorkerHired(Worker w, Company c) {
        if (c == data.company) {
            if (!ob.FIRST_WORKER_HIRED) {
                MentorMessages(new string[] {
                    "You've hired your first employee! Employees help you develop better products by contributing their skills.",
                    "Employees also have a level of happiness and productivity, which are interrelated. Generally, happy employees are more productive.",
                    "More productive employees help you build products faster. You should maintain peak efficiency!"
                }, true);
                ob.FIRST_WORKER_HIRED = true;

            } else if (c.workers.Count > 3 && !ob.MARKET_UNLOCKED) {
                MentorMessages(new string[] {
                    "Now that your office is buzzing with a few employees, you should try to keep them as happy and productive as possible.",
                    "You also want to cultivate an office environment and CULTURE that is attractive to future employees.",
                    "You can purchase perks and equipment to keep your employees efficient and satisfied in The Market."
                }, true);
                ob.MARKET_UNLOCKED = true;

            } else if (c.remainingSpace == 0 && !ob.WORKER_LIMIT_REACHED) {
                MentorMessages(new string[] {
                    "It looks like you're running out of space for more employees.",
                    "It might be time to expand to another location.",
                    "New locations provide room for more employees and infrastructure, access to new markets, and sometimes other bonuses as well.",
                    "You can expand to new locations in the 'Company' window.",
                    "More locations will become available over time."
                }, true);
                ob.WORKER_LIMIT_REACHED = true;
            }
        }
    }

    void OnProductCompleted(Product p, Company c) {
        if (c == data.company) {
            if (!ob.FIRST_PRODUCT_LAUNCHED) {
                MentorMessages(new string[] {
                    "Excellent! Your first product is completed and has been released to the world.",
                    "The success of your product depends on a number of factors, most of all the quality of its design, engineering, and marketing.",
                    "Some products will earn you lots of money, some will fizzle out and be ignored.",
                    "If you see that your product isn't doing so well, you should shut it down and put that infrastructure to better use!"
                }, true);
                ob.FIRST_PRODUCT_LAUNCHED = true;
            }

            if (c.products.Count >= 3 && !ob.PRODUCT_COMBOS_UNLOCKED) {
                MentorMessages(new string[] {
                    "You've developed a few products. But we need to INNOVATE to really get ahead.",
                    "Why don't you try combining different products?",
                    "Some combinations might be real hits. Some might flounder.",
                    "Now that you've gotten the hang of it, why don't you try releasing a few more products.",
                    "I'll come back when you're ready for the next step."
                }, true);
                data.maxProductTypes = 2;
                ob.PRODUCT_COMBOS_UNLOCKED = true;
            }

            if (c.products.Count >= 5 && !ob.RESEARCH_UNLOCKED) {
                MentorMessages(new string[] {
                    "The world is changing and we need to change with it. We need to INNOVATE even harder.",
                    "An internal Innovation Labs has started up to research bleeding edge technologies.",
                    "We can use these earth-shattering technologies to create even more paradigm-shifting products.",
                    "Check it out when you get the chance."
                }, true);
                ob.RESEARCH_UNLOCKED = true;
            }

            if (c.products.Count >= 7 && !ob.VERTICALS_UNLOCKED) {
                MentorMessages(new string[] {
                    "Websites are great and all, but we should try to capture the entire end-to-end digital experience.",
                    "If you want to expand the kinds of products you can develop, you have to expand into new VERTICALS.",
                    "VERTICALS give you access to new products and new technologies.",
                    "You've got enough experience to expand into the Hardware vertical.",
                    "Expansion can be expensive, but it is necessary for your growth.",
                    "You can add on new verticals through the 'Company' window."
                }, true);
                data.unlocked.verticals.Add( Vertical.Load("Hardware") );
                ob.VERTICALS_UNLOCKED = true;
            }
        }
    }

    public void CofounderIntro() {
        MentorMessages(new string[] {
            "Welcome to the world of entrepreneurs!",
            "You have joined a class of accomplished, impactful people. But you need to earn that title.",
            "Starting a company is no small task - you'll need some help.",
            "As your mentor, I'll give you advice and guidance, but you also need someone working directly alongside you.",
            "You need a cofounder."
        }, true);
    }

    public void CofounderSelection() {
        MentorMessages(new string[] {
            "Here are a few entrepreneurs I've worked with in the past.",
            "They each bring something different to the table - it's up to you to decide who to move forward with."
        }, true);
    }

    public void CofounderOutro() {
        MentorMessages(new string[] {
            "Ok good choice.",
            "Let's begin."
        }, true);
    }

    public void Intro() {
        obs = OBS.GAME_INTRO;
        MentorMessages(new string[] {
            "Welcome to your office! It's not much right now, but that'll change.",
            "You've raised a bit of angel investment, but you should keep costs low to start.",
            "You're not much of a business if haven't got anything to sell. Let's create a product.",
            "Open the menu up and select 'Products'."
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

            case "Products":
                if (Stage(OBS.OPENED_PRODUCTS)) {
                    MentorMessages(new string[] {
                        "This window is where you manage products and their development.",
                        "Jump over to the 'New' view to start creating a new product."
                    }, true);

                } else if (Stage(OBS.OPENED_PRODUCTS_W_INFRA)) {
                    MentorMessages(new string[] {
                        "Now that you have the infrastructure, we can start a new product. Go to the 'New' tab."
                    }, true);

                } else if (Stage(OBS.MANAGE_PRODUCTS)) {
                    MentorMessages(new string[] {
                        "Here is where you can manage your developing and released products.",
                        "You can see how they're doing in the market. You can shut down their development or pull them from the market.",
                        "Shutting down or pulling products frees up the infrastructure they were using, so you can put it towards new and better uses.",
                        "This development could be faster. Let's hire an employee to help us out. From the menu, select 'Employees'."
                    }, true);

                }
                break;


            case "New Product":
                if (Stage(OBS.OPENED_NEW_PRODUCT)) {
                    MentorMessages(new string[] {
                        "Here you'll see the types of products you can currently develop.",
                        "Since you're just starting out, you can only build a couple basic things.",
                        "...But we can't build anything yet.",
                        "At the top you'll see what infrastructure you have available, which is none.",
                        "All products require some infrastructure to support them. Websites, for instance, need datacenters.",
                        "Let's close this window and open up the 'Company' window from the menu."
                    }, true);


                } else if (Stage(OBS.OPENED_NEW_PRODUCT_W_INFRA)) {
                    MentorMessages(new string[] {
                        "Select one of the product types here and hit 'Continue'."
                    }, true);
                }
                break;


            case "Company":
                if (Stage(OBS.OPENED_COMPANY)) {
                    MentorMessages(new string[] {
                        "Here is where you can manage various high-level aspects of your company.",
                        "You're just getting started so there's not a lot to see here. Jump over to the 'Manage' view."
                    }, true);
                }
                break;

            case "Manage":
                if (Stage(OBS.OPENED_INFRASTRUCTURE)) {
                    MentorMessages(new string[] {
                        "This is where you manage your company's presence throughout the world.",
                        "Right now you only have one location, but you can expand to others.",
                        "Locations allow you to hire more people, provide access to new markets, and expand the amount of infrastructure you can have.",
                        "Here we can start renting out datacenters at our San Francisco office.",
                        "Each piece of infrastructure incurs a monthly cost.",
                        "Add two datacenters and return to the 'Products' window."
                    }, true);
                }
                break;

            case "Workers":
                if (Stage(OBS.OPENED_EMPLOYEES)) {
                    MentorMessages(new string[] {
                        "Here is where you manage your employees.",
                        "Every great company is built on the sweat and tears of their founders, but employees are useful too.",
                        "They are important for developing high-quality products in reasonable amounts of time.",
                        "Right now, you've got no one working for you. Let's pop over to the 'Hire' tab to bring someone onboard."
                    }, true);
                }
                break;

            case "Hire Workers":
                if (Stage(OBS.OPENED_HIRE_EMPLOYEE)) {
                    MentorMessages(new string[] {
                        "There are a few candidates on the job market right now. Who's available changes over time.",
                        "Hiring employees is tricky. You have to make an offer and see if they accept it.",
                        "If your offers are too low, the candidate might accept a position elsewhere and become unavailable.",
                        "Try hiring someone."
                    }, true);
                }
                break;

            case "Research":
                if (!ob.RESEARCH_OPENED) {
                    MentorMessages(new string[] {
                        "In this window, you can invest more into research per month and appoint a Head of Research.",
                        "The more clever your Head of Research is, the faster you'll make breakthrough discoveries!"
                    }, true);
                    ob.RESEARCH_OPENED = true;
                }
                break;

            default:
                break;
        }
    }

    // This is a catch-all which can be called manually to progress onboarding.
    public void ProgressOnboarding() {
        if (Stage(OBS.OPENED_NEW_PRODUCT_POINTS)) {
            MentorMessages(new string[] {
                "On this screen you can decide how much effort we want to put into different aspects of the product.",
                "Each point adds to development time.",
                "The more employees you have and the better their skills, the faster development will go, so you'll be able to afford more points.",
                "Some products are easier to create than other, so they're quicker to develop.",
                "You don't want products to take too long to develop because you're constantly burning through cash.",
                "For now, just add a point or two and get started."
            }, true);

        } else if (Stage(OBS.STARTED_PRODUCT)) {
            MentorMessages(new string[] {
                "Congratulations! You've started developing your first product.",
                "When it's finished and released into the market, you'll start earning some revenue.",
                "Let's take a look at how development is going. Open up the 'Products' window again."
            }, true);
        }
    }
}
