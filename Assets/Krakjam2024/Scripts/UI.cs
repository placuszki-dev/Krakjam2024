using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;

public class UI : MonoBehaviour
{
    public Animation _uiAnim;
    public TMP_Text MainText;
    public TMP_Text WinText;
    public CanvasGroup CanvasGroup;
    public GameObject PanelWin;
    public GameObject PanelMain;
    private Tween _fadeTween;

    void Start()
    {
        NewGame();
        GameManager.Instance.OnMenu += ShowMenu;
        GameManager.Instance.OnEndGame += EndGame;
        GameManager.Instance.OnStartGame += StartGame;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnMenu -= ShowMenu;
        GameManager.Instance.OnEndGame -= EndGame;
        GameManager.Instance.OnStartGame -= StartGame;
    }

    private void ShowMenu()
    {
        NewGame();
    } 
    
    private void EndGame(string obj)
    {
        gameObject.SetActive(true);
        WinText.text = "Player " + obj + "Wins!";

        _fadeTween = CanvasGroup.DOFade(1, 2);
        PanelWin.SetActive(true);
        PanelMain.SetActive(false);
    }

    private void StartGame()
    {
        _uiAnim.Play("ser");
        _fadeTween = CanvasGroup.DOFade(0, 3);
    }

    void NewGame()
    {
        _fadeTween.Kill();
        gameObject.SetActive(true);
        PanelWin.SetActive(false);
        PanelMain.SetActive(true);
        _uiAnim.Play("serReverse");
        CanvasGroup.alpha = 1;
        MainText.DOFade(0, 3).From();
    }

}
