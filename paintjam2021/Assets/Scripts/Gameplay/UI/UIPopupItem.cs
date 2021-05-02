using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupItem : MonoBehaviour
{
    public static bool CanInteract = true;

    [Header("UI")] //
    public TMPro.TMP_Text ItemTitle;

    public TMPro.TMP_Text ItemUse;
    public TMPro.TMP_Text ItemDiscovery;
    public TMPro.TMP_Text ItemObs;
    public Image ItemSpr;

    UIGameplay root;
    private void Awake()
    {
        root = FindObjectOfType<UIGameplay>();
    }

    Item _item;
    public void Setup(Item item)
    {
        _item = item;
        ItemTitle.text = $"\"{item.Name}\"";
        ItemSpr.sprite = item.Sprite;

        ItemUse.text = item.Use;
        ItemDiscovery.text = item.Discovery;
        ItemObs.text = item.Observations;
        gameObject.SetActive(true);
    }

    public void OnClickBarter()
    {
        AudioManager.ins.Play(AudioManager.ins.Selects);
        FindObjectOfType<AlienController>().Barter(_item.ID);
        Hide();
        UIPopupItem.CanInteract = false;
    }

    public void OnClickClose()
    {
        AudioManager.ins.Play(AudioManager.ins.Selects);
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        UIPopupItem.CanInteract = true;
    }
}
