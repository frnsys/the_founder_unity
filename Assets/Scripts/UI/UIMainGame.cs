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

    public void Setup(Company c, Camera cam, int turns) {
        company = c;
        camera = cam;
        yearLabel.text = GameManager.Instance.year.ToString();
        UpdateUI(turns, turns);
    }

    // Show product info, fade out after a few secs
    public void ShowProductInfo(Product product) {
        StartCoroutine(_ShowProductInfo(product));
    }

    private IEnumerator _ShowProductInfo(Product product) {
        productInfo.alpha = 1f;
        productInfo.gameObject.SetActive(true);

        productDescLabel.text = product.description;
        productNameLabel.text = product.name;
        for (int i=0; i<product.meshes.Length; i++) {
            productObjects[i].GetComponent<MeshFilter>().mesh = product.meshes[i];
        }

        yield return new WaitForSeconds(3f);
        TweenAlpha.Begin(productInfo.gameObject, MainGame.animationDuration, 0f);
        yield return new WaitForSeconds(MainGame.animationDuration);
        productInfo.gameObject.SetActive(false);
    }

    public void UpdateUI(int turnsLeft, int totalTurns) {
        turnsBar.value = (float)turnsLeft/totalTurns;
        statsLabel.text = string.Format(":MARKETING: {0:0}     :ENGINEERING: {1:0}     :DESIGN: {2:0}     :GOODWILL: {3:0}", company.charisma, company.cleverness, company.creativity, company.goodwill);

        float cash = company.cash.value;
        if (cash <= 0) {
            cashLabel.color = negCashColor;
            cashLabel.effectColor = negCashShadow;
        } else {
            cashLabel.color = posCashColor;
            cashLabel.effectColor = posCashShadow;
        }
        cashLabel.text = string.Format("{0:C0}", cash);

        profitLabel.text = string.Format("Profit: {0:C0}/{1:C0}", company.annualRevenue - company.annualCosts - company.toPay, GameManager.Instance.profitTarget);
    }

    public void ShowNextPiece(GameObject nextPiece) {
        // Get top-left coordinate of the next piece anchor
        Vector3 pos = UICamera.mainCamera.WorldToViewportPoint(nextAnchor.GetComponent<UIWidget>().worldCorners[1]);
        pos = camera.ViewportToWorldPoint(pos);
        nextPiece.transform.position = pos + new Vector3(MainGame.gridItemSize/2,-MainGame.gridItemSize/2,1);
    }

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
