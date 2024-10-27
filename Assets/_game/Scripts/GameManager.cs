using System;
using Unity.VisualScripting;
using UnityEngine;

namespace _game.Scripts
{
    public class GameManager : MonoBehaviour
    {
        private float _timer = 300f;

        public event Action<float> OnTimerChange;

        private void Update()
        {
            _timer -= Time.deltaTime;
            OnTimerChange?.Invoke(_timer);
        }
    }
}
