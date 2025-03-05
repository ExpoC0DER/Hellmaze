using UnityEngine;
using System.Collections.Generic;
using _game.Scripts;

public class ParticleProjectile : MonoBehaviour
{
	private ParticleSystem particleSystemm;
	[SerializeField] ProjectileGun projectileGun;
	private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
	[SerializeField] private string impactVFX_poolName;
	
	float damage = 0;
	PlayerStats source = null;
	int weaponIndex = 17;
	bool setupFromProjectile = false;
	
	//[SerializeField] private GunSettings gunSettings;

	void Start()
	{
		particleSystemm = GetComponent<ParticleSystem>();
	}
	
	public void SetupFromProjectile(PlayerStats source, float damage, int weaponIndex)
	{
		this.damage = damage;
		this.source = source;
		this.weaponIndex = weaponIndex;
		setupFromProjectile = true;
	}
	
	PlayerStats GetSource()
	{
		PlayerStats player = projectileGun?.Source;
		if(player == null)
		{
			player = source;
		}
		return player;
	}

	void OnParticleCollision(GameObject other)
	{
		
		if(other.TryGetComponent( out IDestructable dest))
		{
			
			if(other.transform != GetSource().transform)
			{
				if(setupFromProjectile)
				{
					dest.TakeDamage(damage, source, weaponIndex);
				}else
				{
					dest.TakeDamage(projectileGun.GetDamage(), projectileGun.Source, projectileGun.WeaponIndex);
				}
			}
		}
		
		if(!other.CompareTag("Player") && !other.CompareTag("Bot"))
		{
			// Get collision events
			int eventCount = particleSystemm.GetCollisionEvents(other, collisionEvents);

			ObjectPooler.main.SpawnPooledObject(impactVFX_poolName, collisionEvents[0].intersection, Quaternion.LookRotation(collisionEvents[0].normal), other.transform);
			
			/* for (int i = 0; i < eventCount; i++)
			{
				// Extract collision information
				Vector3 collisionPoint = collisionEvents[i].intersection;
				Vector3 collisionNormal = collisionEvents[i].normal;
				GameObject hitCollider = other;

				// Log or use the data
				Debug.Log($"Collision at: {collisionPoint}, Normal: {collisionNormal}, Hit object: {hitCollider.name}");

			} */
		}
		
		
	}
}
