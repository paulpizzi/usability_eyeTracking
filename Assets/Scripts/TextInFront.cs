using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextInFront : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var parent = transform.parent;

        var parentRenderer = parent.GetComponent<Renderer>();
        var renderer = GetComponent<Renderer>();
        renderer.sortingLayerID = parentRenderer.sortingLayerID;
        renderer.sortingOrder = parentRenderer.sortingOrder+1;
    }

  
}
