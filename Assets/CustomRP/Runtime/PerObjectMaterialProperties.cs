using System;
using UnityEngine;

[DisallowMultipleComponent]
public class PerObjectMaterialProperties : MonoBehaviour {
	static MaterialPropertyBlock block;
    static int baseColorId = Shader.PropertyToID("_BaseColor");
	
    [SerializeField]
    Color baseColor = Color.white;

    private void Awake()
    {
	    OnValidate();
    }

    void OnValidate () {
	    if (block == null) {
		    block = new MaterialPropertyBlock();
	    }
	    block.SetColor(baseColorId, baseColor);
	    GetComponent<Renderer>().SetPropertyBlock(block);
    }
    
}