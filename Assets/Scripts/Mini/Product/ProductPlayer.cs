using UnityEngine;

public class ProductPlayer : MonoBehaviour {
    public GameObject prefab;
    public Transform gameBoard;

    private GameObject well;

    void OnPress(bool isDown) {
        if (well != null)
            Destroy(well);

        if (isDown) {
            well = Instantiate(prefab) as GameObject;
            well.transform.position = UICamera.lastHit.point;
            well.transform.parent = gameBoard;
        }
    }
}
