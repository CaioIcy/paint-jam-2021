using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class EndBranch
{
    public Sprite BG;
    public string Dialogue;
    public bool RemovePpl;
}

public class UIEnding : MonoBehaviour
{
    public Image BG;

    public GameObject AlienBalloon;
    public TMP_Text LabelAlienText;

    public GameObject GameOverScreen;
    public TMP_Text GameOverText;

    public GameObject Ppl;

    Coroutine _txtProc = null;
    Coroutine _diagProc = null;
    public float charTime = 0.05f;

    [Header("Branches")]// 
    public List<EndBranch> Init;

    public List<EndBranch> Peaceful;
    public List<EndBranch> MidPeace;
    public List<EndBranch> Genocide;

    public List<EndBranch> Poor;
    public List<EndBranch> Rich;

    bool doneInit = false;
    int next = 0;

    public bool ForceEnd = false;

    private void Start()
    {
        if(ForceEnd)
        {
            Show();
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);

        Debug.Log($"DoBranch=Init");
        DoBranch(Init);

        roachOffer = Mathf.CeilToInt(0.33f * FindObjectOfType<AlienController>(true)._fullPotential);
        roachOffer = Math.Max(1, roachOffer);
    }

    void DoBranch(List<EndBranch> branches)
    {
        StartCoroutine(DoBranchProc(branches));
    }

    IEnumerator DoBranchProc(List<EndBranch> branches)
    {
        foreach(var branch in branches)
        {
            if(branch.BG != null) BG.sprite = branch.BG;

            var msg = branch.Dialogue;
            var roacher = FindObjectOfType<Roacher>(true);
            msg = msg.Replace("<roaches>", roacher.KillCount.ToString());
            msg = msg.Replace("<money>", roachOffer.ToString());

            if(branch.RemovePpl)
            {
                Ppl.SetActive(false);
            }

            yield return ShowDiagProc(new List<string>
            {
                msg,
            }, null);
        }

        Check();
    }

    int roachOffer = 0;

    void Check()
    {
        if(!doneInit)
        {
            var roacher = FindObjectOfType<Roacher>(true);
            var ratio = (roacher.SpawnCount > 0 )
                ? roacher.KillCount / (float) roacher.SpawnCount
                : 0.0f;

            if (roacher.KillCount == 0)
            {
                next = 1;
                Debug.Log($"DoBranch=Peaceful");
                DoBranch(Peaceful);
            }
            else if (ratio < 0.5f)
            {
                next = 2;
                Debug.Log($"DoBranch=MidPeace");

                DoBranch(MidPeace);
            }
            else
            {
                next = 3;
                Debug.Log($"DoBranch=Genocide");

                DoBranch(Genocide);
            }
            doneInit = true;
            return;
        }

        if(next == 3)
        {
            GameOverText.text = "Better luck next time!";
            GameOverScreen.SetActive(true);
            return;
        }

        if (next == 1 || next == 2)
        {
            if (FindObjectOfType<Inventory>(true).Money >= roachOffer)
            {
                next = 4;
                Debug.Log($"DoBranch=Rich");

                DoBranch(Rich);
            }
            else
            {
                next = 5;
                Debug.Log($"DoBranch=Poor");

                DoBranch(Poor);
            }
            return;
        }

        GameOverText.text = (next == 4)
            ? "Good job!"
            : "Better luck next time!";
        GameOverScreen.SetActive(true);
    }

    public void OnQuit()
    {
        AudioManager.ins.Play(AudioManager.ins.Selects);
        Application.Quit();
    }

    public void ShowDialogue(List<string> texts, Action done)
    {
        if (_diagProc != null) StopCoroutine(_diagProc);
        _diagProc = StartCoroutine(ShowDiagProc(texts, done));
    }

    IEnumerator ShowDiagProc(List<string> texts, Action done)
    {
        foreach(var text in texts)
        {
            ShowAlienText(text);
            yield return new WaitForSeconds((text.Length * charTime) + 2.5f);
        }
        done?.Invoke();
    }


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
        var wait = new WaitForSeconds(charTime);
        LabelAlienText.text = string.Empty;
        while (i <= text.Length)
        {
            LabelAlienText.text = text.Substring(0, i++);
            yield return wait;
        }
    }
}
