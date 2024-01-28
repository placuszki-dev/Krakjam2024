using DG.Tweening;
using TMPro;
using UnityEngine;

public class CheeseScorePanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private RectTransform _view;

    [SerializeField] private float _shakeDuration = 0.5f;
    [SerializeField] private float _shakeStrength = 1;
    [SerializeField] private int _shakeVibrato = 10;

    private int _score = -1;

    public void SetScore(int score, int maxScore)
    {
        if (score == _score)
        {
            Debug.Log("CHUJ");
            return;
        }

        _score = score;
        _scoreText.SetText($"{score}/{maxScore}");
        if (score < maxScore)
            Shake();
    }

    private void Shake()
    {
        _view.DOShakeRotation(_shakeDuration, new Vector3(0f, 0f, _shakeStrength), _shakeVibrato)
            .OnComplete(() => _view.DORotate(Vector3.zero, 0.1f));
    }
}