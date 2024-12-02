using UnityEngine;
using System.Collections;

public class Rocket : Explosive, IProjectile
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
		onTrigger = false;
	}
	
	public override void Explode()
	{
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
	
	void FixedUpdate()
	{
		if(!gameObject.activeSelf) return;
		rb.AddForce(init_force * transform.forward, ForceMode.Force);
	}
	
	void Return() => ObjectPooler.main.ReturnObject(transform, PoolName);
	
	/* void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player") || other.CompareTag("Bot") && onTrigger)
		{
			Explode();
			Invoke("Return", 1);
		}
	} */
	
	void OnCollisionEnter(Collision other)
	{
		Explode();
		Invoke("Return", 1);
	}
	
	void OnEnable()
	{
		StartCoroutine(ExplosiveRoutine());
	}
	
	void OnDisable()
	{
		
	}
}
