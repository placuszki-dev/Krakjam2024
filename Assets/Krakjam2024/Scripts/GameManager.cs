using Placuszki.Krakjam2024;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    #endregion

    public event Action OnStartGame;
    public event Action<string> OnEndGame;

    public GameObject _catprefab;
    public Transform[] _catSpawners;
    public int _catCount = 5;
    public int _pointsToWin = 5;

    [Space]
    public AudioSource _gameMusic;
    public AudioSource _menuMusic;

    private List<Cat> _activeCats = new List<Cat>();
    private Dictionary<string, int> _players = new Dictionary<string, int>();

    private void Start()
    {
        _menuMusic?.Play();
    }

    public void CatHit(string playerID)
    {
        _players.TryAdd(playerID, 0);

        int playerPoints = _players[playerID];
        _players[playerID] = playerPoints + 1;

        if (!CheckIfSomeoneWin())
        {
            CreateCat();
        }

    }

    [ContextMenu("Start")]
    public void StartGame()
    {
        _gameMusic?.Play();
        _menuMusic?.Stop();

        for (int i = 0; i < _catCount; i++)
        {
            CreateCat();
        }
    }

    private void CreateCat()
    {
        var obj = Instantiate(_catprefab);
        obj.transform.position = _catSpawners[UnityEngine.Random.Range(0, _catSpawners.Length)].position;
        obj.GetComponent<Cat>().Init();
    }

    private void EndGame()
    {
        for (int i = 0; i < _activeCats.Count; i++)
        {
            _activeCats[i].DestroyCat();
        }

        _activeCats.Clear();
        _players.Clear();

        _gameMusic?.Stop();
        _menuMusic?.Play();
    }

    private bool CheckIfSomeoneWin()
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players.ElementAt(i).Value >= _pointsToWin)
            {
                OnEndGame.Invoke(_players.ElementAt(i).Key);
                EndGame();
                return true;
            }
        }

        return false;
    }
}