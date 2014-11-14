/*
 * The narrative manager handles the progression of the story.
 * These are usually one-off or special events.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NarrativeManager : Singleton<NarrativeManager> {

    // Disable the constructor.
    protected NarrativeManager() {}

    // Prerequests for transitioning to each stage.
    public PrereqSet globalPrereqs;
    public PrereqSet planetaryPrereqs;
    public PrereqSet galacticPrereqs;

    // The 3 starting cofounders you can choose from.
    public List<Founder> cofounders;

    void Awake() {
        cofounders = new List<Founder>(Resources.LoadAll<Founder>("Founders/Cofounders"));
    }


    // A message from your mentor.
    public void MentorMessage(string subject, string message) {
        UIManager.Instance.Email(
                "Babar <mentor@zcombo.com>",
                "Me <boss@foo.com>",
                subject,
                message);
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
