using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _game.Scripts
{
	public class GameManager : MonoBehaviour
	{
		public bool isMainMenu {get; private set;} = false;
		
		private float _timer = 180f;

		public event Action<float> OnTimerChange;
		public event Action OnSceneLoad;
		public event Action OnBeforeSceneLoad;
		
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
			isMainMenu = SceneManager.GetActiveScene().name == "MainMenu";
		}

		private void Start()
		{
			
			
			Time.timeScale = 1;
		}

		private void Update()
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
		
		public void LoadScene(string name)
		{
			OnBeforeSceneLoad?.Invoke();
			SceneManager.LoadScene(name);
			isMainMenu = name == "MainMenu";
			OnSceneLoad?.Invoke();
		}
		public void LoadScene(int index)
		{
			OnBeforeSceneLoad?.Invoke();
			SceneManager.LoadScene(index);
			isMainMenu = index == 0;
			OnSceneLoad?.Invoke();
		}
	}
}
