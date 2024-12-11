using UnityEngine;
using FMODUnity;
using _game.Scripts;
using System.Collections.Generic;

public class Explosive : MonoBehaviour
{
	[SerializeField] float expl_radius = 5;
	[SerializeField] float expl_force = 5;
	
	[SerializeField] protected float _damage;
	[SerializeField] protected int _weaponIndex;
	[SerializeField] protected GameObject visual_model;
	[SerializeField] EventReference _explosion_sfx;
	protected PlayerStats _source;
	protected bool _exploded = false;
	
	[SerializeField] ParticleSystem explosion_part;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	public virtual void Explode()
	{
		if(!_exploded)
		{
			_exploded = true;
		}else
		{
			return;
		}
		explosion_part.Play();
		visual_model.SetActive(false);
		FMODHelper.PlayNewInstance(_explosion_sfx, transform.position);
		Collider[] cols = Physics.OverlapSphere(transform.position, expl_radius);
		
		if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 0.5f, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
		{
			ObjectPooler.main.SpawnPooledObject("explosion_dec", hit.point, Quaternion.LookRotation(hit.normal), hit.transform);
		}
		else if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitt, 2, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
		{
			ObjectPooler.main.SpawnPooledObject("explosion_dec", hitt.point, Quaternion.LookRotation(hitt.normal), hitt.transform);
		}
		
		for (int i = 0; i < cols.Length; i++)
		{
			Transform obj = cols[i].transform;
			if(obj.TryGetComponent(out Rigidbody rb))
			{
				float distanceRatio = Mathf.InverseLerp(0, expl_radius, Vector3.Distance(transform.position, obj.transform.position));
				float force = Mathf.Lerp(expl_force, 0, distanceRatio);
				Vector3 direction = new Vector3(obj.transform.position.x, obj.transform.position.y + 3, obj.transform.position.z) - transform.position;
				/* if(obj.TryGetComponent(out EnemyAI enemyAI))
				{
					enemyAI.SimulatePhysics();
				}else if(obj.TryGetComponent(out PlayerController player))
				{
					player.SimulatePhysics(direction, force);
				} */
				//rb.AddExplosionForce(expl_force, transform.position, expl_radius);
				
				rb.AddForce( direction * force, ForceMode.Impulse);
			
				if(obj.transform.TryGetComponent(out IDestructable destructable))
				{
					float damage = Mathf.Lerp(_damage, 0, distanceRatio);
					destructable.TakeDamage(damage, _source, _weaponIndex);
				}
				
				/* if(obj.TryGetComponent(out PlayerStats playerStats))
				{
					
					playerStats.TakeDamage
					//Debug.Log("explosive damage " + -damage);
				} *//* else if(obj.TryGetComponent(out EnemyAI enemyAI))
				{
					enemyAI.TakeDamage(damage, _source.position);
					Debug.Log("explosive damage bot " + damage);
				} */
			}else
			{
				continue;
			}
		}
	}
	
}
