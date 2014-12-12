/*
 * The narrative manager handles the progression of the story.
 * These are usually one-off or special events.
 */

using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class NarrativeManager : Singleton<NarrativeManager> {

    // Disable the constructor.
    protected NarrativeManager() {}

    // Prerequests for transitioning to each stage.
    public PrereqSet globalPrereqs;
    public PrereqSet planetaryPrereqs;
    public PrereqSet galacticPrereqs;

    // The 3 starting cofounders you can choose from.
    public List<Founder> cofounders;

    private GameObject mentorMessagePrefab;

    void Awake() {
        mentorMessagePrefab = Resources.Load("UI/Narrative/Mentor Message") as GameObject;
        cofounders = new List<Founder>(Resources.LoadAll<Founder>("Founders/Cofounders"));
    }

    // A message from your mentor.
    public void MentorMessage(string message) {
        UIEventListener.VoidDelegate callback = delegate(GameObject obj) {
            obj.GetComponent<UIMentor>().Hide();
        };
        MentorMessage(message, callback);
    }

    public void MentorMessage(string message, UIEventListener.VoidDelegate callback) {
        GameObject alerts = UIRoot.list[0].transform.Find("Alerts").gameObject;

        GameObject msg = NGUITools.AddChild(alerts, mentorMessagePrefab);
        UIMentor mentor = msg.GetComponent<UIMentor>();
        mentor.message = message;

        UIEventListener.Get(mentor.box).onClick += delegate(GameObject obj) {
            callback(msg);
        };
    }

    // A list of messages to be shown in sequence (on tap).
    public void MentorMessages(List<string> messages) {
        int i = 0;
        UIEventListener.VoidDelegate callback = delegate(GameObject obj) {
            if (i < messages.Count - 1) {
                i++;
                obj.GetComponent<UIMentor>().message = messages[i];
            } else {
                obj.GetComponent<UIMentor>().Hide();
            }
        };
        MentorMessage(messages[0], callback);
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
}
