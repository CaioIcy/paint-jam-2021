using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public SpriteRenderer Renderer;

    public UIGameplay _ui;

    const float s = 0.4f;

    string _itemID;

    public void Setup(Item item)
    {
        _itemID = item.ID;
        Renderer.sprite = item.Sprite;
    }

    private void OnMouseEnter()
    {
        if (!UIPopupItem.CanInteract) { return; }

        Renderer.transform.localScale = Vector3.one * (2/s);
        var pos = Renderer.transform.position;
        pos.z = -9f;
        Renderer.transform.position = pos;
    }

    private void OnMouseExit()
    {
        if(!UIPopupItem.CanInteract) { return; }

        Renderer.transform.localScale = Vector3.one * 1;
        var pos = Renderer.transform.position;
        pos.z = 0f;
        Renderer.transform.position = pos;
    }

    private void OnMouseUpAsButton()
    {
        if (!UIPopupItem.CanInteract) { return; }

        AudioManager.ins.Play(AudioManager.ins.Selects);

        _ui.Barter(_itemID);
        UIPopupItem.CanInteract = false;

        Renderer.transform.localScale = Vector3.one * 1;
        var pos = Renderer.transform.position;
        pos.z = 0f;
        Renderer.transform.position = pos;
    }

    private void OnValidate()
    {
        Renderer = GetComponentInChildren<SpriteRenderer>();
        _ui = FindObjectOfType<UIGameplay>(true);
    }
}
