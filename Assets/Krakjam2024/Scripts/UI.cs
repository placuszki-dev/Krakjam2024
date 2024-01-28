using UnityEngine;
using DG.Tweening;
using TMPro;

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
        gameObject.SetActive(true);
        PanelWin.SetActive(false);
        PanelMain.SetActive(true);
        _uiAnim.Play("serReverse");
        MainText.DOFade(0, 3).From();
    }

}
