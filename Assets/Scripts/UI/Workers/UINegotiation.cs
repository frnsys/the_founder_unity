using UnityEngine;
using System.Linq;
using System.Collections.Generic;

// TODO refactor this, ugh
public class UINegotiation : UIWindow {
    public UILabel offerLabel;
    public UILabel probabilitySuccessLabel;
    public UILabel nameLabel;
    public UILabel turnsLabel;
    public UILabel personalInfoLabel;
    public UILabel personalInfoTitle;
    public MeshRenderer employee;

    private AWorker worker;
    private UIHireWorkers hireWorkersWindow;
    private float probAccept;
    private float minSalary;

    private struct DialogueOption {
        public string question;
        public Worker.Preference preference;
        public DialogueOption(string q) {
            question = q;
            preference = Worker.Preference.NONE;
        }
        public DialogueOption(Worker.Preference p) {
            question = null;
            preference = p;
        }
    }
    public List<UIButton> dialogueOptionButtons;
    private List<DialogueOption> dialogueOptions;
    private List<string> questions;
    private List<Worker.Preference> assertions;
    private List<Worker.Preference> knownAssertions;
    private List<Worker.Preference> suspectedAssertions;

    private int turns;
    private int offer_;
    public int offer {
        get { return offer_; }
        set {
            offer_ = value;
            offerLabel.text = string.Format("{0:C0}/mo", offer_);

            probAccept = AcceptanceProb(offer_);
            string color = "7BD6A4";
            if (probAccept <= 0.55) {
                color = "FF1E1E";
            } else if (probAccept <= 0.75) {
                color = "F1B71A";
            }
            probabilitySuccessLabel.text = string.Format("[c][{0}]{1:F0}% success[-][/c]", color, probAccept * 100);
        }
    }

    public void Setup(AWorker w, UIHireWorkers hww) {
        nameLabel.text = w.name;
        if (w.material != null)
            employee.material = w.material;

        if (GameManager.Instance.workerInsight) {
            personalInfoLabel.text = string.Format("- {0}", string.Join("\n- ", w.personalInfos.ToArray()));
            suspectedAssertions = new List<Worker.Preference>(w.personalInfo);
        } else {
            personalInfoLabel.gameObject.SetActive(false);
            personalInfoTitle.gameObject.SetActive(false);
            suspectedAssertions = new List<Worker.Preference>();
        }

        turns = 0;
        worker = w;
        hireWorkersWindow = hww;
        offer = 40000;
        minSalary = worker.MinSalaryForCompany(GameManager.Instance.playerCompany);

        questions = new List<string>(Worker.dialogueToDialogueMap.Keys);
        assertions = new List<Worker.Preference>(Worker.prefToDialogueMap.Keys);
        assertions.Remove(Worker.Preference.NONE);
        knownAssertions = new List<Worker.Preference>();
        dialogueOptions = new List<DialogueOption>();
        GenerateDialogueOptions();
    }

    public void Increment() {
        offer += 2000;
    }

    public void Decrement() {
        offer -= 2000;
        if (offer < 0)
            offer = 0;
    }

    public void MakeOffer() {
        UIManager.Instance.Confirm(string.Format("There will be a {0:C0} hiring fee (0.1%). Is that ok?", offer_ * 0.1f), delegate {
            if (Random.value <= probAccept) {
                hireWorkersWindow.HireWorker(worker);
                base.Close();
            } else {
                // TODO make this better
                UIManager.Instance.Alert("You can do better");
                worker.leaveProb += 0.15f;
                UpdateLeaveProb();
                TakeTurn();
            }
        } , null);
    }

    private float AcceptanceProb(int off) {
        float diff = off - minSalary;
        if (diff >= 0) {
            return 0.99f;
        } else {
            // the further below the min salary,
            // the less likely they are to accept
            float x = -diff/minSalary;
            return Mathf.Max(0, 0.99f - (((1/(-x-1)) + 1)*2));
        }
    }

    private void GenerateDialogueOptions() {
        dialogueOptions.Clear();

        // On first turn, generate all questions
        if (turns == 0) {
            List<string> qs = new List<string>(Worker.dialogueToDialogueMap.Keys);
            for (int i=0; i<3; i++) {
                int idx = Random.Range(0, qs.Count);
                string question = qs[idx];

                dialogueOptionButtons[i].transform.Find("Label").GetComponent<UILabel>().text = question;
                dialogueOptions.Add(new DialogueOption(question));
                qs.RemoveAt(idx);
            }

        // Otherwise, choose one question, two assertions
        } else {
            string q = questions[Random.Range(0, questions.Count)];
            dialogueOptionButtons[0].transform.Find("Label").GetComponent<UILabel>().text = q;
            dialogueOptions.Add(new DialogueOption(q));

            // Select assertions based on suspected assertions
            List<Worker.Preference> asrts = new List<Worker.Preference>(suspectedAssertions);
            for (int i=1; i<3; i++) {
                // Select from available assertions if no suspected ones are available
                if (asrts.Count == 0)
                    asrts = new List<Worker.Preference>(assertions);
                int idx = Random.Range(0, asrts.Count);
                Worker.Preference key = asrts[idx];

                dialogueOptionButtons[i].transform.Find("Label").GetComponent<UILabel>().text = Worker.prefToDialogueMap[key];
                dialogueOptions.Add(new DialogueOption(key));
                asrts.RemoveAt(idx);
            }
        }
    }

    public void ChooseDialogue(GameObject clicked) {
        int idx = dialogueOptionButtons.IndexOf(clicked.GetComponent<UIButton>());
        DialogueOption selected = dialogueOptions[idx];

        // Question
        if (selected.question != null) {
            Dictionary<Worker.Preference, string[]> responseOptions = Worker.dialogueToDialogueMap[selected.question];
            List<Worker.Preference> validKeys = new List<Worker.Preference>();
            foreach (Worker.Preference p in responseOptions.Keys) {
                if (worker.personalInfo.Contains(p) && !knownAssertions.Contains(p)) {
                    validKeys.Add(p);
                }
            }
            if (validKeys.Count == 0) {
                validKeys.Add(Worker.Preference.NONE);
            }
            Worker.Preference key = validKeys[Random.Range(0, validKeys.Count)];
            string[] responses = responseOptions[key];
            string response = responses[Random.Range(0, responses.Length)];

            UIManager.Instance.Alert(response);
            questions.Remove(selected.question);

            if (key != Worker.Preference.NONE)
                suspectedAssertions.Add(key);

        // Assertion
        } else {
            // Success
            if (worker.personalInfo.Contains(selected.preference)) {
                // TODO better dialogue here
                UIManager.Instance.Alert("Amazing!");
                minSalary *= Worker.prefToDiscountMap[selected.preference];
                knownAssertions.Add(selected.preference);
                if (suspectedAssertions.Contains(selected.preference))
                    suspectedAssertions.Remove(selected.preference);

                // Recalculate everything
                offer = offer_;
                worker.leaveProb -= 0.1f;
                if (worker.leaveProb < 0.05f)
                    worker.leaveProb = 0.05f;
                UpdateLeaveProb();
            } else {
                UIManager.Instance.Alert("That's not that important to me");
                worker.leaveProb += 0.15f;
                UpdateLeaveProb();
            }
            assertions.Remove(selected.preference);
        }

        TakeTurn();
        GenerateDialogueOptions();
    }

    private void TakeTurn() {
        if (Random.value <= worker.leaveProb) {
            // The worker goes off the market for a bit.
            worker.offMarketTime = 4;
            UIManager.Instance.Alert("Your offers were too low. I've decided to take a position somewhere else.");
            hireWorkersWindow.RemoveWorker(worker);
            base.Close();
        }
        worker.leaveProb += 0.05f;
        UpdateLeaveProb();
        turns++;
    }

    private void UpdateLeaveProb() {
        turnsLabel.text = string.Format("{0:F0}% chance to leave", worker.leaveProb * 100);
    }

    public void Close() {
        if (turns > 0) {
            UIManager.Instance.Confirm("Are you sure want to leave negotiations? Your turns will take some time to reset.", delegate {
                base.Close();
            } , null);
        } else {
            base.Close();
        }
    }
}