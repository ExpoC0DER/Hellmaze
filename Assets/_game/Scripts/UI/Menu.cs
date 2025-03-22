using System.Collections;
using _game.Scripts.System;
using FMODUnity;
using UnityEngine;

namespace _game.Scripts.UI
{
    public class Menu : MonoBehaviour
    {
        private enum MenuScene
        {
            Menu,
            Game
        }

        [SerializeField] private MenuScene _menuScene;
        [SerializeField] GameObject[] tabs;
        [SerializeField] GameObject[] buttons;
        [SerializeField] GameObject menuObject, pausemenu_visual, mainmenu_visual;
        [SerializeField] EventReference[] button_sfx;
    
        private bool _isPaused;

        private void Start()
        {
            GameManager.Instance.OnSceneLoad += Reload;
            Reload();
        }

        private void OnEnable() { GameManager.Instance.playerControlls.Player.Pause.performed += x => PauseInput(); }

        void OnDisable()
        {
            // TODO: redo subscribing as you cannot unsubscribe anonymous delegate
            // GameManager.Instance.playerControlls.Player.Pause.performed -= x => PauseInput();
        }

        void PauseInput()
        {
            if (_menuScene == MenuScene.Game || !GameManager.Instance.GameEnded) PauseMenu();
        }

        void SetButtons()
        {
            bool activate = _menuScene == MenuScene.Menu;
            for(int i = 0; i < 4; i++)
            {
                if (i >= 2) activate = _menuScene != MenuScene.Menu;
                buttons[i].SetActive(activate);
            }
        }

        public void SetTab(int index)
        {
            for(int i = 0; i < tabs.Length; i++)
            {
                tabs[i].SetActive(i == index);
            }
        }

        public void PlayUISound(int index) { FMODHelper.PlayNewInstance(button_sfx[index], Camera.main!.transform); }

        public void Quit(bool toMenu)
        {
            PauseMenu();
            if (toMenu) GameManager.Instance.LoadScene(0);
            else Application.Quit();
        }

        public void PauseMenu()
        {
            _isPaused = !_isPaused;
            GameManager.Instance.SetGamePaused(_isPaused);
            SetTab(-1);
            menuObject.SetActive(_isPaused);
        }

        void ForcePauseMenu(bool forceON)
        {
            _isPaused = !forceON;
            PauseMenu();
        }

        void SetMainMenu_Functionality()
        {
            SetTab(-1);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void StartGame() { GameManager.Instance.LoadScene("Maze"); }

        void Reload() { StartCoroutine(ReloadMenu()); }

        IEnumerator ReloadMenu()
        {
            yield return new WaitUntil(() => !GameManager.Instance.loadingScene);
            Debug.Log("reloaded menu and main menu is " + _menuScene);

            mainmenu_visual.SetActive(_menuScene == MenuScene.Menu);
            pausemenu_visual.SetActive(_menuScene == MenuScene.Game);
            SetButtons();
            menuObject.SetActive(_menuScene == MenuScene.Menu);
            if (_menuScene == MenuScene.Menu) SetMainMenu_Functionality();

        }

        void OnDestroy() { GameManager.Instance.OnSceneLoad -= Reload; }
    }
}
