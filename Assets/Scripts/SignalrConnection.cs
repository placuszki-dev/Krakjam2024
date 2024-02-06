using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Placuszki.Krakjam2024;
using Placuszki.Krakjam2024.Server;
using UnityEngine;
using UnityEngine.Events;

namespace WebglExample
{
    public class SignalrConnection : MonoBehaviour
    {
        private static bool _connected = false;

        public GameplayServiceConsumer _gameplayServiceConsumer;
        public GameObject PlayerPrefab;
        public UnityEvent<string> OnStarted;
        public UnityEvent<string> OnReconnecting;
        public UnityEvent<string> OnReconnected;
        public UnityEvent<string> OnClosed;
        public UnityEvent<string> OnError;
        public UnityEvent<Notification> OnNotification;
        public UnityEvent<Update> OnGameUpdate;
        public UnityEvent<Update> OnYouJoinedGame;
        public UnityEvent<PlayerInformation> OnNewPlayerJoinGame;
        public UnityEvent<string> OnAPlayerLeftGame;
        public bool Connected => _connected;

        private const string _URL = "http://217.182.74.11:8080/GameHub";
        private string _myId = "";
        private List<NetworkPlayer> _networkPlayers = new List<NetworkPlayer>();

        private void addNetworkPlayer(PlayerInformation playerInformation)
        {
            // var playerPrefab = Instantiate(PlayerPrefab, transform);
            // var networkMovment = playerPrefab.AddComponent<NetworkPlayer>();
            // networkMovment.ServerPlayerInformation = playerInformation;
            // networkMovment.Connection = this;
            // _networkPlayers.Add(networkMovment);
        }
        public void StartConnection()
        {
            gameObject.name = new Guid().ToString();
            startConnection(_URL, gameObject.name, nameof(Started), nameof(Error),
                nameof(Reconnecting), nameof(Reconnected), nameof(Closed), nameof(Notification),
                nameof(YouJoinedGame), nameof(NewPlayerJoinGame), nameof(APlayerLeftGame), nameof(GameUpdate), nameof(SendUserInfo), nameof(SendDataPacket));
            if (Debug.isDebugBuild)
            {
                Debug.Log("starting with name " + name);
            }
        }
        public void JoinGame(string name, string gameId)
        {
            if (!Connected) return;
            joinGame(name, gameId);
            if (Debug.isDebugBuild)
            {
                Debug.Log("joining to game wtih name : " + name + " and game id : " + gameId);
            }
        }
        public void LeaveGame()
        {
            if (!Connected) return;
            leaveGame();
            if (Debug.isDebugBuild)
            {
                Debug.Log("Leaving Game");
            }
        }
        public void SendMessageToAll(string message)
        {
            if (!Connected) return;
            sendMessageToAll(message);
            if (Debug.isDebugBuild)
            {
                Debug.Log("sending message " + message);
            }
        }
        public void Move(float x, float y, float z, float lookY)
        {
            if (!Connected) return;
            move(x, y, z, lookY);
            if (Debug.isDebugBuild)
            {
                if ((DateTime.Now - _lastMoveLog).TotalSeconds > 5)
                {
                    Debug.Log("moving to location x :" + x.ToString() + " y:" + y.ToString() + " z:" + z.ToString() +
                              "look at y:" + lookY.ToString());
                    _lastMoveLog = DateTime.Now;
                }
            }
        }
        
        //Callbacks
        private void Started(string id)
        {
            _myId = id;
            _connected = true;
            OnStarted?.Invoke(id);
            if (Debug.isDebugBuild)
            {
                Debug.Log("started with id " + id);
            }
        }
        private void Reconnecting(string error)
        {
            _connected = false;
            OnReconnecting?.Invoke(error);
            if (Debug.isDebugBuild)
            {
                Debug.Log("reconnecting with error " + error);
            }
        }
        private void Reconnected(string connectionId)
        {
            _connected = true;
            OnReconnected?.Invoke(connectionId);
            if (Debug.isDebugBuild)
            {
                Debug.Log("Reconnected with id " + connectionId);
            }
        }
        private void Closed(string error)
        {
            _connected = false;
            OnClosed?.Invoke(error);
            if (Debug.isDebugBuild)
            {
                Debug.Log("Closed with error " + error);
            }
        }
        private void Error(string error)
        {
            OnError?.Invoke(error);
            if (Debug.isDebugBuild)
            {
                Debug.Log("error occured " + error);
            }
        }
        private void Notification(string notificationJson)
        {
            OnNotification?.Invoke(JsonUtility.FromJson<Notification>(notificationJson));
            if (Debug.isDebugBuild)
            {
                Debug.Log("server notif " + notificationJson);
            }
        }
        public void YouJoinedGame(string updateJson)
        {
            var currentUpdate = JsonUtility.FromJson<Update>(updateJson);
            foreach (var playerInformation in currentUpdate.PlayerInformations)
            {
                if (playerInformation.Id == _myId) continue;
                addNetworkPlayer(playerInformation);
            }
            OnYouJoinedGame?.Invoke(currentUpdate);
            if (Debug.isDebugBuild)
            {
                Debug.Log("you joined game with current players " + updateJson);
            }
        }
        public void NewPlayerJoinGame(string playerJson)
        {
            var newPlayerInformation = JsonUtility.FromJson<PlayerInformation>(playerJson);
            addNetworkPlayer(newPlayerInformation);
            OnNewPlayerJoinGame?.Invoke(newPlayerInformation);
            if (Debug.isDebugBuild)
            {
                Debug.Log("joined " + playerJson);
            }
        }
        public void APlayerLeftGame(string id)
        {
            var player = _networkPlayers.FirstOrDefault(x => id == x.ServerPlayerInformation.Id);
            if (player != null)
                Destroy(player);
            OnAPlayerLeftGame?.Invoke(id);
            if (Debug.isDebugBuild)
            {
                Debug.Log("left " + id);
            }
        }
        private void GameUpdate(string updateJson)
        {
            OnGameUpdate?.Invoke(JsonUtility.FromJson<Update>(updateJson));
            if (Debug.isDebugBuild)
            {
                if ((DateTime.Now - _lastUpdateLog).TotalSeconds > 5)
                {
                    Debug.Log("game updated " + updateJson);
                    _lastUpdateLog = DateTime.Now;
                }
            }
        }
        
        public void SendUserInfo(string userInfoJson)
        {
            Debug.Log("SendUserInfo: " + userInfoJson);
            var userInfo = JsonUtility.FromJson<UserInfo>(userInfoJson);
            Debug.Log("ID: " + userInfo.PlayerId);
            Debug.Log("CheeseType: " + userInfo.CheeseType);
            Debug.Log("PhoneColor: " + userInfo.PhoneColor);
            _gameplayServiceConsumer.ConsumeUserInfo(userInfo);
        }
        
        public void SendDataPacket(string dataPacketJson)
        {
            Debug.Log("SendDataPacket: " + dataPacketJson);
            var dataPacket = JsonUtility.FromJson<DataPacket>(dataPacketJson); 
            Debug.Log("playerId: " + dataPacket.PlayerId);
            Debug.Log("x: " + dataPacket.X);
            Debug.Log("x: " + dataPacket.Y); // TODO: dekodowane JSONa nie działa w ogóle, napraw
            _gameplayServiceConsumer.ConsumeDataPacket(dataPacket);
        }
        
        private void OnApplicationQuit()
        {
            LeaveGame();
        }
        
        public void SendToServerMainMenuOpened()
        {
            Debug.Log("SendToServerMainMenuOpened");
            sendToServerMainMenuOpened();
        }
        
        public void SendToServerEndGame(int winningCheeseType)
        {
            Debug.Log("SendToServerEndGameOpened");
            sendToServerEndGame(winningCheeseType);
        }
        public void SendToServerVibratePhone(string playerID, int playerPoints)
        {
            Debug.Log("SendToServerEndGameOpened");
            sendToServerVibratePhone(playerID, playerPoints);
        }

        DateTime _lastUpdateLog = DateTime.Now;
        DateTime _lastMoveLog = DateTime.Now;

        [DllImport("__Internal")]
        private static extern void joinGame(string name, string gameId);
        [DllImport("__Internal")]
        private static extern void leaveGame();
        [DllImport("__Internal")]
        private static extern void sendMessageToAll(string message);
        [DllImport("__Internal")]
        private static extern void move(float x, float y, float z, float lookY);
        [DllImport("__Internal")]
        private static extern void sendToServerMainMenuOpened();
        [DllImport("__Internal")]
        private static extern void sendToServerEndGame(int winningCheeseType);
        [DllImport("__Internal")]
        private static extern void sendToServerVibratePhone(string playerID, int playerPoints);
        [DllImport("__Internal")]
        private static extern void startConnection(string url, string gameObjectName, string startCallbackName,
            string errorCallbackName, string reconnectingCallbackName, string reconnectedCallbackName,
            string closeCallbackName, string notificationCallbackName, string joinGameCallbackName,
            string newPlayerJoinGameCallbackName, string aPlayerLeftZoneCallbackName, string gameUpdateCallbackName, string sendUserInfoCallbackName, string sendDataPacketCallbackName);
    }
}