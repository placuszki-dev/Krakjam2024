using Placuszki.Krakjam2024;
using Placuszki.Krakjam2024.Server;
using UnityEngine;

public class DebugMenu : MonoBehaviour
{
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
