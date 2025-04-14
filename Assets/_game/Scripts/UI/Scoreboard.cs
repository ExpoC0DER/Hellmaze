using System;
using System.Collections.Generic;
using _game.Scripts.Controllers_Managers;
using _game.Scripts.Definitions;
using _game.Scripts.Lobby;
using UnityEngine;

namespace _game.Scripts.UI
{
    public class Scoreboard : MonoBehaviour
    {
        [SerializeField] private ScoreEntry _scoreEntryPrefab;
        [SerializeField] private Transform _content;
        [SerializeField] private CanvasGroup _canvasGroup;

        private readonly Dictionary<string, ScoreEntry> _scores = new();

        private void Awake() { UpdateScoreboardRows(); }

        public void ShowScoreboard(bool value) { _canvasGroup.alpha = value ? 1 : 0; }

        private void UpdateScoreBoard(string killerPlayerId, string deadPlayerId, int _)
        {
            List<LobbyPlayerData> playerLobbyDatas = GameLobbyManager.Instance.GetPlayers();
            _scores[playerLobbyDatas[int.Parse(killerPlayerId)].Id].Kills++;
            _scores[playerLobbyDatas[int.Parse(deadPlayerId)].Id].Deaths++;
        }

        private void UpdateScoreboardRows()
        {
            List<LobbyPlayerData> playerLobbyDatas = GameLobbyManager.Instance.GetPlayers();
            int i = 0;
            foreach (LobbyPlayerData playerLobbyData in playerLobbyDatas)
            {
                if (_scores.ContainsKey(playerLobbyData.Id))
                    continue;

                ScoreEntry scoreEntry = Instantiate(_scoreEntryPrefab, _content);
                scoreEntry.PlayerName = $"Player {i}";
                _scores[playerLobbyData.Id] = scoreEntry;
                i++;
            }
        }

        private void OnEnable()
        {
            UIEvents.OnPlayerKill += UpdateScoreBoard;
            GameLobbyEvents.OnGameLobbyUpdated += UpdateScoreboardRows;
        }

        private void OnDisable()
        {
            UIEvents.OnPlayerKill -= UpdateScoreBoard;
            GameLobbyEvents.OnGameLobbyUpdated -= UpdateScoreboardRows;
        }
    }
}
