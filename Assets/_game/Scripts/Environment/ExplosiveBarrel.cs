using UnityEngine;

public class ExplosiveBarrel : Explosive, IDestructable
{
	public float Health { get; set; } = 50;
	public float MaxHealth { get; set; } = 50;
	public bool IsDead  { get; set; } = false;

	public void Die()
	{
		base.Explode();
		IsDead = true;
	}

	public void Respawn()
	{
		base.visual_model.SetActive(true);
		Health = MaxHealth;
		IsDead = false;
	}

	public void TakeDamage(float amount, PlayerStats source, int weaponIndex)
	{
		if(IsDead) return;
		Health -= amount;
		if(Health <= 0)
		{
			base._source = source;
			Die();
			
		}
	}
}
