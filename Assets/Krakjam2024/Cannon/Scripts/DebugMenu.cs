using System.Linq;
using Placuszki.Krakjam2024;
using Placuszki.Krakjam2024.Scripts;
using Placuszki.Krakjam2024.Server;
using UnityEngine;
using Random = UnityEngine.Random;

public class DebugMenu : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            DebugRegisterPlayer();
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            DebugShootRandomPlayer();
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            DeleteAllPhones();
        }
    }

    private void DeleteAllPhones()
    {
        foreach (var player in FindObjectsOfType<Player>())
        {
            Destroy(player.gameObject);
        }
    }

    public void DebugRegisterPlayer()
    {
        var userInfo = new UserInfo()
        {
            PlayerId = "Bolo",
            PhoneColor = "#ff0000",
            CheeseType = 0,
        };
        FindObjectOfType<DataPacketHandler>().HandleUserInfo(userInfo);
    }
    
    public void DebugShootRandomPlayer()
    {
        var randomPlayer = FindObjectsOfType<Player>().ToList<Player>().GetRandomElement();
        DataPacket dp = new DataPacket()
        {
            PlayerId = "Bolo",
            X = Random.Range(-0.5f, 0.5f),
            Y = Random.Range(0.5f, 1),
        };
        FindObjectOfType<DataPacketHandler>().HandleDataPacket(dp);
    }
}
