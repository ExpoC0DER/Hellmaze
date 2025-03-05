using UnityEngine;
using FMODUnity;
using _game.Scripts;
public class StickyBomb : Explosive, IProjectile, IDestructable
{
	[HideInInspector] public float Damage { get; set; }
	[HideInInspector] public PlayerStats Source { get; set; }
	[HideInInspector] public string PoolName { get; set; }
	public int WeaponIndex { get; set; }
	
	public float Health { get; set; } = 40;
	public float MaxHealth { get; set; } = 40;
	public bool IsDead { get; set; } = false;

	[SerializeField] Rigidbody rb;
	[SerializeField] float init_force = 10;
	[SerializeField] GameObject laserTrigger;
	[SerializeField] EventReference setup_sound;
	bool setup = false;
	
	public void Initialize(PlayerStats source, float damage, int weaponIndex, string poolName)
	{
		this.Source = source;
		this.Damage = damage;
		this.PoolName = poolName;
		this.WeaponIndex = weaponIndex;
		base._weaponIndex = weaponIndex;
		base._damage = damage;
		base._source = source;
		base._exploded = false;
		setup = false;
		
		/* rb.AddForce(init_force * transform.forward, ForceMode.Impulse);
		Invoke("StartExplosionCor", 0.1f); */
	}
	
	//void StartExplosionCor() => StartCoroutine(ExplosiveRoutine());
	
	public override void Explode()
	{
		/* explosion_part.Play();
		
		Collider[] cols = Physics.OverlapSphere(transform.position, expl_radius);
		
		for (int i = 0; i < cols.Length; i++)
		{
			Transform obj = cols[i].transform;
			if(obj.TryGetComponent(out Rigidbody rb))
			{
				rb.AddExplosionForce(expl_force, transform.position, expl_radius);
				float distanceRatio = Mathf.InverseLerp(0, expl_radius, Vector3.Distance(transform.position, obj.transform.position));
				float damage = Mathf.Lerp(Damage, 0, distanceRatio);
				if(obj.TryGetComponent(out PlayerStats playerStats))
				{
					playerStats.ChangeHealth(-damage);
				}else if(obj.TryGetComponent(out EnemyAI enemyAI))
				{
					enemyAI.TakeDamage(damage, Source.position);
				}
			}else
			{
				continue;
			}
		} */
		base.Explode();
		laserTrigger.SetActive(false);
	}
	
	
	void OnCollisionEnter(Collision other)
	{
		if(!setup && (other.collider.tag != "Player" || other.collider.tag != "Bot"))
		{
			rb.isKinematic = true;
			transform.rotation = Quaternion.LookRotation(other.GetContact(0).normal);
			laserTrigger.SetActive(true);
			FMODHelper.PlayNewInstance(setup_sound, transform);
			setup = true;
		}
	}
	
	public void LaserTrigger(PlayerStats player)
	{
		player?.TakeDamage(_damage * 0.3f, Source, WeaponIndex);
		Explode();
		Invoke("ReturnToPool", 1);
	}
	
	void OnEnable()
	{
		rb.AddForce(init_force * transform.forward, ForceMode.Impulse);
	}

	public void TakeDamage(float amount, PlayerStats source, int weaponIndex)
	{
		if(Health > 0)
		{
			Health -= amount;
			if(Health <= 0)
			{
				Explode();
			}
		}
	}

	public void Die()
	{
		return;
	}

	public void ReturnToPool()
	{
		ObjectPooler.main.ReturnObject(transform, PoolName);
	}
	
	public void Respawn()
	{
		return;
	}
}
