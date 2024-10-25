using UnityEngine;

namespace _game.Scripts
{
    [CreateAssetMenu(fileName = "GunSettings", menuName = "Scriptable Objects/GunSettings")]
    public class GunSettings : ScriptableObject
    {
        [field: SerializeField] public float Damage { get; private set; }
        [field: SerializeField] public float FiringSpeed { get; private set; }
        [field: SerializeField] public float Spread { get; private set; }
    }
}
