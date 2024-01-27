using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class UI : MonoBehaviour
{
    public Animation _uiAnim;
    public TMP_Text MainText;
    public TMP_Text WinText;
    public CanvasGroup CanvasGroup;
    public GameObject PanelWin;
    public GameObject PanelMain;


    void Start()
    {
        NewGame();
        GameManager.Instance.OnEndGame += EndGame;
        GameManager.Instance.OnStartGame += StartGame;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnEndGame -= EndGame;
        GameManager.Instance.OnStartGame -= StartGame;
    }

    private void EndGame(string obj)
    {
        WinText.text = "Player " + obj + "Wins!";

        CanvasGroup.DOFade(1, 2);
        PanelWin.SetActive(true);
        PanelMain.SetActive(false);
    }

    private void StartGame()
    {
        _uiAnim.Play("ser");
        CanvasGroup.DOFade(0, 3);
    }

    void NewGame()
    {
        PanelWin.SetActive(false);
        PanelMain.SetActive(true);
        _uiAnim.Play("serReverse");
        MainText.DOFade(0, 3).From();
    }

}
