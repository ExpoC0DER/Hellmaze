using System;
using EditorAttributes;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _game.Scripts.Services
{
    public class Init : MonoBehaviour
    {
        [SerializeField, SceneDropdown] private string _scene;

        private async void Start()
        {
            await UnityServices.InitializeAsync();

            if (UnityServices.State == ServicesInitializationState.Initialized)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn)
                {
                    Debug.Log("User signed in!");

                    string username = PlayerPrefs.GetString("Username", "AnonymousPlayer");

                    SceneManager.LoadSceneAsync(_scene);
                }
            }
        }
    }
}
