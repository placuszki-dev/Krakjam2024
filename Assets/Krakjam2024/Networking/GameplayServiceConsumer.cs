using System;
using Placuszki.Krakjam2024.Server;
using UnityEngine;

namespace Placuszki.Krakjam2024
{
    public class GameplayServiceConsumer : MonoBehaviour
    {
        public event Action<DataPacket> OnDataPacketReceived;
        public event Action<UserInfo> OnUserInfoReceived;

        public void ConsumeDataPacket(DataPacket dataPacket)
        {
            OnDataPacketReceived?.Invoke(dataPacket);
        }

        public void ConsumeUserInfo(UserInfo userInfo)
        {
            OnUserInfoReceived?.Invoke(userInfo);
        }
    }
}