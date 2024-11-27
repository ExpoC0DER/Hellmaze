using UnityEngine;

namespace _game.Scripts
{
    public class HitPoint : MonoBehaviour
    {
        [SerializeField] private float _duration = 1f;
        private void Start() { Destroy(gameObject, _duration); }
    }
}
