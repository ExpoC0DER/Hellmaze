using UnityEngine;
using FMODUnity;
using _game.Scripts;
using System.Collections.Generic;

public class Explosive : MonoBehaviour
{
	[SerializeField] float expl_radius = 5;
	[SerializeField] float expl_force = 5;
	[SerializeField] protected string explosionDec_poolName = "explosion_dec";
	
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
			ObjectPooler.main.SpawnPooledObject(explosionDec_poolName, hit.point, Quaternion.LookRotation(hit.normal), hit.transform);
		}
		else if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitt, 2, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
		{
			ObjectPooler.main.SpawnPooledObject(explosionDec_poolName, hitt.point, Quaternion.LookRotation(hitt.normal), hitt.transform);
		}
		
		for (int i = 0; i < cols.Length; i++)
		{
			Transform obj = cols[i].transform;
			Collider _col = cols[i];
			/*
			if(Physics.Raycast(transform.position, (_col.transform.position - transform.position).normalized, out RaycastHit hitInfo))
			{
				if(hitInfo.transform != obj)
				{
					Debug.Log("explosive didnt hit same object "+ hitInfo.transform.name + " aiming at " + obj.transform.name, hitInfo.transform);
					LG_tools.DrawPoint(transform.position,60, Color.green);
					LG_tools.DrawPoint(_col.transform.position,60, Color.red);
					LG_tools.DrawLine(transform.position, hitInfo.point);
					//continue;
				}
			}else
			{
				LG_tools.DrawPoint(transform.position,60, Color.green);
				LG_tools.DrawPoint(_col.transform.position,60, Color.red);
				LG_tools.DrawLine(transform.position, hitInfo.point, Color.yellow);
			}*/
			
			float distance = Vector3.Distance(transform.position, _col.ClosestPoint(transform.position));
			float distanceRatio = Mathf.InverseLerp(0, expl_radius, distance);
			
			if(obj.TryGetComponent(out Rigidbody rb))
			{
				
				float force = Mathf.Lerp(expl_force, 0, distanceRatio);
				
				if(expl_force != 0)
				{
					Vector3 direction = new Vector3(obj.transform.position.x, obj.transform.position.y + 3, obj.transform.position.z) - transform.position;
					if(obj.TryGetComponent(out EnemyAI enemyAI))
					{
						enemyAI.SimulateImpulsePhysics(direction * force);
					}else if(obj.TryGetComponent(out PlayerController player))
					{
						//player.SimulatePhysics(direction, force);
						player.ApplyForce(direction * force);
					}else
					{
						rb.AddForce( direction * force, ForceMode.Impulse);
					}
					//rb.AddExplosionForce(expl_force, transform.position, expl_radius);
					
				}
			}
			
			if(obj.transform.TryGetComponent(out IDestructable destructable))
			{
				float damage;
				if(distance <= 1) damage = _damage;
				else damage = Mathf.Lerp(_damage, 0, distanceRatio);
				destructable.TakeDamage(damage, _source, _weaponIndex);
			}
		}
	}
}
