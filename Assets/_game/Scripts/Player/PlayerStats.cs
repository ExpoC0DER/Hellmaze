using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class PlayerStats : MonoBehaviour
{
	public string playerName = "Bot";
	[SerializeField] float currentHealth = 100;
	[SerializeField] float maxHealth = 100;	
	[SerializeField] EnemyAI enemyAI;
	public bool isPlayer {get; private set;}
	
	public int kills { get; private set; } = 0;
	public int deaths { get; private set; } = 0;
	bool dead = false;
	
	int mapCellScale = 4;
	int mapCellCount = 20;
	
	[SerializeField] ParticleSystem blood_part;
	
	public event Action<float> OnHealthChange;
	public static event Action<string,int,string> UpdateKillFeed;
	public static event Action OnDeath;
	
	private void Awake() {
		isPlayer = CompareTag("Player");
	}
	
	public void IncreaseKillCount() => kills++;
	
	public void TakeDamage(float amount, PlayerStats source, int weaponIndex)
	{
		currentHealth -= amount;
		blood_part.Play();
		
		Debug.Log(playerName + " was hit by " + source.playerName + " for damage of:" + amount);
		
		if(currentHealth <= 0)
		{
			Debug.Log("died");
			Die();
			UpdateKillFeed?.Invoke(source.playerName, weaponIndex, playerName);
			source.IncreaseKillCount();
			deaths++;
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
		
		//dead = true;
		Respawn();
	}
	
	private void Respawn()
	{
		//reset state
		currentHealth = maxHealth;
		
		if(!isPlayer)
		{
			enemyAI.Respawn(mapCellScale, mapCellCount);
			return;
		} 
		
		//reset position
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
}
