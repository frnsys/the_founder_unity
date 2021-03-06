using UnityEngine;
using System.Collections;

public class UIOnboardingCofounder : MonoBehaviour {
    private Worker cofounder_;
    public Worker cofounder {
        get { return cofounder_; }
        set {
            cofounder_ = value;
            label.text = cofounder_.name;
            description.text = cofounder_.description;
            mesh.material = cofounder_.material;
        }
    }

    public UILabel label;
    public UILabel description;
    public UITexture background;
    public MeshRenderer mesh;
}


