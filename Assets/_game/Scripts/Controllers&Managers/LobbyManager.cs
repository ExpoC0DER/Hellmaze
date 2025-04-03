using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _game.Scripts.Definitions;
using _game.Scripts.Utils;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace _game.Scripts.Controllers_Managers
{
    public class LobbyManager : Singleton<LobbyManager>
    {
        private Unity.Services.Lobbies.Models.Lobby _lobby;
        private Coroutine _heartBeatCoroutine;
        private Coroutine _refreshCoroutine;
        private List<string> _joinedLobbies;

        public string GetLobbyCode() { return _lobby?.LobbyCode; }
        public string GetHostId() { return _lobby.HostId; }

        public async Task<bool> HasActiveLobbies()
        {
            _joinedLobbies = await LobbyService.Instance.GetJoinedLobbiesAsync();

            return _joinedLobbies.Count > 0;
        }


        public async Task<bool> CreateLobby(int maxPlayers, bool isPrivate, Dictionary<string, string> lobbyPlayerData, Dictionary<string, string> lobbyData)
        {
            Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(lobbyPlayerData);
            Unity.Services.Lobbies.Models.Player player = new Unity.Services.Lobbies.Models.Player(AuthenticationService.Instance.PlayerId, null, playerData);

            CreateLobbyOptions options = new CreateLobbyOptions
            {
                Data = SerializeLobbyData(lobbyData),
                IsPrivate = isPrivate,
                Player = player
            };

            try
            {
                _lobby = await LobbyService.Instance.CreateLobbyAsync("Lobby", maxPlayers, options);
            }
            catch (Exception e)
            {
                return false;
            }

            _heartBeatCoroutine = StartCoroutine(HeartBeatLobbyCoroutine(_lobby.Id, 6f));
            _refreshCoroutine = StartCoroutine(RefreshLobbyCoroutine(_lobby.Id, 1f));

            GUIUtility.systemCopyBuffer = _lobby.LobbyCode;

            Debug.Log($"Lobby created with lobby id {_lobby.Id}");
            return true;
        }
        private IEnumerator HeartBeatLobbyCoroutine(string lobbyId, float interval)
        {
            while (true)
            {
                // Debug.Log("Heartbeat");
                LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
                yield return new WaitForSecondsRealtime(interval);
            }
        }

        private IEnumerator RefreshLobbyCoroutine(string lobbyId, float interval)
        {
            while (true)
            {
                // Debug.Log("RefreshLobby");
                Task<Unity.Services.Lobbies.Models.Lobby> task = LobbyService.Instance.GetLobbyAsync(lobbyId);
                yield return new WaitUntil(() => task.IsCompleted);
                Unity.Services.Lobbies.Models.Lobby newLobby = task.Result;
                if (newLobby.LastUpdated > _lobby.LastUpdated)
                {
                    _lobby = newLobby;
                    LobbyEvents.OnLobbyUpdated?.Invoke(_lobby);
                }

                yield return new WaitForSecondsRealtime(interval);
            }
        }

        private static Dictionary<string, PlayerDataObject> SerializePlayerData(Dictionary<string, string> data)
        {
            Dictionary<string, PlayerDataObject> playerData = new();
            foreach ((string key, string value) in data)
            {
                playerData.Add(key, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, value));
            }

            return playerData;
        }

        private static Dictionary<string, DataObject> SerializeLobbyData(Dictionary<string, string> data)
        {
            Dictionary<string, DataObject> lobbyData = new();
            foreach ((string key, string value) in data)
            {
                lobbyData.Add(key, new DataObject(DataObject.VisibilityOptions.Member, value));
            }

            return lobbyData;
        }

        public void OnApplicationQuit()
        {
            if (_lobby != null && _lobby.HostId == AuthenticationService.Instance.PlayerId)
            {
                LobbyService.Instance.DeleteLobbyAsync(_lobby.Id);
            }
        }
        public async Task<bool> JoinLobby(string lobbyCode, Dictionary<string, string> playerData)
        {
            Unity.Services.Lobbies.Models.Player player = new Unity.Services.Lobbies.Models.Player(AuthenticationService.Instance.PlayerId, null, SerializePlayerData(playerData));

            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions
            {
                Player = player
            };


            try
            {
                _lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, options);
            }
            catch (Exception e)
            {
                return false;
            }

            _refreshCoroutine = StartCoroutine(RefreshLobbyCoroutine(_lobby.Id, 1f));
            return true;
        }
        public List<Dictionary<string, PlayerDataObject>> GetPlayerData() { return _lobby.Players.Select(player => player.Data).ToList(); }

        public async Task<bool> UpdatePlayerData(string playerId, Dictionary<string, string> data, string allocationId = default, string connectionData = default)
        {
            Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(data);
            UpdatePlayerOptions options = new UpdatePlayerOptions
            {
                Data = playerData,
                AllocationId = allocationId,
                ConnectionInfo = connectionData
            };

            try
            {
                _lobby = await LobbyService.Instance.UpdatePlayerAsync(_lobby.Id, playerId, options);
            }
            catch (Exception _)
            {
                return false;
            }

            LobbyEvents.OnLobbyUpdated(_lobby);

            return true;
        }

        public async Task<bool> UpdateLobbyData(Dictionary<string, string> data)
        {
            Dictionary<string, DataObject> lobbyData = SerializeLobbyData(data);

            UpdateLobbyOptions options = new UpdateLobbyOptions()
            {
                Data = lobbyData
            };

            try
            {
                _lobby = await LobbyService.Instance.UpdateLobbyAsync(_lobby.Id, options);
            }
            catch (Exception)
            {
                return false;
            }

            LobbyEvents.OnLobbyUpdated(_lobby);

            return true;
        }

        public async Task<bool> RejoinGame()
        {
            try
            {
                _lobby = await LobbyService.Instance.ReconnectToLobbyAsync(_joinedLobbies[0]);
                LobbyEvents.OnLobbyUpdated(_lobby);
            }
            catch (Exception)
            {
                return false;
            }

            _refreshCoroutine = StartCoroutine(RefreshLobbyCoroutine(_joinedLobbies[0], 1f));
            return true;
        }

        public async Task<bool> LeaveAllLobbies()
        {
            string playerId = AuthenticationService.Instance.PlayerId;
            foreach (string joinedLobbyId in _joinedLobbies)
            {
                try
                {
                    await LobbyService.Instance.RemovePlayerAsync(joinedLobbyId, playerId);
                }
                catch (Exception)
                {
                    return false;
                }

            }
            
            return true;
        }
    }
}
