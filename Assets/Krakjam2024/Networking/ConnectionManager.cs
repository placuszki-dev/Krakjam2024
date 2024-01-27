using System;
using System.Collections;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Placuszki.Krakjam2024.Server;
using UnityEngine;
using UnityEngine.Events;

namespace Placuszki.Krakjam2024
{
    public class ConnectionManager : MonoBehaviour, IGameHub
    {
        public event Action Connected;
        public event Action Disconnected;

        public UnityEvent ConnectingEvent;
        public UnityEvent ConnectedEvent;
        public UnityEvent DisconnectedEvent;

        public bool IsConnected { get; private set; }

        [SerializeField] private GameplayServiceConsumer _gameplayServiceConsumer;
        [SerializeField] private bool _useLocalhost;
        private SignalRConnectionManager _connectionManager;
        private IGameHub _client;
        private Coroutine _startConnectionCoroutine;
        private bool _stoppedManually;

        void Awake()
        {
        }

        void OnEnable()
        {
        }

        void Start()
        {
            _client = this;
            StartConnectionCoroutine();
        }

        void OnDisable()
        {
        }

        void OnDestroy()
        {
            StopConnection();
        }

        public Task SendDataPacket(DataPacket dataPacket)
        {
            _gameplayServiceConsumer.ConsumeDataPacket(dataPacket);
            return Task.CompletedTask;
        }

        private void StartConnectionCoroutine()
        {
            _startConnectionCoroutine = StartCoroutine(StartConnection());
        }

        private void RestartConnectionCoroutine()
        {
            if (_connectionManager != null)
            {
                StopConnection();
            }
            else if (_startConnectionCoroutine != null)
            {
                StopCoroutine(_startConnectionCoroutine);
                _startConnectionCoroutine = null;
            }

            StartConnectionCoroutine();
        }

        private IEnumerator StartConnection()
        {
            yield return new WaitForEndOfFrame();

            if (!IsConnected)
            {
                Init();
                Connect();
            }
        }

        private void StopConnection()
        {
            if (_startConnectionCoroutine != null)
            {
                StopCoroutine(_startConnectionCoroutine);
            }

            _stoppedManually = true;
            _connectionManager.StopConnection();
            Disconnected?.Invoke();
            DisconnectedEvent.Invoke();
            IsConnected = false;
        }

        private void Init()
        {
            var connectionString = GetConnectionString();
            Debug.Log($"ConnectionString: {connectionString}");
            try
            {
                _connectionManager = new SignalRConnectionManager(
                    connectionString,
                    TimeSpan.FromSeconds(ConnectionValues.TimeoutSeconds),
                    TimeSpan.FromSeconds(ConnectionValues.KeepAliveSeconds),
                    TimeSpan.FromSeconds(ConnectionValues.ReconnectTime));
            }
            catch (Exception e)
            {
                string msg = $"[SignalR] exception in SignalRConnectionManager cctor: {e.Message}";
                Debug.LogError(msg);
                StartConnectionCoroutine();
                return;
            }

            _connectionManager.OnDisconnect += Disconnect;
            _connectionManager.ConnectionCallback += OnConnected;
            _connectionManager.Connection.On<DataPacket>(nameof(IGameHub.SendDataPacket), _client.SendDataPacket);
            _connectionManager.Log += OnLog;
        }

        private void Disconnect()
        {
            UnityMainThreadDispatcher.Instance().EnqueueAsync(() => DisconnectedEvent.Invoke());

            if (!_stoppedManually)
            {
                _connectionManager.Connection.DisposeAsync();
                UnityMainThreadDispatcher.Instance().EnqueueAsync(() => _startConnectionCoroutine = StartCoroutine(StartConnection()));
            }

            Disconnected?.Invoke();
            IsConnected = false;
        }

        private void Connect()
        {
            if (_connectionManager == null)
            {
                Debug.Log("_connectionManager is null, can't connect");
                return;
            }

            ConnectingEvent.Invoke();
            _stoppedManually = false;
            _connectionManager.ConnectAsync();
        }

        private void OnConnected()
        {
            UnityMainThreadDispatcher.Instance().EnqueueAsync(() =>
            {
                Debug.Log("[SignalR] Connected to the server");
                IsConnected = true;
                Connected?.Invoke();
                ConnectedEvent?.Invoke();
            });
        }

        private void OnLog(string msg)
        {
            Debug.Log($"[{nameof(ConnectionManager)}] {msg}");
        }

        private string GetConnectionString()
        {
            return _useLocalhost ? GetLocalConnectionString() : GetRemoteConnectionString();
        }
        
        private string GetLocalConnectionString()
        {
            string customServerIp = "localhost";
            string port = "80";
            string hub = "hubs/gamehub";

            return $"http://{customServerIp}:{port}/{hub}";
        }
        
        private string GetRemoteConnectionString()
        {
            string customServerIp = "217.182.74.11";
            string port = "8080";
            string hub = "hubs/gamehub";

            return $"http://{customServerIp}:{port}/{hub}";
        }
    }
}