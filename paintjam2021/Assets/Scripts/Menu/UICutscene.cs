using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICutscene : MonoBehaviour
{
    public List<Sprite> Images;
    public List<GameObject> LabelContainers;
    public Image CutsceneImage;
    public float Speed;

    public bool Jump = false;

    public GameObject Part2;
    public List<string> Part2Dialogue;
    public GameObject AlienBalloon;
    public TMP_Text LabelAlienText;

    public Action OnOver;

    int idx = 0;
    bool _over = false;

    void Start()
    {
        Next();
    }

    void Next()
    {
        if(idx >= Images.Count)
        {
            Over();
            return;
        }
        var pos = CutsceneImage.rectTransform.position;
        pos.x = Screen.width;
        CutsceneImage.rectTransform.position = pos;

        var i = idx++;
        CutsceneImage.sprite = Images[i];
        for(var j = 0; j < LabelContainers.Count; ++j)
        {
            LabelContainers[j].SetActive(i==j);
        }
        

        Debug.Log(CutsceneImage.rectTransform.sizeDelta.x);
        Debug.Log(Screen.width);
    }

    private void Update()
    {
        if (_over) return;

        var pos = CutsceneImage.rectTransform.position;
        pos.x -= Speed * Time.deltaTime;
        CutsceneImage.rectTransform.position = pos;

        if(CutsceneImage.rectTransform.localPosition.x < -(CutsceneImage.rectTransform.sizeDelta.x + (Screen.width / 2)))
        {
            Next();
        }
    }

    void Over()
    {
        _over = true;
        CutsceneImage.enabled = false;
        LabelContainers[idx - 1].SetActive(false);

        if(Jump)
        {
            GoToGame();
        }
        else
        {
            ShowPart2();
        }
    }

    void ShowPart2()
    {
        Part2.SetActive(true);
        StartCoroutine(ShowPart2Proc());
    }

    IEnumerator ShowPart2Proc()
    {
        foreach(var p2t in Part2Dialogue)
        {
            yield return ShowText(p2t);
            yield return new WaitForSeconds(2f);
        }
        GoToGame();
    }

    void GoToGame()
    {
        OnOver.Invoke();
        gameObject.SetActive(false);
    }


    Coroutine _txtProc = null;
    public void ShowAlienText(string text)
    {
        AlienBalloon.gameObject.SetActive(true);
        //LabelAlienText.text = text;

        if (_txtProc != null) StopCoroutine(_txtProc);
        _txtProc = StartCoroutine(ShowText(text));
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
        while (i <= text.Length)
        {
            LabelAlienText.text = text.Substring(0, i++);
            yield return wait;
        }
    }
}
