using System;
using _game.Scripts.Controllers_Managers;
using _game.Scripts.Definitions;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace _game.Scripts.UI
{
    public class PlayerHUDController : MonoBehaviour
    {
        [SerializeField] private Scoreboard _scoreboard;
        [SerializeField] private KillFeed _killFeed;
        [SerializeField] private TMP_Text _healthText;
        [SerializeField] private Gradient _healthColorGrad;
        [SerializeField] private Image _healthBar;
        [SerializeField] private TMP_Text _ammoText;
        [SerializeField] private TMP_Text _timerText;

        [SerializeField] private Button _restartBtn;
        [SerializeField] private ResultScreen resultScreen;
        [SerializeField] private Image _crosshair;

        private IGunOld _currentGun;

        private int _seconds;
        private int _previousAmmo;

        private bool _showMenu;

        private void Start()
        {
            _restartBtn.onClick.AddListener(OnClickRestart);
            
            // Enable scoreboard as for some fucking reason it disables itself by default
            _scoreboard.enabled = true;
        }

        private void Update()
        {
            if (Keyboard.current[Key.Escape].wasPressedThisFrame)
            {
                ShowMenu(!_showMenu);
            }
            
            ShowResults(Keyboard.current[Key.Tab].isPressed);
        }

        private void ShowMenu(bool value)
        {
            _showMenu = value;
            if (value)
            {
                _restartBtn.gameObject.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                _restartBtn.gameObject.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void ShowResults(bool value)
        {
            _scoreboard.ShowScoreboard(value);
        }

        private void OnClickRestart() { NetworkManager.Singleton.Shutdown(); }

        public void SetCurrentGun(IGunOld newGun)
        {
            _currentGun = newGun;
            SetAmmoText(_currentGun.Ammo);
        }

        public void SetHealthText(float newHealth)
        {
            newHealth = Mathf.Max(newHealth, 0); // Clamp health to 0

            _healthText.text = Mathf.RoundToInt(newHealth).ToString();
            _healthBar.fillAmount = Mathf.InverseLerp(0, 100, newHealth);
            _healthText.color = _healthColorGrad.Evaluate(_healthBar.fillAmount);

        }

        private void SetAmmoText(int newAmmo)
        {
            if (_previousAmmo == newAmmo)
                return;

            _previousAmmo = newAmmo;
            _ammoText.text = newAmmo == -1 ? "∞" : newAmmo.ToString();
        }

        public void SetTimerText(float timer)
        {
            int newSeconds = Mathf.CeilToInt(timer);
            if (newSeconds == _seconds)
                return;

            _seconds = newSeconds;
            int minutes = newSeconds / 60;
            int seconds = newSeconds % 60;

            _timerText.text = $"{minutes:00}:{seconds:00}";

            if (_seconds <= 0)
            {
                _restartBtn.onClick.AddListener(() =>
                {
                    //GameManager.Instance.LoadScene(0);
                });
                _restartBtn.gameObject.SetActive(true);
                resultScreen.DisplayEndGameResults();
            }
        }

        public void SetCrosshair(Image newCrosshairImage)
        {
            _crosshair.sprite = newCrosshairImage.sprite;
            _crosshair.color = newCrosshairImage.color;
            _crosshair.rectTransform.sizeDelta = newCrosshairImage.rectTransform.sizeDelta;
        }

        public void DisplayKill(string killerPlayer, string deadPlayer, int weaponId) { _killFeed.AddKillLog(killerPlayer, weaponId, deadPlayer); }

        private void OnEnable()
        {
            //GameManager.Instance.OnTimerChange += SetTimerText;
            PlayerEvents.OnPlayerShot += SetAmmoText;
            GameSettings.SetCrosshair += SetCrosshair;
        }

        private void OnDisable()
        {
            //GameManager.Instance.OnTimerChange -= SetTimerText;
            PlayerEvents.OnPlayerShot -= SetAmmoText;
            GameSettings.SetCrosshair -= SetCrosshair;
        }
    }
}
