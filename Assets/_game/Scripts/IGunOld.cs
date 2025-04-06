using System;
using Unity.Netcode;
using UnityEngine;

namespace _game.Scripts
{
    public interface IGunOld
    {
        public float Damage { get; set; }
        public int Ammo { get; set; }
        public int MaxAmmo { get; set; }
        public void TryShoot(bool keyDown, Transform point);
        public void ApplyRecoil();
    }
}
