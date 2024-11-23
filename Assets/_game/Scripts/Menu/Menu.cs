using _game.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

public class Menu : MonoBehaviour
{
	[SerializeField] GameManager gameManager;
	[SerializeField] GameObject[] tabs;
	[SerializeField] GameObject[] buttons;
	[SerializeField] GameObject menuObject, pausemenu_visual, mainmenu_visual;
	
	public bool isPaused {get; private set; } = false;
	
	void Awake()
	{
		DontDestroyOnLoad(transform.parent);
	}
	
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			PauseGame();
		}
	}
	
	void SetButtons()
	{
		bool activate = gameManager.isMainMenu;
		for (int i = 0; i < 4; i++)
		{
			if(i>=2) activate = !gameManager.isMainMenu;
			buttons[i].SetActive(activate);
		}
	}
	
	public void SetTab(int index)
	{
		for (int i = 0; i < tabs.Length; i++)
		{
			tabs[i].SetActive(i == index);
		}
	}
	
	public void Quit(bool toMenu)
	{
		PauseGame(false);
		if(toMenu) gameManager.LoadScene(0);
		else Application.Quit();
	}
	
	public void PauseGame(bool lockedInMenu = true)
	{
		if(gameManager.isMainMenu && lockedInMenu) return;
		isPaused = !isPaused;
		if(isPaused)
		{
			Time.timeScale = 0f;
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}else
		{
			Time.timeScale = 1f;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		SetTab(999);
		menuObject.SetActive(isPaused);
	}
	
	public void StartGame()
	{
		PauseGame(false);
		gameManager.LoadScene(1);
	}
	
	void Reload()
	{
		mainmenu_visual.SetActive(gameManager.isMainMenu);
		pausemenu_visual.SetActive(!gameManager.isMainMenu);
		SetButtons();
	}
	
	void Preload()
	{
		PauseGame(false);
	}
	
	void OnEnable()
	{
		gameManager.OnSceneLoad += Reload;
		gameManager.OnBeforeSceneLoad += Preload;
	}
	void OnDisable()
	{
		gameManager.OnSceneLoad -= Reload;
		gameManager.OnBeforeSceneLoad -= Preload;
	}
}
