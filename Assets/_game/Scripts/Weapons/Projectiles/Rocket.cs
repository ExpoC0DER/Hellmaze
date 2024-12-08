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
		base._exploded = false;
		onTrigger = false;
		//Invoke("StartExplosionCor", 0.1f);
	}
	
	public override void Explode()
	{
		base.Explode();
	}
	
	//void StartExplosionCor() => StartCoroutine(ExplosiveRoutine());
	
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
		rb.MovePosition(rb.position + init_force * transform.forward * Time.fixedDeltaTime);
	}
	
	void Return() => ObjectPooler.main.ReturnObject(transform, PoolName);
	
	void OnCollisionEnter(Collision other)
	{
		if(other.transform == Source.transform && onTrigger == false) return;
		
		Explode();
		Invoke("Return", 1);
		
	}
	
	/* private void OnTriggerEnter(Collider other)
	{
		if(other.transform != Source.transform && (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Bot")) && !other.isTrigger && onTrigger)
		{
			//Debug.Log(" rocket hit : " + other.gameObject.name + " can it hit it: " + (other.transform != Source.transform) + " source is: " + Source.gameObject.name, other.gameObject);
			Explode();
			Invoke("Return", 1);
		}
	}  */
	
	void OnEnable()
	{
		StartCoroutine(ExplosiveRoutine());
	}
	
}
