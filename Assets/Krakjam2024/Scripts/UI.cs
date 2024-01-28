using UnityEngine;
using DG.Tweening;
using Placuszki.Krakjam2024;
using TMPro;

public class UI : MonoBehaviour
{
    public Animation _uiAnim;
    public TMP_Text MainText;
    public TMP_Text LinkText;
    public TMP_Text WinText;
    public CanvasGroup CanvasGroup;
    public GameObject PanelWin;
    public GameObject PanelMain;
    private Tween _fadeTween;

    void Start()
    {
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
    
    private void EndGame(CheeseType cheeseType)
    {
        gameObject.SetActive(true);
        WinText.text = "Team " + cheeseType + " wins!";

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
        MainText.DOFade(0, 2).From();
        LinkText.DOFade(0, 2).From();
    }

}
