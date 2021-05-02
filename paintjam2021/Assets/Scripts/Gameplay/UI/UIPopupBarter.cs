using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPopupBarter : MonoBehaviour
{
    public Inventory Inventory;

    int n1;
    int n2;
    int n3;
    int n4;
    public TMP_Text L1;
    public TMP_Text L2;
    public TMP_Text L3;
    public TMP_Text L4;

    public Animator MoneyAnimator;

    private void Awake()
    {
        Inventory = FindObjectOfType<Inventory>();
    }

    private void Start()
    {
        //n1 = 0;
        //n2 = 0;
        //n3 = 0;
        //n4 = 0;
        //UpdateLabels();
    }

    public void ArrowUp(int n)
    {
        AudioManager.ins.Play(AudioManager.ins.Selects);
        switch (n)
        {
            case 1: n1 = (n1 + 1) % 10; break;
            case 2: n2 = (n2 + 1) % 10; break;
            case 3: n3 = (n3 + 1) % 10; break;
            case 4: n4 = (n4 + 1) % 10; break;
            default:
                throw new System.Exception("bad case");
        }
        UpdateLabels();
    }

    void UpdateLabels()
    {
        L1.text = n1.ToString();
        L2.text = n2.ToString();
        L3.text = n3.ToString();
        L4.text = n4.ToString();
    }

    public void ArrowDown(int n)
    {
        AudioManager.ins.Play(AudioManager.ins.Selects);
        switch (n)
        {
            case 1: n1 = (n1 - 1 < 0) ? 9 : n1 - 1; ; break;
            case 2: n2 = (n2 - 1 < 0) ? 9 : n2 - 1; ; break;
            case 3: n3 = (n3 - 1 < 0) ? 9 : n3 - 1; ; break;
            case 4: n4 = (n4 - 1 < 0) ? 9 : n4 - 1; ; break;
            default:
                throw new System.Exception("bad case");
        }
        UpdateLabels();
    }

    public void Show(int value)
    {
        Debug.Log($"show with value {value}");
        value = Mathf.Min(value, 9999);
        var digits = new int[4];
        var d = 0;
        while(d<4)
        {
            var digit = value % 10;
            //Debug.Log($"digit {d} = {digit}");
            digits[d++] = digit;
            value /= 10;
        }

        n1 = digits[3];
        n2 = digits[2];
        n3 = digits[1];
        n4 = digits[0];
        UpdateLabels();

        gameObject.SetActive(true);
    }


    public void OnClickBarter() {
        AudioManager.ins.Play(AudioManager.ins.Selects);
        var amount = n1 * 1000 + n2 * 100 + n3 * 10 + n4 * 1;
        Debug.Log($"click barter with {amount}");

        //if(amount > Inventory.Money)
        //{
        //    MoneyAnimator.SetTrigger("Shake");
        //    return;
        //}

        OnClickClose();
        //FindObjectOfType<UIPopupItem>(true).OnClickClose();
        FindObjectOfType<AlienController>().Offer(amount);
    }

    public void OnClickClose()
    {
        AudioManager.ins.Play(AudioManager.ins.Selects);
        n1 = 0;
        n2 = 0;
        n3 = 0;
        n4 = 0;
        UpdateLabels();

        gameObject.SetActive(false);
    }

}
