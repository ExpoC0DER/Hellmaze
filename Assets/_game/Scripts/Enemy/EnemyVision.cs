using System.Collections;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class EnemyVision : MonoBehaviour
{
	[SerializeField] List<Transform> objectsOfInterest;
	[SerializeField] float fovAngle = 90;
	Transform target;
	public bool seeTarget {get; private set;}
	
	void Start()
	{
		seeTarget = false;
		StartCoroutine(RemoveDestroyedObjects());
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player")) // add check for pickups
		{
			objectsOfInterest.Add(other.transform);
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if(other.CompareTag("Player")) // add check for pickups
		{
			objectsOfInterest.Remove(other.transform);
		}
	}
	
	//Check objects in proximity array to see if they are in Enemy FOV and are not behind wall, then prefer PLAYER as target
	private bool CheckVisibility(out Transform target)
	{
		Transform visibleObj = null;
		for (int i = 0; i < objectsOfInterest.Count; i++)
		{
			Transform obj = objectsOfInterest[i];
			Vector3 directionToTarget = (obj.position - transform.position).normalized;
			if (Vector3.Angle(transform.forward, directionToTarget) < fovAngle / 90)
			{
				float distanceToTarget = Vector3.Distance(transform.position, obj.position);
				if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget - 0.1f))
				{
					visibleObj = obj;
					if(obj.CompareTag("Player"))
					{
						target = obj;
						return true;
					}
				}
			}
		}
		target = visibleObj;
		return visibleObj;
	}
	
	
	
	IEnumerator RemoveDestroyedObjects()
	{
		while(true)
		{
			for (int i = 0; i < objectsOfInterest.Count; i++)
			{
				if(objectsOfInterest[i] == null)
				{
					objectsOfInterest.RemoveAt(i);
				}
			}
			yield return new WaitForSeconds(1);
		}
	}
	
}
