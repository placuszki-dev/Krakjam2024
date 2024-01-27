using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Placuszki.Krakjam2024.Server;

public enum GamePhase
{
    Menu = 0,
    Play = 1,
    EndGame = 3,
}

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

    public GameObject _ui;
    public GameObject _catprefab;
    public Transform[] _catSpawners;
    public int _catCount = 5;
    public int _pointsToWin = 5;

    [Space]
    public AudioSource _gameMusic;
    public AudioSource _menuMusic;

    private List<Cat> _activeCats = new List<Cat>();
    private Dictionary<string, int> _players = new Dictionary<string, int>();
    private Dictionary<string, string> _colors = new Dictionary<string, string>();

    private GamePhase _gamePhase;
    private void Start()
    {
        PlayMusic();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            switch (_gamePhase)
            {
                case GamePhase.Menu:
                    StartGame();
                    break;
                case GamePhase.Play:
                    StopGame();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    [ContextMenu("StartGame")]
    public void StartGame()
    {
        _gamePhase = GamePhase.Play;
        
        _ui.SetActive(false);
        PlayMusic();
        CreateCats();
    }

    [ContextMenu("StopGame")]
    private void StopGame()
    {
        _gamePhase = GamePhase.Menu;
        
        _ui.SetActive(true);
        PlayMusic();
        DestroyAllCats();
    }
    
    private void EndGame()
    {
        _gamePhase = GamePhase.EndGame;
        
        DestroyAllCats();
        _players.Clear();
        _colors.Clear();

        PlayMusic();
    }
    
    private void PlayMusic()
    {
        switch (_gamePhase)
        {
            case GamePhase.Menu:
                _gameMusic?.Stop();
                _menuMusic?.Play();
                break;
            case GamePhase.Play:
                _gameMusic?.Play();
                _menuMusic?.Stop();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void RegisterPlayer(DataPacket dataPacket)
    {
        _players.TryAdd(dataPacket.PlayerId, 0);
        _colors.TryAdd(dataPacket.PlayerId, dataPacket.PhoneColor);
    }

    public void DeregisterPlayer(string id)
    {
        _players.Remove(id);
        _colors.Remove(id);
    }

    private void CreateCats()
    {
        for (int i = 0; i < _catCount; i++)
        {
            CreateCat();
        }
    }
    
    private void DestroyAllCats()
    {
        for (int i = 0; i < _activeCats.Count; i++)
        {
            var cat = _activeCats[i];
            cat.DestroyCat();
        }

        _activeCats.Clear();
    }

    private void CreateCat()
    {
        _catprefab.SetActive(true); // Stupid hack
        var obj = Instantiate(_catprefab);
        _catprefab.SetActive(false); // Stupid hack
        
        obj.transform.position = _catSpawners[UnityEngine.Random.Range(0, _catSpawners.Length)].position;

        Cat cat = obj.GetComponent<Cat>();
        cat.Init();
        cat.OnCatDestroyed += OnCatDestroyed;
        _activeCats.Add(cat);
    }

    private void OnCatDestroyed(Cat cat)
    {
        _activeCats.Remove(cat);
    }

    public void CatHit(string playerID)
    {
        int playerPoints = _players[playerID];
        _players[playerID] = playerPoints + 1;

        if (!CheckIfSomeoneWin())
        {
            CreateCat();
        }
    }

   

    private bool CheckIfSomeoneWin()
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players.ElementAt(i).Value >= _pointsToWin)
            {
                OnEndGame?.Invoke(_players.ElementAt(i).Key);
                EndGame();
                return true;
            }
        }

        return false;
    }
}