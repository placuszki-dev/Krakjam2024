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
        if (Input.GetKeyUp(KeyCode.T))
        {
            DebugRegisterPlayers();
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            DebugShootRandomPlayer();
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            DebugShootRandomPlayers();
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
            PlayerId = "DebugUser0",
            PhoneColor = "#ff0000",
            CheeseType = 1,
        };
        FindObjectOfType<DataPacketHandler>().HandleUserInfo(userInfo);
    }
    
    public void DebugRegisterPlayers()
    {
        var dataPacketHandler = FindObjectOfType<DataPacketHandler>();
        dataPacketHandler.HandleUserInfo(new UserInfo()
        {
            PlayerId = "DebugUser1",
            PhoneColor = "#ff0000",
            CheeseType = 1,
        });
        dataPacketHandler.HandleUserInfo(new UserInfo()
        {
            PlayerId = "DebugUser2",
            PhoneColor = "#85a605",
            CheeseType = 2,
        });
        dataPacketHandler.HandleUserInfo(new UserInfo()
        {
            PlayerId = "DebugUser3",
            PhoneColor = "#696e98",
            CheeseType = 1,
        });
        dataPacketHandler.HandleUserInfo(new UserInfo()
        {
            PlayerId = "DebugUser4",
            PhoneColor = "#9860e5",
            CheeseType = 1,
        });
        dataPacketHandler.HandleUserInfo(new UserInfo()
        {
            PlayerId = "DebugUser5",
            PhoneColor = "#fd5fef",
            CheeseType = 1,
        });
        dataPacketHandler.HandleUserInfo(new UserInfo()
        {
            PlayerId = "DebugUser6",
            PhoneColor = "#54a1fd",
            CheeseType = 2,
        });
        dataPacketHandler.HandleUserInfo(new UserInfo()
        {
            PlayerId = "DebugUser7",
            PhoneColor = "#951d6d",
            CheeseType = 2,
        });
        dataPacketHandler.HandleUserInfo(new UserInfo()
        {
            PlayerId = "DebugUser8",
            PhoneColor = "#089f3e",
            CheeseType = 2,
        });
        dataPacketHandler.HandleUserInfo(new UserInfo()
        {
            PlayerId = "DebugUser9",
            PhoneColor = "#cfca3d",
            CheeseType = 1,
        });
        dataPacketHandler.HandleUserInfo(new UserInfo()
        {
            PlayerId = "DebugUser10",
            PhoneColor = "#57e047",
            CheeseType = 2,
        });
        dataPacketHandler.HandleUserInfo(new UserInfo()
        {
            PlayerId = "DebugUser11",
            PhoneColor = "#e17d79",
            CheeseType = 1,
        });
        dataPacketHandler.HandleUserInfo(new UserInfo()
        {
            PlayerId = "DebugUser12",
            PhoneColor = "#99627a",
            CheeseType = 2,
        });
    }

    private void DebugShootRandomPlayer()
    {
        DataPacket dp = new DataPacket()
        {
            PlayerId = "DebugUser0",
            X = Random.Range(-0.5f, 0.5f),
            Y = Random.Range(0.5f, 1),
        };
        FindObjectOfType<DataPacketHandler>().HandleDataPacket(dp);
    }

    private void DebugShootRandomPlayers()
    {
        DataPacketHandler dataPacketHandler = FindObjectOfType<DataPacketHandler>();

        for (int i = 1; i <= 12; i++)
        {
            if(Random.Range(0, 100) > 70f)
                continue;
            
            DataPacket dp = new DataPacket()
            {
                PlayerId = $"DebugUser{i}",
                X = Random.Range(-0.5f, 0.5f),
                Y = Random.Range(0.5f, 1),
            };
            dataPacketHandler.HandleDataPacket(dp);
        }
    }
}
