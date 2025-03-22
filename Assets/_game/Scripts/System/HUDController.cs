using System;
using _game.Scripts.System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _game.Scripts
{
	public class HUDController : MonoBehaviour
	{
		//[SerializeField] private PlayerController _player;
		[SerializeField] private WeaponSlots _weapons;
		[SerializeField] private PlayerStats _playerStats;
		//[SerializeField] private GameManager _gameManager;
		[SerializeField] private TMP_Text _healthText;
		[SerializeField] private Gradient _healthColorGrad;
		[SerializeField] private Image _healthBar;
		[SerializeField] private TMP_Text _ammoText;
		[SerializeField] private TMP_Text _timerText;
		//[SerializeField] private TMP_Text _killCountText;
		[SerializeField] private Button _restartBtn;
		[SerializeField] private ResultScreen resultScreen;
		[SerializeField] private Image _crosshair;
		
		private int _seconds = 0;
		
	/* 	public static HUDController main {get; private set;}
		
		private void Awake()
		{
			if(main != null)
			{
				Destroy(this.gameObject);
			}
		} */
		

		void Start()
		{
			GameManager.Instance.OnTimerChange += SetTimerText;
		}
		
		private void SetHealthText(float health)
		{ 
			if(health < 0) health = 0;
			_healthText.text = Mathf.RoundToInt(health).ToString();
			_healthBar.fillAmount = Mathf.InverseLerp(0, 100, health);
			_healthText.color = _healthColorGrad.Evaluate(_healthBar.fillAmount);
			
		}

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
					GameManager.Instance.LoadScene(0);
				});
				_restartBtn.gameObject.SetActive(true);
				resultScreen.DisplayEndGameResults();
			}
		}

		void SetCrosshair(Image crosshairToSet)
		{
			_crosshair.sprite = crosshairToSet.sprite;
			_crosshair.color = crosshairToSet.color;
			_crosshair.rectTransform.sizeDelta = crosshairToSet.rectTransform.sizeDelta;
		}
		private void OnEnable()
		{
			_playerStats.OnHealthChange += SetHealthText;
			_weapons.OnAmmoChange += SetAmmoText;
			GameSettings.SetCrosshair += SetCrosshair;
			//PlayerStats.OnDeath += UpdateKillCount;
			
		}

		private void OnDisable()
		{
			_playerStats.OnHealthChange -= SetHealthText;
			_weapons.OnAmmoChange -= SetAmmoText;
			GameManager.Instance.OnTimerChange -= SetTimerText;
			GameSettings.SetCrosshair -= SetCrosshair;
			//PlayerStats.OnDeath -= UpdateKillCount;
		}
	}
}
