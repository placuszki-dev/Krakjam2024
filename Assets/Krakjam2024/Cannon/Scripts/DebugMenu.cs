using Placuszki.Krakjam2024;
using Placuszki.Krakjam2024.Server;
using UnityEngine;
using Random = UnityEngine.Random;

public class DebugMenu : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
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

    public void DebugShoot()
    {
        DataPacket dp = new DataPacket()
        {
            PhoneColor = "#ff0000",
            PlayerId = "Bolo",
            X = Random.Range(-0.5f, 0.5f),
            Y = Random.Range(0.5f, 1),
        };
        FindObjectOfType<DataPacketHandler>().HandleDataPacket(dp);
    }
}
