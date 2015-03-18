using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TheMarket : MonoBehaviour {

    public Transform consumerGroup;
    public GameObject[] consumerPrefabs;
    public Transform[] pedestals;
    public Transform[] pedestalGroups;
    public MeshFilter[] products;
    public TextMesh[] companyLabels;
    public TextMesh[] marketLabels;

    void OnEnable() {
        if (GameManager.hasInstance)
            GameManager.Instance.Pause();
    }

    void OnDisable() {
        if (GameManager.hasInstance)
            GameManager.Instance.Resume();
    }

    // Setup the market for a product.
    public void Setup(Product p) {
        Reset();
        gameObject.SetActive(true);

        // Try to find two AI companies which are suitable competitors for this product.
        List<AICompany> competitors = new List<AICompany>();
        List<AICompany> active = AICompany.all.Where(c => !c.disabled).ToList();
        List<AICompany> candidates = active.Where(c => c.specialtyVerticals.Intersect(p.requiredVerticals).Count() > 0).ToList();

        switch (candidates.Count) {
            case 0:
                competitors = active.OrderBy(c => Random.value).Take(2).ToList();
                break;
            case 1:
                competitors = candidates;
                competitors.Add(active.OrderBy(c => Random.value).First(c => c != candidates[0]));
                break;
            case 2:
                competitors = candidates;
                break;
            default:
                competitors = candidates.OrderBy(c => Random.value).Take(2).ToList();
                break;
        }

        float aiMarketShare = Random.value * (1 - p.marketShare);
        float[] marketShares = new float[] {
            p.marketShare,
            aiMarketShare,
            1 - p.marketShare - aiMarketShare
        };


        // Setup the products on the pedestals.
        // Setup the company labels.
        for (int i=0; i < pedestals.Length; i++) {
            if (i == 0) {
                companyLabels[i].text = GameManager.Instance.playerCompany.name;
            } else {
                companyLabels[i].text = competitors[i-1].name;
                Debug.Log(competitors[i-1].name);
            }

            products[i].mesh = p.meshes[0];
            marketLabels[i].text = string.Format("{0:P1}", marketShares[i]);
            marketLabels[i].transform.localScale = Vector3.zero;
            Debug.Log(marketShares[i]);

            // Reset then raise the pedestals.
            Vector3 pos = pedestalGroups[i].localPosition;
            pos.y = -0.8f;
            pedestalGroups[i].localPosition = pos;
            float y = -0.8f * (1 - marketShares[i]);
            StartCoroutine(UIAnimator.Raise(pedestalGroups[i], y, 2f));

            // Spawn consumers.
            for (int j=0; j <= (int)(100 * marketShares[i]); j++) {
                Vector3 target = pedestals[i].localPosition;
                target.y = 0;
                SpawnConsumer(target);
            }
        }


        if (Started != null)
            Started();

        StartCoroutine(Countdown());
    }

    void Reset() {
        // Clean up consumers.
        for (int i=0; i < consumerGroup.transform.childCount; i++) {
            Destroy(consumerGroup.transform.GetChild(0).gameObject);
        }
    }

    private void SpawnConsumer(Vector3 target) {
        GameObject consumer = Instantiate(consumerPrefabs[Random.Range(0, consumerPrefabs.Length)]) as GameObject;
        consumer.transform.parent = consumerGroup;
        consumer.GetComponent<Consumer>().target = target;
    }

    void Update() {
        for (int i=0; i < products.Length; i++) {
            UIAnimator.Rotate(products[i].gameObject);
        }
    }

    static public event System.Action Started;
    static public event System.Action Done;
    private IEnumerator Countdown() {
        yield return new WaitForSeconds(8f);

        // Show the market labels.
        for (int i=0; i < marketLabels.Length; i++) {
            StartCoroutine(UIAnimator.Scale(marketLabels[i].transform, 0, 0.12f, 4f));
        }

        yield return new WaitForSeconds(5f);

        Reset();
        gameObject.SetActive(false);

        // Emit the done event.
        if (Done != null)
            Done();
    }
}
