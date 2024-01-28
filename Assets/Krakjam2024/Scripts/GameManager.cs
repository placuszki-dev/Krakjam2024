using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Placuszki.Krakjam2024;
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

    public event Action OnMenu;
    public event Action OnStartGame;
    public event Action<UserInfo> OnEndGame;

    public GameObject _ui;
    public GameObject _catprefab;
    public Transform[] _catSpawners;
    public int _catCount = 5;
    public int _pointsToWin = 5;
    public ConnectionManager _connectionManager;
        
    [Space]
    public AudioSource _gameMusic;
    public AudioSource _menuMusic;

    private List<Cat> _activeCats = new List<Cat>();
    private readonly List<Player> _players = new ();

    private GamePhase _gamePhase;
    private void Start()
    {
        _connectionManager.Connected += ShowMenu;
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
                    // do nothing
                    break;
                case GamePhase.EndGame:
                    ShowMenu();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (_gamePhase == GamePhase.Play)
            {
                StopGame();
                ShowMenu();
            }
        }
    }

    private void ShowMenu()
    {
        SetPhase(GamePhase.Menu);
        //_ui.SetActive(true);
        PlayMusic();
        SendMainMenuOpenedEndGameToServer();
        OnMenu?.Invoke();
    }

    private void SetPhase(GamePhase phase)
    {
        Debug.Log($"SetPhase: {phase.ToString()}");
        _gamePhase = phase;
    }

    [ContextMenu("StartGame")]
    public void StartGame()
    {
        SetPhase(GamePhase.Play);
        //_ui.SetActive(false);
        PlayMusic();
        CreateCats();
        OnStartGame?.Invoke();
    }

    [ContextMenu("StopGame")]
    private void StopGame()
    {
        SetPhase(GamePhase.Menu);

        // _ui.SetActive(true);
        PlayMusic();
        DestroyAllCats();
        DestroyAllPlayers();
        SendMainMenuOpenedEndGameToServer();
    }

    private void EndGame(Player winner)
    {
        SetPhase(GamePhase.EndGame);
        OnEndGame?.Invoke(winner.UserInfo);

        _ui.SetActive(true);
        DestroyAllCats();
        DestroyAllPlayers();

        PlayMusic();
        
        UserInfo userInfo = new UserInfo()
        {
            CheeseType = 0,
            PhoneColor = "TODO",
            PlayerId = "TODO",
        };
        
        SendEndGameToServer(userInfo);
    }
   
    private void SendEndGameToServer(UserInfo userInfo)
    {
        _connectionManager.SendEndGameToServer(userInfo);
    }

    [ContextMenu("DebugSendEndGameToServer")]
    private void DebugSendEndGameToServer()
    {
        UserInfo userInfo = new UserInfo()
        {
            CheeseType = 0,
            PhoneColor = "TODO",
            PlayerId = "TODO",
        };
        _connectionManager.SendEndGameToServer(userInfo);
    }
    
    [ContextMenu("DebugSendMainMenuOpenedEndGameToServer")]
    private void DebugSendMainMenuOpenedEndGameToServer()
    {
        _connectionManager.SendMainMenuOpenedToServer();
    }
    
    private void SendMainMenuOpenedEndGameToServer()
    {
        _connectionManager.SendMainMenuOpenedToServer();
    }

    private void PlayMusic()
    {
        switch (_gamePhase)
        {
            case GamePhase.Play:
                _gameMusic?.Play();
                _menuMusic?.Stop();
                break;
            case GamePhase.Menu:
            case GamePhase.EndGame:
                _gameMusic?.Stop();
                _menuMusic?.Play();
                break;
                
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void RegisterPlayer(Player player)
    {
        if(!_players.Contains(player))
            _players.Add(player);
    }

    public void DeregisterPlayer(Player player)
    {
        _players.Remove(player);
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
        foreach (var cat in _activeCats)
        {
            cat.DestroyCat();
        }

        _activeCats.Clear();
    }

    private void DestroyAllPlayers()
    {
        foreach (var player in FindObjectsOfType<Player>().ToList())
        {
            Destroy(player.gameObject);
        }
        _players.Clear();
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
        Player player = _players.FirstOrDefault(p => p.UserInfo.PlayerId.Equals(playerID));
        player.Points++;

        if (!CheckIfSomeoneWin())
        {
            CreateCat();
        }
    }

    private bool CheckIfSomeoneWin()
    {
        foreach (var player in _players)
        {
            if (player.Points >= _pointsToWin)
            {
                EndGame(player);
                return true;
            }
        }

        return false;
    }
}