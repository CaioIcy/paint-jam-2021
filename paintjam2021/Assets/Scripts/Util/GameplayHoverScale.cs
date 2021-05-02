using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameplayHoverScale : MonoBehaviour
{
    public float ScaleBy = 1.2f;
    Vector3 _initScale;

    void Start()
    {
        _initScale = transform.localScale;
    }

    void OnMouseExit()
    {
        transform.localScale = _initScale;
    }

    void OnMouseEnter()
    {
        transform.localScale = _initScale * ScaleBy;
    }
}
