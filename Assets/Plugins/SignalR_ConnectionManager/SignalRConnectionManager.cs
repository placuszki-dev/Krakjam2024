using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Placuszki.Krakjam2024
{
    /// <summary>
    /// To use this class declare project wide preprocessor directive USE_SIGNALR
    /// </summary>
    public class SignalRConnectionManager
    {
        public event Action OnDisconnect;

        public event Action ConnectionCallback;
        public event Action<string> Log;

        public bool IsConnected => _isConnected;
        public HubConnection Connection { get; private set; }

        private CancellationTokenSource _connectTokenSource;
        private int _betweenConnectionTime = 1000;
        private volatile bool _isConnected;
        private Action<Action> _dispatchOnMainThread;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="serverTimeout">connection timeout must be the same as servers</param>
        /// <param name="keepAlive">keep alive ping frequency</param>
        /// <param name="reconnectTime">reconnect frequency</param>
        public SignalRConnectionManager(string connectionString, TimeSpan serverTimeout, TimeSpan keepAlive, TimeSpan reconnectTime, bool useBuiltinReconnect = false)
        {
            SetupConnection(connectionString, serverTimeout, keepAlive, reconnectTime, useBuiltinReconnect);
        }

        public SignalRConnectionManager(string connectionString) : this(connectionString, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(8))
        {
        }

        public void SetDispatchMethod(Action<Action> dispatchMethod)
        {
            _dispatchOnMainThread = dispatchMethod;
        }

        public async Task<bool> ConnectAsync()
        {
            if (IsConnected)
            {
                Log?.Invoke("[SignalR] Already connected.");
                return false;
            }

            _connectTokenSource = new CancellationTokenSource();
            return await ConnectionLoop(_connectTokenSource.Token);
        }

        public async Task StopConnection()
        {
            _connectTokenSource.Cancel();
            try
            {
                await Connection.StopAsync();
            }
            finally
            {
                await Connection.DisposeAsync();
            }
        }

        private void SetupConnection(string connectionString, TimeSpan serverTimeout, TimeSpan keepAlive, TimeSpan reconnectTime, bool useBuiltinReconnect = false)
        {
            if (Connection != null)
            {
                Log?.Invoke("Could not create connection because previous is in use");
                return;
            }

            var connectionBuilder = new HubConnectionBuilder()
                .AddNewtonsoftJsonProtocol()
                .WithUrl(connectionString);
            if (useBuiltinReconnect)
            {
                connectionBuilder.WithAutomaticReconnect(new RetryPolicy(reconnectTime));
            }

            Connection = connectionBuilder.Build();
            Connection.Closed += OnClosed;
            Connection.Reconnecting += OnReconnecting;
            if (useBuiltinReconnect)
            {
                Connection.Reconnected += OnReconnected;
            }

            Connection.ServerTimeout = serverTimeout;
            Connection.KeepAliveInterval = keepAlive;
            _isConnected = false;
        }

        private Task OnReconnected(string arg)
        {
            OnConnectedCallbacks();
            return Task.CompletedTask;
        }

        private Task OnReconnecting(Exception error)
        {
            if (_isConnected)
            {
                OnClosed(error);
            }

            Log?.Invoke("[SignalR] Reconnecting ...");
            return Task.CompletedTask;
        }

        private async Task OnClosed(Exception error)
        {
            _isConnected = false;
            Log?.Invoke("[SignalR] Client disconnected!");
            try
            {
                await Connection.DisposeAsync();
            }
            catch
            {
                Log?.Invoke($"[SignalR] could not dispose connection");
            }

            if (_dispatchOnMainThread != null)
            {
                _dispatchOnMainThread(OnDisconnect);
            }
            else
            {
                OnDisconnect?.Invoke();
            }
        }

        private async Task<bool> ConnectionLoop(CancellationToken token)
        {
            if (IsConnected)
            {
                Log?.Invoke("[SignalR] Already connected, skipping connection loop");
                return false;
            }

            Log?.Invoke("[SignalR] Connection started:");
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    return false;
                }

                var success = await Connect();
                _isConnected = success;
                if (success)
                {
                    OnConnectedCallbacks();
                    return true;
                }

                await Task.Delay(_betweenConnectionTime);
            }
        }

        private async Task<bool> Connect()
        {
            try
            {
                await Connection.StartAsync();
                return true;
            }
            catch (Exception _)
            {
                Log?.Invoke("[SignalR] Retrying connect.");
            }

            return false;
        }

        private void OnConnectedCallbacks()
        {
            Log?.Invoke("[SignalR] Calling connected callbacks");
            if (_dispatchOnMainThread != null)
            {
                _dispatchOnMainThread(ConnectionCallback);
            }
            else
            {
                ConnectionCallback?.Invoke();
            }
        }

        public void AddDispatchedHandler(string functionName, Func<Task> func)
        {
            Connection.On(functionName, () => WrapFunction(func));
        }

        public void AddDispatchedHandler<T>(string functionName, Func<T, Task> func)
        {
            Connection.On<T>(functionName, (arg) => WrapFunction(func, arg));
        }

        public void AddDispatchedHandler<T1, T2>(string functionName, Func<T1, T2, Task> func)
        {
            Connection.On<T1, T2>(functionName, (arg1, arg2) => WrapFunction(func, arg1, arg2));
        }

        private void WrapFunction(Func<Task> action)
        {
            if (_dispatchOnMainThread != null)
            {
                _dispatchOnMainThread(() => { action?.Invoke(); });
            }
            else
            {
                action?.Invoke();
            }
        }

        private void WrapFunction<T>(Func<T, Task> action, T arg)
        {
            if (_dispatchOnMainThread != null)
            {
                _dispatchOnMainThread(() => { action?.Invoke(arg); });
            }
            else
            {
                action?.Invoke(arg);
            }
        }

        private void WrapFunction<T1, T2>(Func<T1, T2, Task> action, T1 arg1, T2 arg2)
        {
            if (_dispatchOnMainThread != null)
            {
                _dispatchOnMainThread(() => { action?.Invoke(arg1, arg2); });
            }
            else
            {
                action?.Invoke(arg1, arg2);
            }
        }

        private void SendDispatchedError(string errorMsg)
        {
            if (_dispatchOnMainThread != null)
            {
                _dispatchOnMainThread(() => { Log?.Invoke($"[SignalR] error during sendAsync {errorMsg}"); });
            }
            else
            {
                Log?.Invoke($"[SignalR] error during sendAsync {errorMsg}");
            }
        }

        public async Task SendRequestToServer(string funcName)
        {
            try
            {
                await Connection.InvokeAsync(funcName);
            }
            catch (Exception e)
            {
                SendDispatchedError(e.Message);
            }
        }

        public async Task SendRequestToServer<T>(string funcName, T arg1)
        {
            try
            {
                await Connection.InvokeAsync(funcName, arg1);
            }
            catch (Exception e)
            {
                SendDispatchedError(e.Message);
            }
        }

        public async Task SendRequestToServer<T1, T2>(string funcName, T1 arg1, T2 arg2)
        {
            try
            {
                await Connection.InvokeAsync(funcName, arg1, arg2);
            }
            catch (Exception e)
            {
                SendDispatchedError(e.Message);
            }
        }

        public async Task SendRequestToServer<T1, T2, T3>(string funcName, T1 arg1, T2 arg2, T3 arg3)
        {
            try
            {
                await Connection.InvokeAsync(funcName, arg1, arg2, arg3);
            }
            catch (Exception e)
            {
                SendDispatchedError(e.Message);
            }
        }

        public async Task SendRequestToServer<T1, T2, T3, T4>(string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            try
            {
                await Connection.InvokeAsync(funcName, arg1, arg2, arg3, arg4);
            }
            catch (Exception e)
            {
                SendDispatchedError(e.Message);
            }
        }

        public class RetryPolicy : IRetryPolicy
        {
            private TimeSpan _reconnectDelay;

            public RetryPolicy(TimeSpan reconnectDelay)
            {
                _reconnectDelay = reconnectDelay;
            }

            TimeSpan? IRetryPolicy.NextRetryDelay(RetryContext retryContext)
            {
                return _reconnectDelay;
            }
        }
    }
}