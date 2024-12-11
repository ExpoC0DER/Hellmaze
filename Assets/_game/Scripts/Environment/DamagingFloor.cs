using UnityEngine;
using AYellowpaper;
using System.Collections.Generic;

public class DamagingFloor : MonoBehaviour
{
	List<InterfaceReference<IDestructable, MonoBehaviour>> destructables = new List<InterfaceReference<IDestructable, MonoBehaviour>>();
	[SerializeField] float _damage = 3;
	[SerializeField] float _damageInterval = 0.3f;
	[SerializeField] int _weaponIndex = 0;
	
	float _cd;
	
	void Update()
	{
		if(_cd >= 0)
		{
			_cd -= Time.deltaTime;
			if(_cd <= 0)
			{
				DamageObjects();
				_cd = _damageInterval;
			}
		}
	}
	
	void DamageObjects()
	{
		List<InterfaceReference<IDestructable, MonoBehaviour>> tempList = new List<InterfaceReference<IDestructable, MonoBehaviour>>();
		for (int i = 0; i < destructables.Count; i++)
		{
			if(destructables[i] != null)
			{
				tempList.Add(destructables[i]);
			}
		}
		destructables.Clear();
		destructables = tempList;
		
		for (int i = 0; i < destructables.Count; i++)
		{
			destructables[i].Value.TakeDamage(_damage, null, _weaponIndex);
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.transform.TryGetComponent(out IDestructable dest))
		{
			destructables.Add(dest as InterfaceReference<IDestructable, MonoBehaviour>);
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if(other.transform.TryGetComponent(out IDestructable dest))
		{
			destructables.Remove(dest as InterfaceReference<IDestructable, MonoBehaviour>);
		}
	}
}
