using UnityEngine;
using AYellowpaper;
using System.Collections.Generic;
using FMODUnity;
using FMOD.Studio;
using _game.Scripts;

public class DamagingFloor : MonoBehaviour
{
	[SerializeField] Collider col;
	[SerializeField] float _damage = 3;
	[SerializeField] float _damageInterval = 0.3f;
	[SerializeField] int _weaponIndex = 0;
	
	[SerializeField] EventReference damage_sfx;
	
	PlayerStats source = null;
	
	float _cd;
	
	void Update()
	{
		if(!this.isActiveAndEnabled) return;
		if(_cd >= 0)
		{
			_cd -= Time.deltaTime;
			if(_cd <= 0)
			{
				col.enabled = false;
				_cd = _damageInterval;
			}else
			{
				col.enabled = true;
			}
			
		}
	}
	
	private void OnTriggerEnter(Collider other)
	{
		if(other.TryGetComponent(out PlayerStats player))
		{
			player.TakeDamage(_damage, source, _weaponIndex);
			if(!damage_sfx.IsNull) FMODHelper.PlayNewInstance(damage_sfx, transform);
		}
	}
	
	void OnDisable()
	{
		col.enabled = false;
		_cd = _damageInterval;
	}
	
	public void SetSource(PlayerStats player) => source = player;
}
