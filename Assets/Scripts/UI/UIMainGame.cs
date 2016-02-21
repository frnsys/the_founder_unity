using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIMainGame : MonoBehaviour {
    public GameObject resultPrefab;
    public UIProgressBar turnsBar;
    public UILabel statsLabel;
    public UILabel cashLabel;
    public UILabel yearLabel;
    public UILabel profitLabel;
    public UIProgressBar profitProgress;
    public UIProgressBar profitNegativeProgress;

    public Color posCashColor;
    public Color posCashShadow;
    public Color negCashColor;
    public Color negCashShadow;

    public GameObject nextAnchor;
    public UIWidget productInfo;
    public UILabel productNameLabel;
    public UILabel productDescLabel;
    public GameObject[] productObjects;

    private Company company;
    private Camera camera;
    private int totalTurns;
    private int turnsLeft;

    public void Setup(Company c, Camera cam, int turns) {
        company = c;
        camera = cam;
        yearLabel.text = GameManager.Instance.year.ToString();
        totalTurns = turns;
        turnsLeft = turns;
    }

    public void TakeTurn() {
        turnsLeft--;
    }

    // Show product info, fade out after a few secs
    public void ShowProductInfo(Product product) {
        ShowInfo(product.name, product.description, product.meshes);
    }

    public void ShowInfo(string name, string desc, Mesh[] meshes=null) {
        productInfo.alpha = 1f;
        productInfo.gameObject.SetActive(true);

        productNameLabel.text = name;
        productDescLabel.text = desc;

        if (meshes != null) {
            for (int i=0; i<meshes.Length; i++) {
                productObjects[i].GetComponent<MeshFilter>().mesh = meshes[i];
                productObjects[i].SetActive(true);
            }
        } else {
            foreach (GameObject go in productObjects) {
                go.SetActive(false);
            }
        }
    }

    void Update() {
        turnsBar.value = (float)turnsLeft/totalTurns;

        string emo = "OUTRAGE";
        if (company.opinion >= 0) {
            emo = "GOODWILL";
        }
        statsLabel.text = string.Format(":MARKETING: {0:0}     :ENGINEERING: {1:0}     :DESIGN: {2:0}\n:HYPE: {3}     :{4}: {5:0}", company.charisma, company.cleverness, company.creativity, company.hype, emo, company.opinion);

        float cash = company.cash.value;
        if (cash <= 0) {
            cashLabel.color = negCashColor;
            cashLabel.effectColor = negCashShadow;
        } else {
            cashLabel.color = posCashColor;
            cashLabel.effectColor = posCashShadow;
        }
        cashLabel.text = string.Format("{0:C0}", cash);

        float profit = company.annualRevenue - company.annualCosts - company.toPay;
        float profitPercent = profit/GameManager.Instance.profitTarget;

        if (profitPercent < 0) {
            profitProgress.value = 0;
            profitNegativeProgress.value = Mathf.Min(1f, -profitPercent);
        } else {
            profitProgress.value = profitPercent;
            profitNegativeProgress.value = 0;
        }
        profitLabel.text = string.Format("{0:C0}/{1:C0}", profit, GameManager.Instance.profitTarget);
    }

    // TO DO should this be adapted for in-progress pieces?
    //public void ShowNextPiece(GameObject nextPiece) {
        //// Get top-left coordinate of the next piece anchor
        //Vector3 pos = UICamera.mainCamera.WorldToViewportPoint(nextAnchor.GetComponent<UIWidget>().worldCorners[1]);
        //pos = camera.ViewportToWorldPoint(pos);
        //nextPiece.transform.position = pos + new Vector3(MainGame.gridItemSize/2,-MainGame.gridItemSize/2,1);
    //}

    // Show a float-up text bit at the specified position
    public void ShowResultAt(Vector2 pos, string text) {
        // Get correct positioning for NGUI
        Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
        screenPos.x -= Screen.width/2f;
        screenPos.y -= Screen.height/2f;

        GameObject go = NGUITools.AddChild(gameObject, resultPrefab);
        go.GetComponent<UILabel>().text = text;
        go.transform.localPosition = screenPos;
        go.transform.localPositionTo(MainGame.animationDuration, screenPos + new Vector3(0, 32f, 0));
        TweenAlpha.Begin(go, 3f, 0f);
    }
}
