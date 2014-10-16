using UnityEngine;
using System.Collections;

public class UIConsultancyItem : MonoBehaviour {
    private Consultancy consultancy_;
    public Consultancy consultancy {
        get { return consultancy_; }
        set {
            consultancy_ = value;
            transform.Find("Name").GetComponent<UILabel>().text = consultancy_.name;
            transform.Find("Description").GetComponent<UILabel>().text = consultancy_.description;
            transform.Find("Price").GetComponent<UILabel>().text = "$" + consultancy_.cost.ToString();
        }
    }
}
