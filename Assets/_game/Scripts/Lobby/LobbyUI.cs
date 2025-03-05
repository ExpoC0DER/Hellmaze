using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _game.Scripts.Lobby
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField] private Button _createLobbyButton;
        [SerializeField] private Button _joinLobbyButton;
        [SerializeField] private int _sceneId;

        private void Awake()
        {
            _createLobbyButton.onClick.AddListener(CreateGame);
            _joinLobbyButton.onClick.AddListener(JoinGame);
        }

        private async void CreateGame()
        {
            await Multiplayer.Instance.CreateLobby();
            Loader.LoadNetwork(_sceneId);
        }

        private async void JoinGame() { await Multiplayer.Instance.QuickJoinLobby(); }
    }

    public static class Loader
    {
        public static void LoadNetwork(int sceneId) { NetworkManager.Singleton.SceneManager.LoadScene("NetworkTest", LoadSceneMode.Single); }
    }
}
