using System.Collections.Generic;
using AYellowpaper;
using UnityEngine;
using System;
using _game.Scripts;

public class WeaponSlots : MonoBehaviour
{
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
	
	void Awake()
	{
		isPlayer = CompareTag("Player");
		weaponsPicked = new bool[weaponObjects.Count];
		currentWeaponIndex = 0;
		PickWeapon(currentWeaponIndex, 0);
		if(!autoSwitch)SwitchWeapon(currentWeaponIndex);
	}
	
	void Update()
	{
		if(!isPlayer) return;
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
				animator.SetFloat("WeaponIndex", 0);
				currentWeaponIndex = weaponIndex;
				_currentGun = _guns[currentWeaponIndex];
				OnAmmoChange?.Invoke(_currentGun.Value.Ammo);
			}  
		}
	}
	
	private void HandleShooting()
	{
		bool triggered = Input.GetMouseButton(0);
		_currentGun.Value.Shoot(triggered, out bool succesShot);
		if (succesShot) OnAmmoChange?.Invoke(_currentGun.Value.Ammo);
		animator.SetBool("Shooting", succesShot && triggered);
		
	}
	
	public void Bot_Shooting()
	{
		
	}
	
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
