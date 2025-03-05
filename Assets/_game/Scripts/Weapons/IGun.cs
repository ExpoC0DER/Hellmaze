using UnityEngine;

namespace _game.Scripts
{
	public interface IGun
	{
		public int WeaponIndex { get; set; }
		public int Ammo { get; set; }
		public int MaxAmmo { get; set; }
		public float BotInaccuracy { get; set; }
		public float DamageMultiplier { get; set; }
		public PlayerStats Source { get; set; }
		public void PickAmmo(int amount);
		public void Shoot(bool keyDown, Transform target, out bool succesShot);
		public void StopShooting();
	}
}
