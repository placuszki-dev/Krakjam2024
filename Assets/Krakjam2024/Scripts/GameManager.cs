using System.Collections.Generic;
using UnityEngine;

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

    public GameObject _catprefab;
    public Transform[] _catSpawners;
    public int _catCount = 5;

    private List<Cat> _activeCats;

    public void CatHit(string playerID)
    {
        CreateCat();
    }

    [ContextMenu("Start")]
    public void StartGame()
    {
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
}