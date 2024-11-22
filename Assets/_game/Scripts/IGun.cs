namespace _game.Scripts
{
	public interface IGun
	{
		public int Ammo { get; set; }
		public int MaxAmmo { get; set; }
		public void PickAmmo(int amount);
		public void Shoot(bool keyDown, out bool succesShot);
	}
}
