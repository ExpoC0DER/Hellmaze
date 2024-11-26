using System.Collections;
using UnityEngine;

public class Bomb : Explosive, IProjectile
{
	[HideInInspector] public float Damage { get; set; }
	[HideInInspector] public Transform Source { get; set; }
	[HideInInspector] public string PoolName { get; set; }

	[SerializeField] Rigidbody rb;
	[SerializeField] float init_force = 10;
	[SerializeField] float detonation_time = 5;
	[SerializeField] float expl_radius = 5;
	[SerializeField] float expl_force = 5;
	
	[SerializeField] ParticleSystem explosion_part;
	
	public void Initialize(Transform source, float damage, string poolName)
	{
		this.Source = source;
		this.Damage = damage;
		this.PoolName = poolName;
		base._damage = damage;
		base._source = source;
	}
	
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
		Invoke("Return", 1f);
	}
	
	void Return() => ObjectPooler.main.ReturnObject(transform, PoolName);
	
	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player") || other.CompareTag("Bot"))
		{
			Explode();
		}
	}
	
	void OnEnable()
	{
		rb.AddForce(init_force * transform.forward, ForceMode.Impulse);
		Invoke("Explode", detonation_time);
	}
	
	void OnDisable()
	{
		
	}
}
