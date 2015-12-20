using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MainGameUI : MonoBehaviour {
    public GameObject resultPrefab;
    public UIProgressBar turnsBar;
    public UILabel goodwillLabel;
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
        goodwillLabel.text = string.Format("{0} goodwill", company.goodwill);
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
        TweenAlpha.Begin(go, MainGame.animationDuration, 0f);
    }
}
