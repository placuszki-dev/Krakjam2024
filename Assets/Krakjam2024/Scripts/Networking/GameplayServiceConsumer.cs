using System;
using Placuszki.Krakjam2024.Server;
using UnityEngine;

namespace Placuszki.Krakjam2024
{
    public class GameplayServiceConsumer : MonoBehaviour
    {
        public event Action<DataPacket> OnDataPacketReceived;

        public void ConsumeDataPacket(DataPacket dataPacket)
        {
            OnDataPacketReceived?.Invoke(dataPacket);
        }
    }
}