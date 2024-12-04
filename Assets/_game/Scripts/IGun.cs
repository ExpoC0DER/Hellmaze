using UnityEngine;

namespace _game.Scripts
{
    public interface IGun
    {
        public float Damage { get; set; }
        public int Ammo { get; set; }
        public void TryShoot(bool keyDown, Transform point);
        public void ApplyRecoil();
    }
}
