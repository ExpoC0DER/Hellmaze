using System;
using System.Collections.Generic;
using _game.Scripts.Controllers_Managers;
using _game.Scripts.Definitions;
using UnityEngine;

namespace _game.Scripts.Lobby
{
    public class LobbySpawner : MonoBehaviour
    {
        [SerializeField] private List<LobbyPlayer> _players;


        private void Start() { OnLobbyUpdated(); }

        private void OnLobbyUpdated()
        {
            List<LobbyPlayerData> playerDatas = GameLobbyManager.Instance.GetPlayers();

            for(int i = 0; i < _players.Count; i++)
            {
                if (i < playerDatas.Count)
                {
                    LobbyPlayerData data = playerDatas[i];
                    _players[i].SetData(data);
                }
                else
                {
                    _players[i].SetData(null);
                }
            }
        }

        private void OnEnable() { GameLobbyEvents.OnGameLobbyUpdated += OnLobbyUpdated; }

        private void OnDisable() { GameLobbyEvents.OnGameLobbyUpdated -= OnLobbyUpdated; }
    }
}
