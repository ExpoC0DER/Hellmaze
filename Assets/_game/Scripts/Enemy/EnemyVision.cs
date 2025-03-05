using System.Collections;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(SphereCollider))]
public class EnemyVision : MonoBehaviour
{
	//add events when health kit or weapon is seen (when low ammo / health go get it)
	[SerializeField] EnemyAI enemyAI;
	[SerializeField] float fovAngle = 90;
	[SerializeField] float reactionTime = 0.3f;
	[SerializeField] public Transform target { get; private set; }
	[SerializeField] List<Transform> targets = new List<Transform>();
	[SerializeField] List<Transform> healthKits = new List<Transform>();
	[SerializeField] List<Transform> weaponPickups = new List<Transform>();
	[SerializeField] List<Transform> seenHealthKits = new List<Transform>();
	[SerializeField] List<Transform> seenWeaponPickups = new List<Transform>();
	public bool seePlayer {get; private set;}
	SphereCollider col;
	float detectionRadius;
	float checkInterval = 0.3f;
	float checkInterval_cd = 0;
	float playerCheck_cd = 0;
	float removeDestroyed_cd = 0;
	Transform lastSeenTarget;
	bool spottedNewTarget = true;
	
	
	 void Start()
	{
		seePlayer = false;
		col = GetComponent<SphereCollider>();
		detectionRadius = col.radius;
	}
	
	void Update()
	{
		RemoveDestroyedObjects();
		//CheckVisibleObjects();
		LookForPlayer();
	}

	// add check for players and pickups in range
	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player") || other.CompareTag("Bot")) 
		{
			if(!targets.Contains(other.transform))
			{
				targets.Add(other.transform);
			} 
		}
		/* else if(other.TryGetComponent(out HealthKit healthKit))
		{
			if(!healthKits.Contains(other.transform)) healthKits.Add(other.transform);
		}
		else if(other.TryGetComponent(out WeaponPickup weapon))
		{
			if(!weaponPickups.Contains(other.transform)) weaponPickups.Add(other.transform);
		} */
	}
	
	void OnTriggerExit(Collider other)
	{
		if(other.CompareTag("Player") || other.CompareTag("Bot"))
		{
			if(targets.Contains(other.transform))
			{
				targets.Remove(other.transform);
			} 
		}
		/* else if(other.TryGetComponent(out HealthKit healthKit))
		{
			if(healthKits.Contains(other.transform)) healthKits.Add(other.transform);
		}
		else if(other.TryGetComponent(out WeaponPickup weapon))
		{
			if(weaponPickups.Contains(other.transform)) weaponPickups.Add(other.transform);
		} */
	}
	
	//Check objects in proximity array to see if they are in Enemy FOV and are not behind wall, then prefer PLAYER as target
	public bool CheckPlayerVisibility(out Transform target)
	{
		Transform visibleObj = null;
		float closestDistance = detectionRadius;
		for (int i = 0; i < targets.Count; i++)
		{
			Transform obj = targets[i];
			if(obj == null) continue;
			Vector3 directionToTarget = (obj.position - transform.position).normalized;
			float distanceToTarget = Vector3.Distance(transform.position, obj.position);
			//player is not behind wall
			//Debug.DrawRay(transform.position, directionToTarget * distanceToTarget, Color.yellow, 60);
			if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget - 0.1f))
			{
				//player is in fov 
				if (Vector3.Angle(transform.forward, directionToTarget) < fovAngle)
				{
					//get the closest player
					if(distanceToTarget < closestDistance)
					{
						closestDistance = distanceToTarget;
						visibleObj = obj;
					}
				}
			}
		}
		target = visibleObj;
		//Debug.Log("Target seen = " + (target != null), target );
		return target;
	}
	
	public bool CheckClosestHealthKit(out Transform position)
	{
		Transform visibleObj = null;
		float closestDistance = detectionRadius;
		for (int i = 0; i < seenHealthKits.Count; i++)
		{
			Transform obj = seenHealthKits[i];
			if(obj == null) continue;
			float distanceToTarget = Vector3.Distance(transform.position, obj.position);
			if(distanceToTarget < closestDistance)
			{
				closestDistance = distanceToTarget;
				visibleObj = obj;
			}
		}
		position = visibleObj;
		return position;
	}
	
	public bool CheckClosestWeaponPickups(out Transform position)
	{
		Transform visibleObj = null;
		float closestDistance = detectionRadius;
		for (int i = 0; i < seenWeaponPickups.Count; i++)
		{
			Transform obj = seenWeaponPickups[i];
			if(obj == null) continue;
			float distanceToTarget = Vector3.Distance(transform.position, obj.position);
			if(distanceToTarget < closestDistance)
			{
				closestDistance = distanceToTarget;
				visibleObj = obj;
			}
		}
		position = visibleObj;
		return position;
	}
	
	void LookForPlayer()
	{
		if(playerCheck_cd >= 0)
		{
			playerCheck_cd -= Time.deltaTime;
			if(playerCheck_cd <= 0)
			{
				if(CheckPlayerVisibility(out Transform target))
				{
					if(spottedNewTarget)
					{
						lastSeenTarget = target;
						enemyAI.SetTarget(target);
						spottedNewTarget = false;
					}
				}else
				{
					//Debug.Log("player not visible");
					if(!spottedNewTarget)
					{
						if(Physics.Raycast(new Vector3(transform.position.x, transform.position.y +0.9f, transform.position.z), (lastSeenTarget.position - transform.position).normalized, out RaycastHit hit))
						{
							
							if(hit.transform != lastSeenTarget)
							{
								//Debug.Log("player not in room");
								enemyAI.SetTarget(null);
								spottedNewTarget = true;
							}
						}
					}
					
				}
				playerCheck_cd = reactionTime;
			}
		}
	}
	
	void CheckVisibleObjects()
	{
		if(checkInterval_cd >= 0)
		{
			checkInterval_cd -= Time.deltaTime;
			if(checkInterval_cd <= 0)
			{
				for (int i = 0; i < weaponPickups.Count; i++)
				{
					Transform obj = weaponPickups[i];
					if(obj == null) continue;
					Vector3 directionToTarget = (obj.position - transform.position).normalized;
					float distanceToTarget = Vector3.Distance(transform.position, obj.position);
					//player is not behind wall
					if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget - 0.1f))
					{
						//player is in fov 
						if (Vector3.Angle(transform.forward, directionToTarget) < fovAngle / 90)
						{
							if(!seenWeaponPickups.Contains(obj))seenWeaponPickups.Add(obj);
							enemyAI.SeenWeaponPickup(obj);
						}
					}
				}
				for (int i = 0; i < healthKits.Count; i++)
				{
					Transform obj = healthKits[i];
					if(obj == null) continue;
					Vector3 directionToTarget = (obj.position - transform.position).normalized;
					float distanceToTarget = Vector3.Distance(transform.position, obj.position);
					//player is not behind wall
					if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget - 0.1f))
					{
						//player is in fov 
						if (Vector3.Angle(transform.forward, directionToTarget) < fovAngle / 90)
						{
							if(!seenHealthKits.Contains(obj))seenHealthKits.Add(obj);
							enemyAI.SeenHealthKit(obj);
						}
					}
				}
				checkInterval_cd = checkInterval;
			}
		}
		
		
	}
	
	void RemoveDestroyedObjects()
	{
		if(removeDestroyed_cd >= 0)
		{
			removeDestroyed_cd -= Time.deltaTime;
			if(removeDestroyed_cd <= 0)
			{
				for (int i = 0; i < targets.Count; i++)
				{
					if(targets[i] == null)
					{
						targets.RemoveAt(i);
					}
				}
				for (int i = 0; i < weaponPickups.Count; i++)
				{
					if(weaponPickups[i] == null)
					{
						weaponPickups.RemoveAt(i);
					}
				}
				for (int i = 0; i < healthKits.Count; i++)
				{
					if(healthKits[i] == null)
					{
						healthKits.RemoveAt(i);
					}
				}
				removeDestroyed_cd = checkInterval;
			}
		}
		
	}
	
}
