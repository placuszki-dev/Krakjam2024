using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Placuszki.Krakjam2024;

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
    public event Action<CheeseType> OnEndGame;

    public GameObject _ui;
    public GameObject _catprefab;
    public Transform[] _catSpawners;
    public int _catCount = 5;
    public int _pointsToWin = 10;
    // public ConnectionManager _connectionManager;
        
    [Space]
    public AudioSource _gameMusic;
    public AudioSource _menuMusic;
    public AudioSource _applauseSFX;

    [Space]
    [SerializeField] private CheeseScorePanel _goudaScorePanel;
    [SerializeField] private CheeseScorePanel _cheddarScorePanel;
    
    private List<Cat> _activeCats = new List<Cat>();
    private readonly List<Player> _players = new ();
    
    
    
    private GamePhase _gamePhase;

    private void Start()
    {
        // _connectionManager.Connected += ShowMenu;
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
        DestroyAllPlayers();
        _goudaScorePanel.SetScore(0, _pointsToWin);
        _cheddarScorePanel.SetScore(0, _pointsToWin);
        
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

    private void EndGame(CheeseType cheeseType)
    {
        SetPhase(GamePhase.EndGame);
        OnEndGame?.Invoke(cheeseType);

        _ui.SetActive(true);
        DestroyAllCats();

        PlayMusic();
        
        SendEndGameToServer(cheeseType);
    }
   
    private void SendEndGameToServer(CheeseType cheeseType)
    {
        // _connectionManager.SendEndGameToServer((int)cheeseType);
    }
    
    private void SendMainMenuOpenedEndGameToServer()
    {
        // _connectionManager.SendMainMenuOpenedToServer();
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
                _applauseSFX?.Play();
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
        player.SetPlayerScoreText(player.Points); // xd refactor this shit

        // _connectionManager.VibratePhone(playerID, player.Points);
        
        if (!UpdateScore())
        {
            CreateCat();
        }
    }

    private bool UpdateScore()
    {
        int cheddarPoints = 0;
        int goudaPoints = 0;
        foreach (var player in _players)
        {
            int points = player.Points;
            CheeseType cheeseType = (CheeseType) player.UserInfo.CheeseType;
            switch (cheeseType)
            {
                case CheeseType.Unknown:
                    goudaPoints += points;
                    break;
                case CheeseType.Gouda:
                    goudaPoints += points;
                    break;
                case CheeseType.Cheddar:
                    cheddarPoints += points;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        _goudaScorePanel.SetScore(goudaPoints, _pointsToWin);
        _cheddarScorePanel.SetScore(cheddarPoints, _pointsToWin);
        
        if (cheddarPoints >= _pointsToWin)
        {
            EndGame(CheeseType.Cheddar);
            return true;
        }
        
        if (goudaPoints >= _pointsToWin)
        {
            EndGame(CheeseType.Gouda);
            return true;
        }

        return false;
    }
}