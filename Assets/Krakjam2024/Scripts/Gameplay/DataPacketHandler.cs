using System.Collections.Generic;
using System.Linq;
using Placuszki.Krakjam2024.Server;
using UnityEngine;

namespace Placuszki.Krakjam2024
{
    public class DataPacketHandler : MonoBehaviour
    {
        [Header("Prefabs")] [SerializeField] 
        private Player _playerPrefab;

        [Header("References")] [SerializeField]
        private GameplayServiceConsumer _gameplayServiceConsumer;

        private readonly List<Player> _players = new();

        private void Awake()
        {
            _gameplayServiceConsumer.OnDataPacketReceived += HandleDataPacket;
        }

        private void HandleDataPacket(DataPacket dataPacket)
        {
            string playerId = dataPacket.PlayerId;
            var player = _players.FirstOrDefault(p => p.Id.Equals(playerId));
            if (player == null)
            {
                player = CreatePlayer(playerId);
                _players.Add(player);
            }

            player.HandleDataPacket(dataPacket);
        }

        private Player CreatePlayer(string playerId)
        {
            Player player = Instantiate(_playerPrefab);
            player.Id = playerId;
            return player;
        }
    }
}