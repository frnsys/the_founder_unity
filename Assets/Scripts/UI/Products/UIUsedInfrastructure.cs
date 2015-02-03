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
            obj.transform.Rotate(0,0,-50*Time.deltaTime);
        }

        Infrastructure usedInf = company.usedInfrastructure;
        Infrastructure infra = company.infrastructure;
        datacenter.text = usedInf[Infrastructure.Type.Datacenter].ToString() + "/" + infra[Infrastructure.Type.Datacenter].ToString();
        factory.text = usedInf[Infrastructure.Type.Factory].ToString() + "/" + infra[Infrastructure.Type.Factory].ToString();
        lab.text = usedInf[Infrastructure.Type.Lab].ToString() + "/" + infra[Infrastructure.Type.Lab].ToString();
        studio.text = usedInf[Infrastructure.Type.Studio].ToString() + "/" + infra[Infrastructure.Type.Studio].ToString();
    }
}
