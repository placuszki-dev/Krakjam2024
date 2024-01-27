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

    public void CatHit(int playerID)
    {

    }

    [ContextMenu("Start")]
    public void StartGame()
    {
        for (int i = 0; i < _catCount; i++)
        {
            var obj = Instantiate(_catprefab);
            obj.transform.position = _catSpawners[UnityEngine.Random.Range(0, _catSpawners.Length)].position;
            obj.GetComponent<Cat>().Init();
        }
    }
}