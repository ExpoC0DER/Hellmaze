using System.Collections.Generic;
using AYellowpaper;
using UnityEngine;
using System;
using _game.Scripts;

public class WeaponSlots : MonoBehaviour
{
	[SerializeField] PlayerStats playerStats;
	[SerializeField] PlayerAnimatorFunctions animatorFunctions;
	[SerializeField] Animator animator;
	[SerializeField] InterfaceReference<IGun, MonoBehaviour>[] _guns;
	[SerializeField] List<GameObject> weaponObjects;
	[SerializeField] bool autoSwitch = false;
	int currentWeaponIndex = 0;
	InterfaceReference<IGun, MonoBehaviour> _currentGun;
	bool[] weaponsPicked;
	bool isPlayer = true;
	public event Action<int> OnAmmoChange;
	[SerializeField] List<KeyCode> keyBinds;
	
	
	[Header("Bot")]
	[SerializeField] float innaccuracy = 1;
	[SerializeField] float damage_multiplier = 0.5f;
	bool bot_shoot_trigger = false;
	
	void Awake()
	{
		isPlayer = CompareTag("Player");
		for (int i = 0; i < _guns.Length; i++)
		{
			_guns[i].Value.WeaponIndex = i;
			_guns[i].Value.Source = playerStats;
		}
		weaponsPicked = new bool[weaponObjects.Count];
		currentWeaponIndex = 0;
		PickWeapon(currentWeaponIndex, 0);
		if(!autoSwitch)SwitchWeapon(currentWeaponIndex);
		if(!isPlayer) // setup bot weapons
		{
			for (int i = 0; i < _guns.Length; i++)
			{
				_guns[i].Value.BotInaccuracy = innaccuracy;
				_guns[i].Value.DamageMultiplier = damage_multiplier;
			}	
		}		
	}
	
	void Update()
	{
		if(!isPlayer) return;
		if(Menu.main.isPaused) return;
		
		HandleInput();
		HandleShooting();
	}
	
	void HandleInput()
	{
		for (int i = 0; i < keyBinds.Count; i++)
		{
			if(Input.GetKeyDown(keyBinds[i]))
			{
				SwitchWeapon(i);
			}
		}
	}
	
	public void PickWeapon(int weaponIndex, int ammoAmount)
	{
		weaponsPicked[weaponIndex] = true;
		
		//add ammo
		_guns[weaponIndex].Value.PickAmmo(ammoAmount);
		
		if(autoSwitch) SwitchWeapon(weaponIndex);
	}
	
	void SwitchWeapon(int weaponIndex)
	{
		if(!weaponsPicked[weaponIndex]) return;
		bool canSwitch;
		for (int i = 0; i < weaponObjects.Count; i++)
		{
			canSwitch = i == weaponIndex;
			weaponObjects[i].SetActive(canSwitch);
			if(canSwitch)
			{
				animatorFunctions.SetWeapon(i);
				animator.SetFloat("WeaponIndex", i);
				currentWeaponIndex = weaponIndex;
				_currentGun = _guns[currentWeaponIndex];
				OnAmmoChange?.Invoke(_currentGun.Value.Ammo);
			}  
		}
	}
	void SwitchToWeaponWithAmmo()
	{
		bool switched = false;
		for (int i = weaponObjects.Count -1; i >= 0; i--)
		{
			bool canSwitch = weaponsPicked[i] && _guns[i].Value.Ammo > 0;
			weaponObjects[i].SetActive(canSwitch && !switched);
			if(canSwitch && !switched)
			{
				animatorFunctions.SetWeapon(i);
				animator.SetFloat("WeaponIndex", i);
				currentWeaponIndex = i;
				_currentGun = _guns[currentWeaponIndex];
				OnAmmoChange?.Invoke(_currentGun.Value.Ammo);
				switched = true;
			}
		}
	}
	
	private void HandleShooting()
	{
		bool triggered = Input.GetMouseButton(0);
		_currentGun.Value.Shoot(triggered, null, out bool succesShot);
		if (succesShot) OnAmmoChange?.Invoke(_currentGun.Value.Ammo);
		animator.SetBool("Shooting", succesShot && triggered);
		
	}
	
	public void Bot_Shooting(Transform target, Transform source)
	{
		if(bot_shoot_trigger)
		{
			_currentGun.Value.StopShooting();
			animator.SetBool("Shooting", false);
			return;
		}
		Debug.Log("shot");
		bot_shoot_trigger = true;
		_currentGun.Value.Shoot(bot_shoot_trigger, source, out bool succesShot);
		animator.SetBool("Shooting", succesShot && bot_shoot_trigger);
		Invoke("StopShooting", UnityEngine.Random.Range(0.2f, 0.4f));
		if(!succesShot) // no ammo try other gun
		{
			SwitchToWeaponWithAmmo();
		}
	}
	
	public void StopShooting() => bot_shoot_trigger = false;
	
	
	public void ThrowAwayWeapon(int weaponIndex)
	{
		if(weaponIndex == 0) return;
		weaponsPicked[weaponIndex] = false;
		
		//spawn projectile of throw away gun
		
		for (int i = 0; i < weaponObjects.Count; i++)
		{
			if(weaponsPicked[i] == true)
			{
				SwitchWeapon(i);
				break;
			}
		}
	}
}
