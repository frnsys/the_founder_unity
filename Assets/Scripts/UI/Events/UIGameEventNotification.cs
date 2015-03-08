using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections;

public class UIGameEventNotification: UIEffectAlert {
    public UIWidget title;
    public UILabel titleLabel;
    public UILabel fromLabel;
    public UILabel toLabel;
    public UIGrid actionGrid;
    public UITexture image;

    public GameObject actionPrefab;

    private GameEvent gameEvent_;
    public GameEvent gameEvent {
        get { return gameEvent_; }
        set {
            gameEvent_ = value;

            titleLabel.text = gameEvent_.name;
            bodyLabel.text = ProcessText(gameEvent_.description, false);
            fromLabel.text = ProcessText(gameEvent_.from, true);

            if (toLabel != null)
                toLabel.text = string.Format("founder@{0}.com", Slugify(companyName));

            if (gameEvent_.image != null)
                image.mainTexture = gameEvent_.image;

            Extend(bodyLabel.height);

            RenderActions();
            RenderEffects(gameEvent_.effects);

            // -1 because by default there is space for about 1 effect.
            Extend((int)((effectGrid.GetChildList().Count - 1) * effectGrid.cellHeight));
        }
    }

    string companyName;
    string aiCompanyName;
    string cofounderName;
    void OnEnable() {
        GameManager gm = GameManager.Instance;
        companyName = gm.playerCompany.name;
        aiCompanyName = gm.activeAICompanies[Random.Range(0, gm.activeAICompanies.Count)].name;

        // If we didn't go through onboarding, there are no founders.
        if (gm.playerCompany.founders.Count > 0)
            cofounderName = gm.playerCompany.founders[0].name;
        else
            cofounderName = "<testing cofounder>";

        Show();
    }

    private string ProcessText(string text, bool slugify) {
        if (slugify) {
            text = text.Replace("<PLAYERCOMPANY>", Slugify(companyName));
            text = text.Replace("<AICOMPANY>", Slugify(aiCompanyName));
            text = text.Replace("<COFOUNDER>", Slugify(cofounderName));
        } else {
            text = text.Replace("<PLAYERCOMPANY>", companyName);
            text = text.Replace("<AICOMPANY>", aiCompanyName);
            text = text.Replace("<COFOUNDER>", cofounderName);
        }
        return text;
    }

    private void RenderActions() {
        // Clear out existing action elements.
        while (actionGrid.transform.childCount > 0) {
            GameObject go = actionGrid.transform.GetChild(0).gameObject;
            UIEventListener.Get(go).onClick -= Close;
            NGUITools.DestroyImmediate(go);
        }

        // Render the available actions for this event.
        if (gameEvent_.actions.Length > 0) {
            foreach (GameEvent.Action action in gameEvent_.actions) {
                GameObject actionObj = NGUITools.AddChild(actionGrid.gameObject, actionPrefab);
                actionObj.GetComponent<UIEventActionButton>().action = action;

                // When this action button is clicked, close this notification.
                UIEventListener.Get(actionObj).onClick += Close;
            }
        } else {
            // Create a default "OK" action button.
            GameObject actionObj = NGUITools.AddChild(actionGrid.gameObject, actionPrefab);

            // When this action button is clicked, close this notification.
            UIEventListener.Get(actionObj).onClick += Close;
        }
    }

    protected override void Extend(int amount) {
        int current = body.bottomAnchor.absolute;
        body.bottomAnchor.Set(title.transform, 0, current - amount);

        // Adjust height of the popup shadow.
        float actionHeight = (actionGrid.GetChildList().Count * actionGrid.cellHeight) - (actionGrid.cellHeight/2) + 3;
        UIWidget shadow = transform.Find("Shadow").GetComponent<UIWidget>();
        shadow.bottomAnchor.Set(body.transform, 0, -actionHeight);
    }


    public string Slugify(string str) {
        str = str.ToLower();
        str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
        str = Regex.Replace(str, @"\s+", " ").Trim();
        str = Regex.Replace(str, @"\s", "_");
        return str;
    }
}


