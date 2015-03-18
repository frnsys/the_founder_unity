using UnityEngine;
using System.Collections.Generic;

public class UIUsedInfrastructure : MonoBehaviour {
    public GameObject[] objects;
    public UILabel datacenter;
    public UILabel factory;
    public UILabel lab;
    public UILabel studio;

    private Company company;

    void OnEnable() {
        company = GameManager.Instance.playerCompany;
    }

    void Update() {
        // Rotate the product, fancy.
        foreach (GameObject obj in objects) {
            UIAnimator.Rotate(obj);
        }

        Infrastructure usedInf = company.usedInfrastructure;
        Infrastructure infra = company.infrastructure;
        datacenter.text = string.Format("{0}/{1}", usedInf[Infrastructure.Type.Datacenter], infra[Infrastructure.Type.Datacenter]);
        factory.text = string.Format("{0}/{1}", usedInf[Infrastructure.Type.Factory], infra[Infrastructure.Type.Factory]);
        lab.text = string.Format("{0}/{1}", usedInf[Infrastructure.Type.Lab], infra[Infrastructure.Type.Lab]);
        studio.text = string.Format("{0}/{1}", usedInf[Infrastructure.Type.Studio], infra[Infrastructure.Type.Studio]);
    }
}
