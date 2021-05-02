using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roach : MonoBehaviour
{
    public SpriteRenderer Renderer;

    private void OnMouseUpAsButton()
    {
        Debug.Log("Kill");
        OnStep?.Invoke();
    }

    void OnValidate()
    {
        Renderer = GetComponentInChildren<SpriteRenderer>();
    }

    public Action OnStep;
}
