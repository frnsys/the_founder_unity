using UnityEngine;
using System.Collections;

public class UIMarket : UIWindow {
    public GameObject storePagePrefab;
    public GameObject itemItemPrefab;
    public GameObject itemDetailPrefab;
    public UISimpleGrid storePageGrid;
    public UIScrollManager scrollManager;
    public UIDynamicDragScrollView currentPageScrollDrag;

    void OnEnable() {
        foreach (Store store in GameManager.Instance.unlocked.stores) {
            GameObject storePage = NGUITools.AddChild(storePageGrid.gameObject, storePagePrefab);
            storePage.GetComponent<UIWidget>().SetAnchor(storePageGrid.gameObject);

            storePage.transform.Find("Title").GetComponent<UILabel>().text = store.ToString();
            GameObject itemGrid = storePage.transform.Find("Content Scroll/Items Grid").gameObject;

            foreach (Item item in GameManager.Instance.unlocked.items.FindAll(i => i.store == store)) {
                // This for loop is just for testing a bunch of items in the grid layout.
                for (int i =0; i< 20; i++) {
                    GameObject itemItem = NGUITools.AddChild(itemGrid, itemItemPrefab);
                    itemItem.GetComponent<UIMarketItem>().item = item;
                    UIEventListener.Get(itemItem).onClick += ShowItemDetail;

                    // Unfortunately, we must have a UIDragScrollView on every item
                    // so that you can both click them as a button and drag on them to scroll the page.
                    itemItem.GetComponent<UIDragScrollView>().scrollView = storePage.transform.Find("Content Scroll").GetComponent<UIScrollView>();
                }
            }
            itemGrid.GetComponent<UISimpleGrid>().Reposition();
        }
        storePageGrid.Reposition();

        // Get first page and set it as the vertical scroll view.
        Transform startingPage = UISimpleGrid.GetChildren(storePageGrid.transform)[0].transform;
        currentPageScrollDrag.scrollView = startingPage.Find("Content Scroll").GetComponent<UIScrollView>();

        // If there is only one store, disable page-to-page scrolling.
        if (GameManager.Instance.unlocked.stores.Count == 1) {
            scrollManager.direction = UIScrollManager.Movement.Vertical;
        }
    }

    public void ShowItemDetail(GameObject obj) {
        GameObject itemDetailPopup = NGUITools.AddChild(gameObject, itemDetailPrefab);
        itemDetailPopup.GetComponent<UIMarketItemDetail>().item = obj.GetComponent<UIMarketItem>().item;
    }
}
