using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using Placuszki.Krakjam2024;
using Placuszki.Krakjam2024.Server;
using UnityEditor;

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
    public event Action<string> OnEndGame;

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
                case GamePhase.EndGame:
                    ShowMenu();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void ShowMenu()
    {
        _gamePhase = GamePhase.Menu;
        //_ui.SetActive(true);
        PlayMusic();
        OnMenu?.Invoke();
    }
    
    [ContextMenu("StartGame")]
    public void StartGame()
    {
        _gamePhase = GamePhase.Play;
        //_ui.SetActive(false);
        PlayMusic();
        CreateCats();
        OnStartGame?.Invoke();
    }

    [ContextMenu("StopGame")]
    private void StopGame()
    {
        _gamePhase = GamePhase.Menu;

        // _ui.SetActive(true);
        PlayMusic();
        DestroyAllCats();
    }
    
    private void EndGame(int winningPlayerIndex)
    {
        _gamePhase = GamePhase.EndGame;
        OnEndGame?.Invoke(_players.ElementAt(winningPlayerIndex).Key);

        _ui.SetActive(true);
        DestroyAllCats();
        _players.Clear();
        _colors.Clear();

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

    public void RegisterPlayer(UserInfo userInfo)
    {
        _players.TryAdd(userInfo.PlayerId, 0);
        _colors.TryAdd(userInfo.PlayerId, userInfo.PhoneColor);
    }

    public void DeregisterPlayer(UserInfo userInfo)
    {
        _players.Remove(userInfo.PlayerId);
        _colors.Remove(userInfo.PlayerId);
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
                EndGame(i);
                return true;
            }
        }

        return false;
    }
}