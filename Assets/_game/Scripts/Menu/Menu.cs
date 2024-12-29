using _game.Scripts;
using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;


public class Menu : MonoBehaviour
{
	[SerializeField] GameObject[] tabs;
	[SerializeField] GameObject[] buttons;
	[SerializeField] GameObject menuObject, pausemenu_visual, mainmenu_visual;
	[SerializeField] EventReference[] button_sfx;
	
	[field: SerializeField] public MapSettings mapSettings { get; private set; }
	[field: SerializeField] public Settings clientSettings { get; private set; }
	
	public bool isPaused {get; private set; } = false;
	public static Menu main {get; private set;}
	
	void Awake()
	{
		if(main != null)
		{
			Destroy(this.gameObject);
		}else
		{
			main = this;
		}
		DontDestroyOnLoad(transform.parent);
	}
	
	void Start()
	{
		GameManager.main.OnSceneLoad += Reload;
		Reload();
	}
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			if(!GameManager.main.isMainMenu) PauseMenu();
		}
	}
	
	void SetButtons()
	{
		bool activate = GameManager.main.isMainMenu;
		for (int i = 0; i < 4; i++)
		{
			if(i>=2) activate = !GameManager.main.isMainMenu;
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
	
	public void PlayUISound(int index)
	{
		FMODHelper.PlayNewInstance(button_sfx[index], Camera.main!.transform);
	}
	
	public void Quit(bool toMenu)
	{
		PauseMenu();
		if(toMenu) GameManager.main.LoadScene(0);
		else Application.Quit();
	}
	
	public void PauseMenu()
	{
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
		SetTab(-1);
		menuObject.SetActive(isPaused);
	}
	
	void ForcePauseMenu(bool forceON)
	{
		isPaused = !forceON;
		PauseMenu();
	}
	
	void SetMainMenu_Functionality()
	{	
		SetTab(-1);
		Time.timeScale = 1f;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}
	
	public void StartGame()
	{
		GameManager.main.LoadScene(1);
	}
	
	void Reload()
	{
		mainmenu_visual.SetActive(GameManager.main.isMainMenu);
		pausemenu_visual.SetActive(!GameManager.main.isMainMenu);
		SetButtons();
		menuObject.SetActive(GameManager.main.isMainMenu);
		if(GameManager.main.isMainMenu) SetMainMenu_Functionality();
	}
	
	void OnDisable()
	{
		GameManager.main.OnSceneLoad -= Reload;
	}
}
