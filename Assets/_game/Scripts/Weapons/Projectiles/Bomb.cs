using System.Collections;
using UnityEngine;

public class Bomb : Explosive, IProjectile
{
	[HideInInspector] public float Damage { get; set; }
	[HideInInspector] public PlayerStats Source { get; set; }
	[HideInInspector] public string PoolName { get; set; }
	public int WeaponIndex { get; set; }

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
	}
	
	IEnumerator ExplosiveRoutine()
	{
		yield return new WaitForSeconds(0.3f);
		onTrigger = true;
		yield return new WaitForSeconds(detonation_time);
		Explode();
		yield return new WaitForSeconds(1);
		Return();
		
	}
	
	void Return() => ObjectPooler.main.ReturnObject(transform, PoolName);
	
	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player") || other.CompareTag("Bot") && onTrigger)
		{
			Explode();
			Invoke("Return", 1);
		}
	}
	
	void OnEnable()
	{
		rb.AddForce(init_force * transform.forward, ForceMode.Impulse);
		StartCoroutine(ExplosiveRoutine());
	}
	
	void OnDisable()
	{
		
	}
}
