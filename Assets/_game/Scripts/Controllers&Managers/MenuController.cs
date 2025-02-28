using System;
using EditorAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _game.Scripts.Controllers_Managers
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _joinButton;
        [SerializeField] private Button _submitCodeButton;

        [SerializeField] private TMP_InputField _lobbyCodeInput;

        [SerializeField] private GameObject _selectionScreen;
        [SerializeField] private GameObject _joinScreen;

        [SerializeField, SceneDropdown] private string _lobbyScene;

        private void Start()
        {
            _selectionScreen.SetActive(true);
            _joinScreen.SetActive(false);
        }

        private async void OnHostClicked()
        {
            print("Host clicked");
            bool success = await GameLobbyManager.Instance.CreateLobby();

            if (success)
            {
                SceneManager.LoadSceneAsync(_lobbyScene);
            }
        }

        private void OnJoinClicked()
        {
            print("Join clicked");
            _selectionScreen.SetActive(false);
            _joinScreen.SetActive(true);
        }

        private async void OnSubmitCodeClicked()
        {
            string lobbyCode = _lobbyCodeInput.text;
            lobbyCode = lobbyCode.Trim();

            print("Submit code clicked. Code: " + lobbyCode);

            bool success = await GameLobbyManager.Instance.JoinLobby(lobbyCode);

            if (success)
            {
                SceneManager.LoadSceneAsync(_lobbyScene);
            }
        }

        private void OnEnable()
        {
            _hostButton.onClick.AddListener(OnHostClicked);
            _joinButton.onClick.AddListener(OnJoinClicked);
            _submitCodeButton.onClick.AddListener(OnSubmitCodeClicked);
        }

        private void OnDisable()
        {
            _hostButton.onClick.RemoveListener(OnHostClicked);
            _joinButton.onClick.RemoveListener(OnJoinClicked);
            _submitCodeButton.onClick.RemoveListener(OnSubmitCodeClicked);

        }
    }
}
