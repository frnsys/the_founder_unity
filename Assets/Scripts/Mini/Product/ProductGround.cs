using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ProductGround : MonoBehaviour {

    public Camera camera;
    public ProductPlayer player;
    public GameObject prefab;
    public GameObject group;
    public Mesh[] meshes;
    public List<GameObject> rowGroups;

    private int lastRow;
    private GameObject lastRowRep;

    private float side = 2;
    private float z;
    private Vector3 refPoint;
    private Vector3 refPoint_;

    private void GenerateGround() {
        rowGroups = new List<GameObject>();
        z = camera.WorldToViewportPoint(player.transform.position).z + 30;

        refPoint  = camera.ViewportToWorldPoint(new Vector3(0, 0, z));
        refPoint_ = camera.ViewportToWorldPoint(new Vector3(1, 1, z));

        // Some extra padding rowGroups.
        int cols = Mathf.CeilToInt((refPoint_.x - refPoint.x)/side);
        int rows = Mathf.CeilToInt((refPoint_.y - refPoint.y)/side) + 4;

        for (int j=0; j<rows; j++) {
            GameObject groundRow = new GameObject("Ground Row");
            groundRow.transform.parent = group.transform;
            rowGroups.Add(groundRow);
            for (int i=0; i<cols; i++) {
                float height = 2 + Random.value * 20;
                GameObject groundObj = Instantiate(prefab) as GameObject;
                groundObj.GetComponent<MeshFilter>().mesh = meshes[Random.Range(0, meshes.Length)];
                groundObj.transform.parent = groundRow.transform;
                groundObj.transform.localScale = new Vector3(side, side, height);

                // j is -1 for the padding rowGroups.
                groundObj.transform.position = new Vector3(refPoint.x + (side * i), refPoint_.y - (side * j - 2), z - height/2);

                Vector3 dir = new Vector3(0, -1, 0);
                float speed = 200;
                float speedLimit = 200;
                groundObj.rigidbody.AddForce(Vector3.ClampMagnitude(dir * speed, speedLimit));
            }
        }

        lastRow = rowGroups.Count - 1;
        lastRowRep = rowGroups[lastRow].transform.GetChild(0).gameObject;
    }

    void Reset() {
        // TO DO
    }

    void Start() {
        GenerateGround();
    }

    void Update() {
        if (!lastRowRep.renderer.isVisible) {
            int firstGroundRow = lastRow == rowGroups.Count - 1 ? 0 : lastRow + 1;
            float y = rowGroups[firstGroundRow].transform.GetChild(0).position.y + side;

            foreach (Transform t in rowGroups[lastRow].transform) {
                float height = 2 + Random.value * 20;
                t.localScale = new Vector3(side, side, height);
                t.GetComponent<MeshFilter>().mesh = meshes[Random.Range(0, meshes.Length)];
                Vector3 pos = t.position;
                pos.y = y;
                pos.z = z - height/2;
                t.position = pos;
            }

            lastRow = lastRow == 0 ? lastRow = rowGroups.Count - 1 : lastRow - 1;
            lastRowRep = rowGroups[lastRow].transform.GetChild(0).gameObject;
        }
    }
}
