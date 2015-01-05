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

    // Disable the constructor.
    protected NarrativeManager() {}

    // The 3 starting cofounders you can choose from.
    public List<Founder> cofounders;

    private GameObject mentorMessagePrefab;

    void Awake() {
        mentorMessagePrefab = Resources.Load("UI/Narrative/Mentor Message") as GameObject;
        cofounders = new List<Founder>(Resources.LoadAll<Founder>("Founders/Cofounders"));
    }

    public void Load(GameData d) {
        data = d;
        ob = d.onboardingState;
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
        GameManager.Instance.playerCompany.founders.Add(cofounder);

        // Apply their bonuses.
        GameManager.Instance.ApplyEffectSet(cofounder.bonuses);

        // The cofounders you didn't pick start their own rival company.
        AICompany aic = ScriptableObject.CreateInstance<AICompany>();
        aic.name = "RIVAL CORP.";
        aic.founders = cofounders.Where(c => c != cofounder).ToList();
        return aic;
    }

    // Onboarding
    public void OpenedProducts() {
        if (!ob.PRODUCTS_OPENED) {
            ob.PRODUCTS_OPENED = true;
            MentorMessages(new string[] {
                "This window is where you manage products and their development.",
                "Jump over to the 'New' view to start creating a new product."
            }, true);
        }
    }
}
