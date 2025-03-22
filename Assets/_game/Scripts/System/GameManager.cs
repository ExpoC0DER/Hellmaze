using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUtils;

namespace _game.Scripts.System
{
    public class GameManager : Singleton<GameManager>
    {
        public InputSystem_Actions playerControlls;
        public PlayerDatabase playerDatabase;

        private float _timer = -1;

        public event Action<float> OnTimerChange;
        public event Action OnSceneLoad;
        public event Action OnBeforeSceneLoad;

        public bool loadingScene = true;

        public bool GamePaused { get; private set; }
        public bool GameEnded { get; set; }

        public void SetGamePaused(bool value)
        {
            if (value)
            {
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            Time.timeScale = 1;
            loadingScene = false;
            playerControlls = new InputSystem_Actions();
        }

        private void Update()
        {
            if (_timer >= 0)
            {
                _timer -= Time.deltaTime;
                OnTimerChange?.Invoke(_timer);
                if (_timer <= 0)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    Time.timeScale = 0;
                }
            }
        }

        void OnEnable() { playerControlls.Enable(); }

        void OnDisable() { playerControlls?.Disable(); }

        private void SetTimer(float gameTime) { _timer = gameTime; }

        public void LoadScene(string name)
        {
            loadingScene = true;
            OnBeforeSceneLoad?.Invoke();
            SceneManager.LoadScene(name);
            loadingScene = false;
            OnSceneLoad?.Invoke();
            SetupGameManager();
        }
        public void LoadScene(int index)
        {
            loadingScene = true;
            OnBeforeSceneLoad?.Invoke();
            SceneManager.LoadScene(index);
            loadingScene = false;
            OnSceneLoad?.Invoke();
            SetupGameManager();
        }

        private void SetupGameManager()
        {
            Time.timeScale = 1;
            SetTimer(120);
        }
    }
}
