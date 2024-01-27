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
        }

        public void HandleDataPacket(DataPacket dataPacket)
        {
            string playerId = dataPacket.PlayerId;
            Player player = FindObjectsOfType<Player>().FirstOrDefault(p => p.GetPlayerId().Equals(playerId));
            if (player == null)
            {
                player = CreatePlayer(playerId);
            }

            player.HandleDataPacket(dataPacket);
        }

        private Player CreatePlayer(string playerId)
        {
            Player player = Instantiate(_playerPrefab, GetFirstAvailableParent(), false);
            player.Id = playerId;
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