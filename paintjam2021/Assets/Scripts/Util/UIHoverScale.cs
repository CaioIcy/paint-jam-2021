using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float ScaleBy = 1.2f;
    Vector3 _initScale;

    void Start()
    {
        _initScale = transform.localScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = _initScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = _initScale * ScaleBy;
    }
}
