using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using FMODUnity;
using FMOD.Studio;
using _game.Scripts;

public class Drone :  Explosive, IProjectile, IDestructable
{
	[HideInInspector] public float Damage { get; set; }
	[HideInInspector] public PlayerStats Source { get; set; }
	[HideInInspector] public string PoolName { get; set; }
	public int WeaponIndex { get; set; }
	public float Health {get; set;} = 75;
	public float MaxHealth {get; set;} = 75;
	public bool IsDead {get; set;} = false;

	[SerializeField] Rigidbody rb;
	[SerializeField] float move_speed = 10;
	[SerializeField] float detonation_time = 60;
	[SerializeField] SpriteRenderer profilePic;
	[SerializeField] ParticleSystem attackParticleProjectile;
	[SerializeField] ParticleProjectile particleProjectile;
	
	[SerializeField] EventReference drone_sfx;
	[SerializeField] EventReference drone_attack_sfx;
	private EventInstance droneSound;
	private EventInstance droneSound_attack;
	
	bool seeTarget = false;
	
	List<Transform> targets = new List<Transform>();
	Transform targetToAttack;
	Vector3 roamPos = Vector3.zero;
	float roamRadius = 5;
	float collisionCheckRadius = 2;
	
	float cd_visibility;
	
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
		Respawn();
		profilePic.sprite = source.playerProfilePic;
		particleProjectile.SetupFromProjectile(source, damage * 0.2f, weaponIndex);
		
		droneSound = FMODHelper.CreateNewInstance(drone_sfx, transform);
		droneSound.setParameterByName("Parameter 1", 1);
		droneSound.start();
		
		//Invoke("StartExplosionCor", 0.1f);
	}
	
	public override void Explode()
	{
		base.Explode();
		
		if (FMODHelper.InstanceIsPlaying(droneSound))
		{
			droneSound.setParameterByName("Parameter 1", 2);
			droneSound.release();
		}
		if (FMODHelper.InstanceIsPlaying(droneSound_attack))
		{
			droneSound_attack.setParameterByName("Parameter 1", 2);
			droneSound_attack.release();
		}
		
		Invoke("ReturnToPool", 1);
	}
	
	//void StartExplosionCor() => StartCoroutine(ExplosiveRoutine());
	
	IEnumerator ExplosiveRoutine()
	{
		yield return new WaitForSeconds(0.3f);
		onTrigger = true;
		yield return new WaitForSeconds(detonation_time);
		Explode();
		yield return new WaitForSeconds(1);
		ReturnToPool();
		
	}
	
	void FixedUpdate()
	{
		if(!gameObject.activeSelf || IsDead) return;
		
		if(cd_visibility >= 0)
		{
			cd_visibility -= Time.deltaTime;
			if(cd_visibility <= 0)
			{
				if(targets.Count > 0)
				{
					targetToAttack = GetClosestVisibleTarget();
				}
				cd_visibility = 0.5f;
			}
		}
		
		if(targetToAttack)
		{
			//rb.rotation = Quaternion.Slerp(rb.rotation, Quaternion.FromToRotation(transform.forward, (targetToAttack.position - transform.position).normalized), 50 * Time.deltaTime);
			rb.transform.LookAt(targetToAttack.position);
			rb.MovePosition(rb.position + 1.5f * move_speed * transform.forward * Time.fixedDeltaTime);
			if(!attackParticleProjectile.isPlaying)
			{
				attackParticleProjectile.Play();
				if(!FMODHelper.InstanceIsPlaying(droneSound_attack))
				{
					droneSound_attack = FMODHelper.CreateNewInstance(drone_attack_sfx, transform);
					droneSound_attack.setParameterByName("Parameter 1", 1);
					droneSound_attack.start();
				}
				
				//Debug.Log("A attack");
			}
			seeTarget = true;
			//Debug.Log("A see player");
		}else
		{
			//Debug.Log("R not see player");
			seeTarget = false;
			if(attackParticleProjectile.isPlaying)
			{
				attackParticleProjectile.Stop();
				
				if(FMODHelper.InstanceIsPlaying(droneSound_attack))
				{
					droneSound_attack.setParameterByName("Parameter 1", 2);
					droneSound_attack.release();
				}
				//Debug.Log("R no attack");
			}
			if(roamPos != Vector3.zero)
			{
				float distance = Vector3.Distance(roamPos, transform.position);
				//rb.rotation = Quaternion.Slerp(rb.rotation, Quaternion.FromToRotation(transform.forward, (roamPos - transform.position).normalized), 50 * Time.deltaTime);
				rb.transform.LookAt(roamPos);
				rb.MovePosition(rb.position + move_speed * transform.forward * Time.fixedDeltaTime);
				if(distance < collisionCheckRadius)
				{
					roamPos = Vector3.zero;
					//Debug.Log("R close roam elsewhere");
				}
				
			}else
			{
				roamPos = GetRandomPos();
				//Debug.Log("R get random pos");
			}
		}
		
	}
	
	Transform GetClosestVisibleTarget()
	{
		Transform target = null;
		float distance = 100;
		for (int i = 0; i < targets.Count; i++)
		{
			Transform pickedTarget = targets[i];
			if(!Physics.Raycast(transform.position, pickedTarget.position))
			{
				float distanceFromTarget = Vector3.Distance(pickedTarget.position, transform.position);
				if(distanceFromTarget < distance)
				{
					distance = distanceFromTarget;
					target = pickedTarget;
				}
			}
		}
		return target;
	}
	
	
	public Vector3 GetRandomPos()
	{
		Vector3 finalPos = transform.position;
		bool foundPos = false;
		int attempts = 50;
		while(!foundPos && attempts > 0)
		{
			finalPos = transform.position + Random.insideUnitSphere * roamRadius;
			if(!Physics.Raycast(transform.position, (finalPos - transform.position).normalized, out RaycastHit hit, Vector3.Distance(transform.position, finalPos) * 0.98f))
			{
				if(!Physics.CheckSphere(finalPos, collisionCheckRadius))
				{
					foundPos = true;
					LG_tools.DrawLine(transform.position, finalPos, Color.green);
					LG_tools.DrawRay(finalPos, Vector3.down * collisionCheckRadius, Color.green);
				}
				/*  else
				{
					LG_tools.DrawLine(transform.position, finalPos, Color.green);
					LG_tools.DrawRay(finalPos, Vector3.down * sphere_check_radius, Color.red);
				}
				Debug.Log("clear path"); */
			}
			else
			{
				Debug.Log("hit this " + hit.transform.gameObject.name, hit.transform.gameObject);
				LG_tools.DrawLine(transform.position, finalPos, Color.red);
				LG_tools.DrawRay(finalPos, Vector3.down * collisionCheckRadius, Color.red);
			}
			attempts--;
			
		}
		return finalPos;
	}
	
	void OnCollisionEnter(Collision other)
	{
		if(other.transform == Source.transform && onTrigger == false) return;
		
		if(other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Bot"))	Explode();
		
	}
	
	private void OnTriggerEnter(Collider other)
	{
		if(other.transform != Source.transform && (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Bot")))
		{
			//Debug.Log(" rocket hit : " + other.gameObject.name + " can it hit it: " + (other.transform != Source.transform) + " source is: " + Source.gameObject.name, other.gameObject);
			seeTarget = true;
			if(!targets.Contains(other.transform)) targets.Add(other.transform);
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if(targets.Contains(other.transform))
		{
			targets.Remove(other.transform);
		}
	}
	
	
	public void ReturnToPool()
	{
		ObjectPooler.main.ReturnObject(transform, PoolName);
	}
	
	void OnEnable()
	{
		StartCoroutine(ExplosiveRoutine());
	}

	public void Die()
	{
		base.Explode();
		IsDead = true;
	}

	public void TakeDamage(float amount, PlayerStats source, int weaponIndex)
	{
		if(IsDead) return;
		Health -= amount;
		if(Health <= 0)
		{
			Die();
			
		}
	}

	public void Respawn()
	{
		Health = MaxHealth;
		IsDead = false;
		onTrigger = false;
	}
}