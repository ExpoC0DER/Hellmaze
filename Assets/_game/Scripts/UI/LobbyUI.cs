using System;
using _game.Scripts.Controllers_Managers;
using _game.Scripts.Definitions;
using _game.Scripts.Lobby;
using EditorAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _game.Scripts.UI
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField, SceneDropdown] private string _menuSceneName;
        [SerializeField] private TMP_Text _lobbyCodeText;
        [SerializeField] private Button _startBtn;
        [SerializeField] private Button _readyBtn;
        [SerializeField] private Button _leaveBtn;
        [SerializeField] private Image _mapImage;
        [SerializeField] private Button _prevButton;
        [SerializeField] private Button _nextButton;
        [SerializeField] private TMP_Text _mapName;
        [SerializeField, PropertyDropdown] private MapSelectionData _mapSelectionData;

        private int _currentMapIndex;

        private void Start()
        {
            _lobbyCodeText.text = "Lobby code : " + GameLobbyManager.Instance.GetLobbyCode();

            if (!GameLobbyManager.Instance.IsHost)
            {
                _prevButton.gameObject.SetActive(false);
                _nextButton.gameObject.SetActive(false);
            }
            else
            {
                GameLobbyManager.Instance.SetSelectedMap(_currentMapIndex, _mapSelectionData.Maps[_currentMapIndex].SceneName);
            }
        }

        private async void OnReadyPressed()
        {
            bool success = await GameLobbyManager.Instance.SetPlayerReady();
            if (success)
                _readyBtn.gameObject.SetActive(false);
        }

        private async void OnNextClicked()
        {
            _currentMapIndex++;
            if (_currentMapIndex >= _mapSelectionData.Maps.Count)
                _currentMapIndex = 0;

            UpdateMap();
            await GameLobbyManager.Instance.SetSelectedMap(_currentMapIndex, _mapSelectionData.Maps[_currentMapIndex].SceneName);
        }

        private async void OnPrevClicked()
        {
            _currentMapIndex--;
            if (_currentMapIndex < 0)
                _currentMapIndex = _mapSelectionData.Maps.Count - 1;

            UpdateMap();
            await GameLobbyManager.Instance.SetSelectedMap(_currentMapIndex, _mapSelectionData.Maps[_currentMapIndex].SceneName);
        }

        private async void OnStartClicked()
        {
            // await GameLobbyManager.Instance.SetSelectedMap(_currentMapIndex,_mapSelectionData.Maps[_currentMapIndex].SceneName);
            await GameLobbyManager.Instance.StartGame();
        }

        private void UpdateMap()
        {
            _mapImage.color = _mapSelectionData.Maps[_currentMapIndex].MapThumbnail;
            _mapName.text = _mapSelectionData.Maps[_currentMapIndex].MapName;
        }

        private void OnLobbyUpdated(Unity.Services.Lobbies.Models.Lobby lobby)
        {
            _currentMapIndex = GameLobbyManager.Instance.GetMapIndex();
            UpdateMap();
        }

        private void OnGameLobbyReady() { _startBtn.gameObject.SetActive(true); }

        private async void OnLeavePressed()
        {
            bool succeeded = await GameLobbyManager.Instance.LeaveCurrentLobby();
            
            if (succeeded)
                SceneManager.LoadSceneAsync(_menuSceneName);
        }

        private void OnEnable()
        {
            _readyBtn.onClick.AddListener(OnReadyPressed);
            _leaveBtn.onClick.AddListener(OnLeavePressed);

            if (GameLobbyManager.Instance.IsHost)
            {
                _prevButton.onClick.AddListener(OnPrevClicked);
                _nextButton.onClick.AddListener(OnNextClicked);
                _startBtn.onClick.AddListener(OnStartClicked);
                GameLobbyEvents.OnGameLobbyReady += OnGameLobbyReady;
            }

            LobbyEvents.OnLobbyUpdated += OnLobbyUpdated;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void OnDisable()
        {
            _readyBtn.onClick.RemoveListener(OnReadyPressed);
            _leaveBtn.onClick.RemoveListener(OnLeavePressed);
            _prevButton.onClick.RemoveListener(OnPrevClicked);
            _nextButton.onClick.RemoveListener(OnNextClicked);
            _startBtn.onClick.RemoveListener(OnStartClicked);

            LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;
            GameLobbyEvents.OnGameLobbyReady -= OnGameLobbyReady;
        }
    }
}
