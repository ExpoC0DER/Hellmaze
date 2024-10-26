using DG.Tweening;
using UnityEngine;

namespace _game.Scripts
{
    [CreateAssetMenu(fileName = "GunSettings", menuName = "Scriptable Objects/GunSettings")]
    public class GunSettings : ScriptableObject
    {
        public enum FiringModeSetting
        {
            Manual,
            Automatic
        }
        
        [field: SerializeField] public float Damage { get; private set; }
        [field: SerializeField] public float FiringSpeed { get; private set; }
        [field: SerializeField] public float Spread { get; private set; }
        [field: SerializeField] public FiringModeSetting FiringMode { get; private set; }

        [Header("Position Recoil Settings")]
        [field: SerializeField] public Vector3 RecoilKickback { get; private set; } = new Vector3(0, 0, -0.2f); // backward direction
        [field: SerializeField] public float RecoilDuration { get; private set; } = 0.1f; // duration of recoil
        [field: SerializeField] public float RecoilReturnDuration { get; private set; } = 0.15f; // time to return to original position
        [field: SerializeField] public Ease RecoilEase { get; private set; } = Ease.OutQuad; // ease type for recoil
        [field: SerializeField] public Ease ReturnEase { get; private set; } = Ease.InQuad; // ease type for return

        [Header("Rotation Recoil Settings")]
        [field: SerializeField] public Vector3 RecoilRotation { get; private set; } = new Vector3(-5, 2, 0); // rotational recoil (pitch, yaw, roll)
        [field: SerializeField] public float RotationDuration { get; private set; } = 0.1f; // duration of rotation recoil
        [field: SerializeField] public float RotationReturnDuration { get; private set; } = 0.15f; // time to return to original rotation

    }
}
