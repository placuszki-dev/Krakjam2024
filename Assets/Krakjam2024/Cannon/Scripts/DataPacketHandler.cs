using System;
using System.Linq;
using Placuszki.Krakjam2024.Scripts;
using Placuszki.Krakjam2024.Server;
using UnityEngine;

namespace Placuszki.Krakjam2024
{
    public class DataPacketHandler : MonoBehaviour
    {
        [Header("Prefabs")] [SerializeField] private Player _playerPrefab;

        [Header("References")] [SerializeField]
        private GameplayServiceConsumer _gameplayServiceConsumer;

        [SerializeField] private Transform _playerParentsContainer;

        private void Awake()
        {
            _gameplayServiceConsumer.OnDataPacketReceived += HandleDataPacket;
            _gameplayServiceConsumer.OnUserInfoReceived += HandleUserInfo;
        }

        public void HandleUserInfo(UserInfo userInfo)
        {
            string playerId = userInfo.PlayerId;
            Player player = FindObjectsOfType<Player>().FirstOrDefault(p => p.GetPlayerId().Equals(playerId));
            
            if (player == null)
            {
                CreatePlayer(userInfo);
            }
        }
        
        public void HandleDataPacket(DataPacket dataPacket)
        {
            string playerId = dataPacket.PlayerId;
            Player player = FindObjectsOfType<Player>().FirstOrDefault(p => p.GetPlayerId().Equals(playerId));
            if (player == null)
            {
                Debug.LogError($"Received data packet with owner of non existing player: {dataPacket.PlayerId}");
            }

            player.HandleDataPacket(dataPacket);
        }

        private Player CreatePlayer(UserInfo userInfo)
        {
            Player player = Instantiate(_playerPrefab, GetFirstAvailableParent(), false);
            player.SetupPlayer(userInfo);
            return player;
        }

        private Transform GetFirstAvailableParent()
        {
            var positionTransforms = _playerParentsContainer.GetAllChildren();
            foreach (Transform t in positionTransforms)
            {
                if (t.childCount == 0)
                {
                    return t;
                }
            }

            throw new Exception("Can't find empty parent!!!");
        }
    }
}