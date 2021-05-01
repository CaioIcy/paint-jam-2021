using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public SpriteRenderer Renderer;

    const float s = 0.4f;

    private void OnMouseEnter()
    {
        Renderer.transform.localScale = Vector3.one * (2/s);
        var pos = Renderer.transform.position;
        pos.z = -9f;
        Renderer.transform.position = pos;
    }

    private void OnMouseExit()
    {
        Renderer.transform.localScale = Vector3.one * 1;
        var pos = Renderer.transform.position;
        pos.z = 0f;
        Renderer.transform.position = pos;
    }

    private void OnValidate()
    {
        Renderer = GetComponentInChildren<SpriteRenderer>();
    }
}
