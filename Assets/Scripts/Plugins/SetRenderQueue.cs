using UnityEngine;

public class SetRenderQueue : MonoBehaviour
{
    public int renderQueue = 3000;

    Material mMat;

    void Start() {
        Setup();
    }

    void OnDestroy() { if (mMat != null) Destroy(mMat); }

    public void Reset() {
        Setup();
    }

    private void Setup() {
        Renderer ren = renderer;

        if (ren == null)
        {
            ParticleSystem sys = GetComponent<ParticleSystem>();
            if (sys != null) ren = sys.renderer;
        }

        if (ren != null)
        {
            mMat = new Material(ren.sharedMaterial);
            mMat.renderQueue = renderQueue;
            ren.material = mMat;
        }
    }
}
