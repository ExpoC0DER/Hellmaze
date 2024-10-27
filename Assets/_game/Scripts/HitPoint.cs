using UnityEngine;

namespace _game.Scripts
{
    public class HitPoint : MonoBehaviour
    {
        private void Start() { Destroy(gameObject, 1f); }
    }
}
