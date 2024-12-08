using UnityEngine;
using System;
using Random = UnityEngine.Random;
using UnityEngine.AI;
using _game.Scripts;

public class PlayerStats : MonoBehaviour
{
	public string playerName = "Player";
	public Sprite playerProfilePic;
	[SerializeField] float currentHealth = 100;
	[SerializeField] float maxHealth = 100;	
	[SerializeField] EnemyAI enemyAI;
	[SerializeField] PlayerAnimatorFunctions animatorFunctions;
	[SerializeField] float respawnTime = 5;
	[SerializeField] PlayerController playerController;
	public bool isPlayer {get; private set;}
	
	public int kills { get; private set; } = 0;
	public int deaths { get; private set; } = 0;
	bool dead = false;
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
		currentHealth -= amount;
		if(!dead) blood_part.Play();
		
		//Debug.Log(playerName + " was hit by " + source.playerName + " for damage of:" + amount);
		
		if(currentHealth <= 0 && !dead)
		{
			Die();
			UpdateKillFeed?.Invoke(source.playerName, weaponIndex, playerName);
			source.IncreaseKillCount();
			deaths++;
		}
		
		if((IsWeaponInstaGibbing(weaponIndex) && currentHealth < 0) || currentHealth < -50 && !gibbed)
		{
			GibBody();
			gibbed = true;
		}
		
		OnHealthChange?.Invoke(currentHealth);
	}
	
	public void AddHealth(float amount)
	{
		currentHealth += amount;
		if(currentHealth >= maxHealth)
		{
			currentHealth = maxHealth;
		}
		OnHealthChange?.Invoke(currentHealth);
	}
	
	void Die()
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
		
		dead = true;
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
		
		currentHealth = maxHealth;
		
		dead = false;
		
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
}
