using System;
using TMPro;
using UnityEngine;

namespace _game.Scripts
{
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private PlayerController _player;
        [SerializeField] private TMP_Text _healthText;
        [SerializeField] private TMP_Text _ammoText;

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

        private void OnEnable()
        {
            _player.OnHealthChange += SetHealthText;
            _player.OnAmmoChange += SetAmmoText;
        }

        private void OnDisable()
        {
            _player.OnHealthChange -= SetHealthText;
            _player.OnAmmoChange -= SetAmmoText;
        }
    }
}
