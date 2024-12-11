using UnityEngine;
using System;
using Random = UnityEngine.Random;
using UnityEngine.AI;
using _game.Scripts;
using UnityEngine.UI;
using System.Collections;

public class PlayerStats : MonoBehaviour, IDestructable
{
	public string playerName = "Player";
	public Sprite playerProfilePic;
	[SerializeField] EnemyAI enemyAI;
	[SerializeField] PlayerAnimatorFunctions animatorFunctions;
	[SerializeField] float respawnTime = 5;
	[SerializeField] PlayerController playerController;
	public bool isPlayer {get; private set;}
	
	public int kills { get; private set; } = 0;
	public int deaths { get; private set; } = 0;
	
	public float Health { get; set; } = 100;
	public float MaxHealth { get; set; } = 100;
	public bool IsDead { get; set; } = false;

	bool gibbed = false;
	
	int mapCellScale = 4;
	int mapCellCount = 20;
	
	int[] instaGibWeaponIndexes = {2, 3};
	
	[SerializeField] ParticleSystem blood_part;
	[SerializeField] ParticleSystem gib_part;
	
	public event Action<float> OnHealthChange;
	public static event Action<string,int,string> UpdateKillFeed;
	public event Action OnDeath;
	public event Action OnRespawn;
	
	private void Awake() {
		isPlayer = CompareTag("Player");
	}
	
	public void IncreaseKillCount() => kills++;
	
	public void TakeDamage(float amount, PlayerStats source, int weaponIndex)
	{
		Health -= amount;
		if(!IsDead) blood_part.Play();
		
		//Debug.Log(playerName + " was hit by " + source.playerName + " for damage of:" + amount);
		
		if(Health <= 0 && !IsDead)
		{
			Die();
			UpdateKillFeed?.Invoke(source.playerName, weaponIndex, playerName);
			source.IncreaseKillCount();
			deaths++;
		}
		
		if((IsWeaponInstaGibbing(weaponIndex) && Health < 0) || Health < -50 && !gibbed)
		{
			GibBody();
			gibbed = true;
		}
		
		OnHealthChange?.Invoke(Health);
	}
	
	public void AddHealth(float amount)
	{
		Health += amount;
		if(Health >= MaxHealth)
		{
			Health = MaxHealth;
		}
		OnHealthChange?.Invoke(Health);
	}
	
	public void Die()
	{
		OnDeath?.Invoke();
		
		animatorFunctions.SetDead();
		
		if(!isPlayer)
		{
			enemyAI.SetDead(true);
		}else
		{
			playerController.ResetCharacterControllerVelocity();
		}
		
		IsDead = true;
		Invoke("Respawn", respawnTime);
	}
	
	
	private void Respawn()
	{
		OnRespawn?.Invoke();
		if(gibbed)
		{
			UnGibBody();
			gibbed = false;
		}
		animatorFunctions.SetAlive();
		
		Health = MaxHealth;
		OnHealthChange?.Invoke(Health);
		
		IsDead = false;
		
		if(!isPlayer)
		{
			enemyAI.SetDead(false);
			enemyAI.Respawn(mapCellScale, mapCellCount);
			return;
		}
		ResetPlayerPosition();
	}
	
	void ResetPlayerPosition()
	{
		int side = Random.Range(0, mapCellScale);
		int cell = Random.Range(-5, 6);
		switch (side)
		{
			case 0:
				transform.position = new Vector3(mapCellCount, 0, cell * mapCellScale);
				break;
			case 1:
				transform.position = new Vector3(-mapCellCount, 0, cell * mapCellScale);
				break;
			case 2:
				transform.position = new Vector3(cell * mapCellScale, 0, mapCellCount);
				break;
			case 3:
				transform.position = new Vector3(cell * mapCellScale, 0, -mapCellCount);
				break;
		}
	}
	
	void GibBody()
	{
		if(!isPlayer) enemyAI.SetGibbed(true);
		else playerController.SetGibbed(true);
		
		animatorFunctions.gameObject.SetActive(false);
		gib_part.Play();
		if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit gibHit, 2, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
		{
			ObjectPooler.main.SpawnPooledObject("blood_gib_dec", gibHit.point, Quaternion.LookRotation(gibHit.normal), gibHit.transform);
		}
		
	}
	
	void UnGibBody()
	{
		if(!isPlayer) enemyAI.SetGibbed(false);
		else playerController.SetGibbed(false);
		
		animatorFunctions.gameObject.SetActive(true);
	}

	bool IsWeaponInstaGibbing(int weaponIndex)
	{
		for (int i = 0; i < instaGibWeaponIndexes.Length; i++)
		{
			if(instaGibWeaponIndexes[i] == weaponIndex) return true;
		}
		return false;
	}
	
	void SetPlayerName(string name)
	{
		if(!isPlayer) return;
		playerName = name;
		Debug.Log("event name passed");
	}
	
	
	private void OnEnable()
	{
		GameSettings.SetPlayerName += SetPlayerName;
	}
	private void OnDisable()
	{
		GameSettings.SetPlayerName -= SetPlayerName;
	}
}
