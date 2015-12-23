using UnityEngine;
using System.Collections.Generic;

public class Mentor {
    private GameObject mentorMessagePrefab;

    public Mentor() {
        mentorMessagePrefab = Resources.Load("UI/Narrative/Mentor Message") as GameObject;
    }

    // A message from your mentor.
    public UIMentor Message(string message) {
        UIEventListener.VoidDelegate callback = delegate(GameObject obj) {
            obj.GetComponent<UIMentor>().Hide();
        };
        return Message(message, callback);
    }

    // A message from your mentor with a callback after tap.
    public UIMentor Message(string message, UIEventListener.VoidDelegate callback) {
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
    public void Messages(string[] messages) {
        UIEventListener.VoidDelegate callback = delegate(GameObject obj) {};
        Messages(messages, callback);
    }

    public void Messages(string[] messages, UIEventListener.VoidDelegate callback) {
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

        Message(messages[0], afterEach);
    }
}
