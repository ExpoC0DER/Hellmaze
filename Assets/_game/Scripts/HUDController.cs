using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _game.Scripts
{
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private PlayerController _player;
        [SerializeField] private WeaponSlots _weapons;
        [SerializeField] private PlayerStats _playerStats;
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private TMP_Text _healthText;
        [SerializeField] private TMP_Text _ammoText;
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private TMP_Text _killCountText;
        [SerializeField] private Button _restartBtn;

        private int _seconds = 0;

        private void SetHealthText(float health) { _healthText.text = Mathf.RoundToInt(health).ToString(); }

        private void SetAmmoText(int ammo)
        {
            if (ammo == -1)
            {
                if (!_ammoText.text.Equals("∞"))
                    _ammoText.text = "∞";
            }
            else
            {
                _ammoText.text = ammo.ToString();
            }
        }

        private void SetTimerText(float timer)
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
                    SceneManager.LoadScene(0);
                });
                _restartBtn.gameObject.SetActive(true);
            }
        }

        private int _killCount = 0;

        private void UpdateKillCount()
        {
            _killCount++;
            _killCountText.text = $"Kills: {_killCount}";
        }

        private void OnEnable()
        {
            _playerStats.OnHealthChange += SetHealthText;
            _weapons.OnAmmoChange += SetAmmoText;
            _gameManager.OnTimerChange += SetTimerText;
            EnemyAI.OnDeath += UpdateKillCount;
        }

        private void OnDisable()
        {
            _playerStats.OnHealthChange -= SetHealthText;
            _weapons.OnAmmoChange -= SetAmmoText;
            _gameManager.OnTimerChange -= SetTimerText;
            EnemyAI.OnDeath -= UpdateKillCount;
        }
    }
}
