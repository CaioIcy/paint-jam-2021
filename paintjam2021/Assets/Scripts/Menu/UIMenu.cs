using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMenu : MonoBehaviour
{
    public GameObject _gameplay;
    public GameObject _gameplayUI;

    public UICutscene Cutscene;

    private void Awake()
    {
        Cutscene.OnOver = StartGame;
    }

    public void OnClickStart()
    {
        //Debug.Log("OnClickStart");
        AudioManager.ins.Play(AudioManager.ins.Selects);
        Cutscene.gameObject.SetActive(true);
    }

    public void OnClickQuit()
    {
        //Debug.Log("OnClickQuit");
        AudioManager.ins.Play(AudioManager.ins.Selects);
        Application.Quit();
    }

    void StartGame()
    {
        _gameplay.SetActive(true);
        _gameplayUI.SetActive(true);
        gameObject.SetActive(false);
    }
}
