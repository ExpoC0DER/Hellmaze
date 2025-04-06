using _game.Scripts.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using _game.Scripts.Definitions;
using _game.Scripts.Lobby;
using EditorAttributes;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace _game.Scripts.Controllers_Managers
{
    public class GameLobbyManager : Singleton<GameLobbyManager>
    {
        private List<LobbyPlayerData> _lobbyPlayerDatas = new List<LobbyPlayerData>();
        private LobbyPlayerData _localLobbyPlayerData;
        private LobbyData _lobbyData;
        private const int MAX_NUMBER_OF_PLAYERS = 4;
        private const string LOBBY_SCENE = "GameLobby";
        private bool _inGame;
        private bool _wasDisconnected;
        private string _previousRelayCode;

        public bool IsHost => _localLobbyPlayerData.Id == LobbyManager.Instance.GetHostId();
        public string GetLobbyCode() { return LobbyManager.Instance.GetLobbyCode(); }
        public List<LobbyPlayerData> GetPlayers() { return _lobbyPlayerDatas; }
        public int GetMapIndex() { return _lobbyData.MapIndex; }

        public async Task<bool> HasActiveLobbies() { return await LobbyManager.Instance.HasActiveLobbies(); }

        public async Task<bool> CreateLobby()
        {
            _localLobbyPlayerData = new LobbyPlayerData(AuthenticationService.Instance.PlayerId, "HostPlayer");
            _lobbyData = new LobbyData(0);
            bool success = await LobbyManager.Instance.CreateLobby(MAX_NUMBER_OF_PLAYERS, true, _localLobbyPlayerData.Serialize(), _lobbyData.Serialize());
            return success;
        }

        public async Task<bool> JoinLobby(string lobbyCode)
        {
            _localLobbyPlayerData = new LobbyPlayerData(AuthenticationService.Instance.PlayerId, "JoinPlayer");
            bool success = await LobbyManager.Instance.JoinLobby(lobbyCode, _localLobbyPlayerData.Serialize());

            return success;
        }

        private async void OnLobbyUpdated(Unity.Services.Lobbies.Models.Lobby lobby)
        {
            List<Dictionary<string, PlayerDataObject>> playerData = LobbyManager.Instance.GetPlayerData();
            _lobbyPlayerDatas.Clear();

            int numberOfPlayersReady = 0;
            foreach (Dictionary<string, PlayerDataObject> data in playerData)
            {
                LobbyPlayerData lobbyPlayerData = new LobbyPlayerData(data);

                if (lobbyPlayerData.IsReady)
                {
                    numberOfPlayersReady++;
                }

                if (lobbyPlayerData.Id == AuthenticationService.Instance.PlayerId)
                {
                    _localLobbyPlayerData = lobbyPlayerData;
                }

                _lobbyPlayerDatas.Add(lobbyPlayerData);
            }

            _lobbyData = new LobbyData(lobby.Data);

            GameLobbyEvents.OnGameLobbyUpdated?.Invoke();

            if (numberOfPlayersReady == lobby.Players.Count)
            {
                GameLobbyEvents.OnGameLobbyReady?.Invoke();
            }

            // Join Lobby
            if (_lobbyData.RelayJoinCode != default && !_inGame)
            {
                if (_wasDisconnected)
                {
                    if (_lobbyData.RelayJoinCode != _previousRelayCode)
                    {
                        await JoinRelayServer(_lobbyData.RelayJoinCode);
                        SceneManager.LoadSceneAsync(_lobbyData.SceneName);
                    }
                }
                else
                {
                    await JoinRelayServer(_lobbyData.RelayJoinCode);
                    SceneManager.LoadSceneAsync(_lobbyData.SceneName);
                }

            }
        }

        public async Task StartGame()
        {
            string joinRelayCode = await RelayManager.Instance.CreateRelay(MAX_NUMBER_OF_PLAYERS);
            _inGame = true;

            _lobbyData.RelayJoinCode = joinRelayCode;
            await LobbyManager.Instance.UpdateLobbyData(_lobbyData.Serialize());

            string allocationId = RelayManager.Instance.GetAllocationId();
            string connectionData = RelayManager.Instance.GetConnectionData();

            _localLobbyPlayerData.IsReady = false;

            await LobbyManager.Instance.UpdatePlayerData(_localLobbyPlayerData.Id, _localLobbyPlayerData.Serialize(), allocationId, connectionData);

            SceneManager.LoadSceneAsync(_lobbyData.SceneName);
        }

        public async Task<bool> SetPlayerReady()
        {
            _localLobbyPlayerData.IsReady = true;
            return await LobbyManager.Instance.UpdatePlayerData(_localLobbyPlayerData.Id, _localLobbyPlayerData.Serialize());
        }


        public async Task<bool> SetSelectedMap(int currentMapIndex, string sceneName)
        {
            _lobbyData.MapIndex = currentMapIndex;
            _lobbyData.SceneName = sceneName;
            return await LobbyManager.Instance.UpdateLobbyData(_lobbyData.Serialize());
        }

        private async Task<bool> JoinRelayServer(string relayJoinCode)
        {
            _inGame = true;
            await RelayManager.Instance.JoinRelay(relayJoinCode);

            string allocationId = RelayManager.Instance.GetAllocationId();
            string connectionData = RelayManager.Instance.GetConnectionData();

            _localLobbyPlayerData.IsReady = false;

            await LobbyManager.Instance.UpdatePlayerData(_localLobbyPlayerData.Id, _localLobbyPlayerData.Serialize(), allocationId, connectionData);

            return true;
        }

        public async Task<bool> RejoinGame() { return await LobbyManager.Instance.RejoinGame(); }

        public async Task<bool> LeaveAllLobbies() { return await LobbyManager.Instance.LeaveAllLobbies(); }

        public async void GoBackToLobby(bool wasDisconnected)
        {
            _inGame = false;
            _wasDisconnected = wasDisconnected;

            if (_wasDisconnected)
            {
                _previousRelayCode = _lobbyData.RelayJoinCode;
            }

            _localLobbyPlayerData.IsReady = false;
            await LobbyManager.Instance.UpdatePlayerData(_localLobbyPlayerData.Id, _localLobbyPlayerData.Serialize());
            SceneManager.LoadSceneAsync(LOBBY_SCENE);
        }

        private void OnEnable() { LobbyEvents.OnLobbyUpdated += OnLobbyUpdated; }

        private void OnDisable() { LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated; }
    }
}
