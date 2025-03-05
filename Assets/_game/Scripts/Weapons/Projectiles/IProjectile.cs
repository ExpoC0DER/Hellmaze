using UnityEngine;

public interface IProjectile
{
	public float Damage { get; set; }
	public PlayerStats Source { get; set; }
	public string PoolName { get; set; }
	public int WeaponIndex { get; set; }
	public void Initialize(PlayerStats source, float damage, int weaponIndex, string poolName);
	public void ReturnToPool();
}
