using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace _game.Scripts.Lobby
{
    [Serializable]
    public enum EncryptionType
    {
        DTLS, // Datagram Transport Layer Security
        WSS // Web Socket Secure
    }

    public class Multiplayer : MonoBehaviour
    {
        [SerializeField] private string _lobbyName = "Lobby";
        [SerializeField] private int _maxPlayers = 4;
        [SerializeField] private EncryptionType _encryption = EncryptionType.DTLS;
        public static Multiplayer Instance { get; private set; }

        public string PlayerId { get; private set; }
        public string PlayerName { get; private set; }

        private Unity.Services.Lobbies.Models.Lobby _currentLobby;

        private string _connectionType => _encryption == EncryptionType.DTLS ? DtlsEncryption : WssEncryption;

        private const float LobbyHeartbeatInterval = 20f;
        private const float LobbyPollForUpdatesInterval = 65f;
        private const string KeyJoinCode = "RelayJoinCode";
        private const string DtlsEncryption = "dtls";
        private const string WssEncryption = "wss";

        private CountdownTimer _heartbeatTimer = new CountdownTimer(LobbyHeartbeatInterval);
        private CountdownTimer _pollForUpdatesTimer = new CountdownTimer(LobbyPollForUpdatesInterval);

        private async void Start()
        {
            Instance = this;
            DontDestroyOnLoad(this);

            await Authenticate();

            _heartbeatTimer.OnTimerStop += () =>
            {
                HandleHeartbeatAsync();
                _heartbeatTimer.Start();
            };

            _pollForUpdatesTimer.OnTimerStop += () =>
            {
                HandlePollForUpdatesAsync();
                _pollForUpdatesTimer.Start();
            };
        }

        private async Task Authenticate() { await Authenticate($"Player{Random.Range(0, 1000)}"); }

        private async Task Authenticate(string playerName)
        {
            if (UnityServices.State == ServicesInitializationState.Uninitialized)
            {
                InitializationOptions options = new InitializationOptions();
                options.SetProfile(playerName);

                await UnityServices.InitializeAsync();
            }

            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log($"Signed in as {AuthenticationService.Instance.PlayerId}");
            };

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                PlayerId = AuthenticationService.Instance.PlayerId;
                PlayerName = playerName;
            }
        }

        public async Task CreateLobby()
        {
            try
            {
                Allocation allocation = await AllocateRelay();
                string relayJoinCode = await GetRelayJoinCode(allocation);

                CreateLobbyOptions options = new CreateLobbyOptions
                {
                    IsPrivate = false
                };

                _currentLobby = await LobbyService.Instance.CreateLobbyAsync(_lobbyName, _maxPlayers, options);
                Debug.Log($"Created lobby: {_currentLobby.Name}, with code: {_currentLobby.LobbyCode}");

                _heartbeatTimer.Start();
                _pollForUpdatesTimer.Start();

                await LobbyService.Instance.UpdateLobbyAsync(_currentLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        { KeyJoinCode, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                    }
                });

                //NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(ref allocation, _connectionType));

                //NetworkManager.Singleton.StartHost();
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"Failed to create lobby: {e.Message}");
            }
        }

        public async Task QuickJoinLobby()
        {
            try
            {
                _currentLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
                _pollForUpdatesTimer.Start();

                string relayJoinCode = _currentLobby.Data[KeyJoinCode].Value;
                JoinAllocation joinAllocation = await JoinAllocation(relayJoinCode);

                //NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, _connectionType));

                NetworkManager.Singleton.StartClient();
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"Failed to quick join lobby: {e.Message}");
            }
        }

        private async Task<Allocation> AllocateRelay()
        {
            try
            {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(_maxPlayers - 1); // exclude the host
                return allocation;
            }
            catch (RelayServiceException e)
            {
                Debug.LogError($"Failed to allocate relay: {e.Message}");
                return default;
            }
        }

        private async Task<string> GetRelayJoinCode(Allocation allocation)
        {
            try
            {
                string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                return relayJoinCode;
            }
            catch (RelayServiceException e)
            {
                Debug.LogError($"Failed to get relay join code: {e.Message}");
                return default;
            }
        }

        private async Task<JoinAllocation> JoinAllocation(string relayJoinCode)
        {
            try
            {
                JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
                return allocation;
            }
            catch (RelayServiceException e)
            {
                Debug.LogError($"Failed to join relay: {e.Message}");
                return default;
            }
        }

        private async Task HandleHeartbeatAsync()
        {
            try
            {
                await LobbyService.Instance.SendHeartbeatPingAsync(_currentLobby.Id);
                Debug.Log($"Sent heartbeat ping to lobby: {_currentLobby.Name}");
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"Failed to heartbeat lobby: {e.Message}");
            }
        }

        private async Task HandlePollForUpdatesAsync()
        {
            try
            {
                Unity.Services.Lobbies.Models.Lobby lobby = await LobbyService.Instance.GetLobbyAsync(_currentLobby.Id);
                Debug.Log($"Polled for updates on lobby: {lobby.Name}");
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"Failed to poll for updates lobby: {e.Message}");
            }
        }
    }
}
