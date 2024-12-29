using System.Collections;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using _game.Scripts;

public class Bomb : Explosive, IProjectile
{
	[HideInInspector] public float Damage { get; set; }
	[HideInInspector] public PlayerStats Source { get; set; }
	[HideInInspector] public string PoolName { get; set; }
	public int WeaponIndex { get; set; }

	[SerializeField] EventReference bounce_sfx;
	[SerializeField] Rigidbody rb;
	[SerializeField] float init_force = 10;
	[SerializeField] float detonation_time = 5;
	
	bool onTrigger = false;
	
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
		onTrigger = false;
		
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
		Invoke("ReturnToPool", 1);
	}
	
	IEnumerator ExplosiveRoutine()
	{
		yield return new WaitForSeconds(0.3f);
		onTrigger = true;
		yield return new WaitForSeconds(detonation_time);
		Explode();
		
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player") || other.CompareTag("Bot") && onTrigger)
		{
			Explode();
		}
	}
	
	void OnCollisionEnter(Collision other)
	{
		PlayBounceSound();
		//Debug.Log("explode on col: " + other.gameObject.tag, other.gameObject);
		if((other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Bot")) && onTrigger)
		{
			Explode();
		}
	}
	
	void PlayBounceSound()
	{
		FMODHelper.PlayNewInstance(bounce_sfx, transform);
	}
	
	void OnEnable()
	{
		rb.AddForce(init_force * transform.forward, ForceMode.Impulse);
		StartCoroutine(ExplosiveRoutine());
	}
	public void ReturnToPool()
	{
		ObjectPooler.main.ReturnObject(transform, PoolName);
	}
}
