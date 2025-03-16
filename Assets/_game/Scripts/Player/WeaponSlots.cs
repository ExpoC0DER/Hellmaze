using System.Collections.Generic;
using AYellowpaper;
using EditorAttributes;
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
	bool isDead = false;
	public event Action<int> OnAmmoChange;
	public event Action<int> OnWeaponChange;
	[SerializeField] List<KeyCode> keyBinds;
	
	
	[Header("Bot")]
	[SerializeField] float innaccuracy = 1;
	[SerializeField] float damage_multiplier = 0.5f;
	bool bot_shoot_trigger = false;
	bool botCanShot = true;
	bool _shootInput = false;
	float scrollCd = 0;
	float scrollCd_Time = 0.1f;
		
	public int WeaponToSwitchIndex = 0;
	[Button]
	void ForceSwitchWeapon()
	{
		weaponsPicked[WeaponToSwitchIndex] = true;
		SwitchWeapon(WeaponToSwitchIndex);
	}
	
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
		playerStats.OnDeath += OnDeath;
		playerStats.OnRespawn += OnRespawn;
	}
	
	void OnEnable()
	{
		GameManager.main.playerControlls.Player.Attack.started += x => _shootInput = true;
		GameManager.main.playerControlls.Player.Attack.canceled += x => _shootInput = false;
		
	}
	
	void OnDisable()
	{
		playerStats.OnDeath -= OnDeath;
		playerStats.OnRespawn -= OnRespawn;
		
		GameManager.main.playerControlls.Player.Attack.started -= x => _shootInput = true;
		GameManager.main.playerControlls.Player.Attack.canceled -= x => _shootInput = false;
	}
	
	void Update()
	{
		if(!isPlayer) return;
		if(Menu.main.isPaused) return;
		if(isDead) return;
		if(scrollCd >= 0)
		{
		    scrollCd -= Time.deltaTime;
		}
		//HandleInput();
		HandleShooting(_shootInput);
		HandleScrollWheelInput(GameManager.main.playerControlls.Player.WeaponSelect.ReadValue<Vector2>().y);
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
	
	void HandleScrollWheelInput(float input)
	{
		if(scrollCd > 0) return;
		float scroll = input;//Input.GetAxis("Mouse ScrollWheel");
		if(scroll > 0)
		{
			Debug.Log("Scroll Next to " + ScrollToNextWeapon() + " from " + currentWeaponIndex);
			SwitchWeapon(ScrollToNextWeapon());
		}
		else if(scroll < 0)
		{
			Debug.Log("Scroll Back to " + ScrollToPreviousWeapon() + " from " + currentWeaponIndex);
			SwitchWeapon(ScrollToPreviousWeapon());
		}
	}
	
	int ScrollToNextWeapon()
	{
		int weaponIndex = currentWeaponIndex + 1;
		
		if(weaponIndex >= weaponsPicked.Length) weaponIndex = 0;
		
		for (int i = weaponIndex; i < weaponsPicked.Length; i++)
		{
			if(weaponsPicked[i]) return i;
			if(i == weaponsPicked.Length - 1) i = 0;
		}
		return 0;
		//return weaponIndex;
	}
	
	int ScrollToPreviousWeapon()
	{
		int weaponIndex = currentWeaponIndex - 1;
		
		if(weaponIndex < 0) weaponIndex = weaponsPicked.Length -1;
		
		for (int i = weaponIndex; i >= 0 ; i--)
		{
			if(weaponsPicked[i]) return i;
			if(i == 0) i = weaponsPicked.Length -1;
		}
		
		return 0;
		//return weaponIndex;
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
		if(weaponIndex != 0)_currentGun.Value.StopShooting();
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
		OnWeaponChange?.Invoke(currentWeaponIndex);
		scrollCd = scrollCd_Time;
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
	
	private void HandleShooting(bool input)
	{
		bool triggered = input;//Input.GetMouseButton(0);
		_currentGun.Value.Shoot(triggered, null, out bool succesShot);
		if (succesShot) OnAmmoChange?.Invoke(_currentGun.Value.Ammo);
		animator.SetBool("Shooting", succesShot && triggered);
		
	}
	
	public void Bot_Shooting(Transform target)
	{
		/* if(bot_shoot_trigger)
		{
			_currentGun.Value.StopShooting();
			animator.SetBool("Shooting", false);
			return;
		} */
		
		//bot_shoot_trigger = true;
		_currentGun.Value.Shoot(botCanShot, target, out bool succesShot);
		animator.SetBool("Shooting", succesShot);
		
		//Invoke("StopShooting", UnityEngine.Random.Range(0.2f, 0.4f)); //insert gun firerate here
		
		if(!succesShot) // no ammo try other gun
		{
			SwitchToWeaponWithAmmo();
		}
	}
	
	/* public void StopShooting()
	{
		bot_shoot_trigger = false;
		Debug.Log("bot reset shot");
	} */
	
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
	
	void ResetWeaponsOnDeath()
	{
		for (int i = 0; i < weaponsPicked.Length; i++)
		{
			weaponsPicked[i] = false;
		}
		for (int i = 0; i < weaponObjects.Count; i++)
		{
			weaponObjects[i].SetActive(false);
		}
	}
	
	public bool[] GunsThatHaveAmmo()
	{
		bool[] gunsThatHaveAmmo = new bool[_guns.Length];
		for (int i = 0; i < _guns.Length; i++)
		{
			gunsThatHaveAmmo[i] = _guns[i].Value.Ammo != 0;
		}
		return gunsThatHaveAmmo;
	}
	
	void OnDeath()
	{
		botCanShot = false;
		_currentGun.Value.StopShooting();
		animator.SetBool("Shooting", false);
		ResetWeaponsOnDeath();
		isDead = true;
	}
	
	void OnRespawn()
	{
		botCanShot = true;
		weaponsPicked[0] = true;
		SwitchWeapon(0);
		isDead = false;
	}
}
