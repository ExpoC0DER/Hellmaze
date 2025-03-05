using UnityEngine;

public interface IDestructable
{
	public void TakeDamage(float amount, PlayerStats source, int weaponIndex);
	public void Die();
	public void Respawn();
	public float Health { get; set; }
	public float MaxHealth { get; set; }
	public bool IsDead { get; set; }
}
