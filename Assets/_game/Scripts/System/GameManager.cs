using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace _game.Scripts
{
	public class GameManager : MonoBehaviour
	{
		public InputSystem_Actions playerControlls;
		public PlayerDatabase playerDatabase;
		public bool isMainMenu {get; private set;} = true;
		private float _timer = -1;

		public event Action<float> OnTimerChange;
		public event Action OnSceneLoad;
		public event Action OnBeforeSceneLoad;
		
		public bool loadingScene = true;
		
		public static GameManager main { get; private set; }
		
		void Awake()
		{
			if(main != null)
			{
				Destroy(this.gameObject);
			}else
			{
				main = this;
			}
			DontDestroyOnLoad(this.gameObject);
			Time.timeScale = 1;
			isMainMenu = SceneManager.GetActiveScene().name == "MainMenu";
			loadingScene = false;
			playerControlls = new InputSystem_Actions();
		}

		private void Update()
		{
			if(_timer >= 0)
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
		
		void OnEnable()
		{
			playerControlls.Enable();
		}
		
		void OnDisable()
		{
			playerControlls?.Disable();
		}
		
		public void SetTimer(float gameTime)
		{
			_timer = gameTime;
		}
		
		public void LoadScene(string name)
		{
			loadingScene = true;
			OnBeforeSceneLoad?.Invoke();
			SceneManager.LoadScene(name);
			isMainMenu = name == "MainMenu";
			loadingScene = false;
			OnSceneLoad?.Invoke();
			SetupGameManager();
		}
		public void LoadScene(int index)
		{
			loadingScene = true;
			OnBeforeSceneLoad?.Invoke();
			SceneManager.LoadScene(index);
			isMainMenu = index == 0;
			loadingScene = false;
			OnSceneLoad?.Invoke();
			SetupGameManager();
		}
		
		void SetupGameManager()
		{
			Time.timeScale = 1;
			if(isMainMenu) SetTimer(-1);
			else SetTimer(120);
		}
	}
}
