using System;
using Unity.VisualScripting;
using UnityEngine;

namespace _game.Scripts
{
    public class GameManager : MonoBehaviour
    {
        private float _timer = 180f;

        public event Action<float> OnTimerChange;

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
    }
}
