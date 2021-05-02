using System.Collections;
using TMPro;
using UnityEngine;

public class UIGameplay : InvLisMonoBehaviour
{
    public ItemData ItemData;

    [Header("UI")] //
    public TMP_Text LabelMoney;

    public GameObject AlienBalloon;
    public TMP_Text LabelAlienText;

    [HideInInspector] public UIPopupItem ItemPopup;
    [HideInInspector] public UIPopupBarter BarterPopup;
    [HideInInspector] public UIEnding Ending;

    protected override void Awake()
    {
        base.Awake();
        ItemPopup = FindObjectOfType<UIPopupItem>(true);
        ItemPopup.gameObject.SetActive(false);
        BarterPopup = FindObjectOfType<UIPopupBarter>(true);
        BarterPopup.gameObject.SetActive(false);

        Ending = FindObjectOfType<UIEnding>(true);
        Ending.gameObject.SetActive(false);

        OnInventoryChanged(Inventory);
    }

    public override void OnInventoryChanged(Inventory inv)
    {
        LabelMoney.text = $"{inv.Money}";
    }

    public void Barter(string itemID)
    {
        ItemPopup.Setup(ItemData.GetItem(itemID));
    }

    Coroutine _txtProc = null;
    public void ShowAlienText(string text)
    {
        AlienBalloon.gameObject.SetActive(true);
        //LabelAlienText.text = text;

        if (_txtProc != null) StopCoroutine(_txtProc); 
        _txtProc = StartCoroutine(ShowText(text));
    }

    public void SetupEnding()
    {
        ItemPopup.Hide();
        BarterPopup.gameObject.SetActive(false);
        Ending.Show();
        gameObject.SetActive(false);
        UIPopupItem.CanInteract = false;
    }

    public void HideAlienText()
    {
        AlienBalloon.gameObject.SetActive(false);
        _txtProc = null;
    }

    IEnumerator ShowText(string text)
    {
        var i = 0;
        var wait = new WaitForSeconds(0.05f);
        LabelAlienText.text = string.Empty;
        while (i <= text.Length) {
            LabelAlienText.text = text.Substring(0, i++);
            yield return wait;
        }
    }
}
