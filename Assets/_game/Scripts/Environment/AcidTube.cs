using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using FMODUnity;
using FMOD.Studio;
using _game.Scripts;

public class AcidTube : MonoBehaviour, IDestructable
{
	public float Health { get; set; } = 100;
	public float MaxHealth { get; set; } = 100;
	public bool IsDead { get; set; } = false;
	
	[SerializeField] ParticleSystem die_part;
	[SerializeField] MeshRenderer meshRenderer;
	[SerializeField] GameObject[] visualObjs;
	[SerializeField] NavMeshObstacle obstacle;
	[SerializeField] Collider col;
	
	[SerializeField] string poolName = "acidSplash";
	[SerializeField] int projNumber = 8;
	[SerializeField] float damage = 10;
	[SerializeField] int weaponIndex = 16;
	PlayerStats lastSource;

	[SerializeField] EventReference tube_sfx;
	private EventInstance tubeSound;
	
	void Start()
	{
		tubeSound = FMODHelper.CreateNewInstance(tube_sfx, transform);
		tubeSound.setParameterByName("Parameter 1", 1);
		tubeSound.start();
	}
	
	public void Respawn()
	{
		for (int i = 0; i < visualObjs.Length; i++)
		{
			visualObjs[i].SetActive(true);
		}
		col.enabled = true;
		meshRenderer.enabled = true;
		Health = MaxHealth;
		IsDead = false;
		
		if (FMODHelper.InstanceIsPlaying(tubeSound))
		{
			tubeSound.setParameterByName("Parameter 1", 2);
			tubeSound.release();
		}
		
		tubeSound = FMODHelper.CreateNewInstance(tube_sfx, transform);
		tubeSound.setParameterByName("Parameter 1", 1);
		tubeSound.start();
	}
	
	public void Die()
	{
		meshRenderer.enabled = false;
		col.enabled = false;
		foreach (var obj in visualObjs)
		{
			obj.SetActive(false);
		}
		
		for (int i = 0; i < projNumber; i++)
		{
			float rot_Y = i *(360/projNumber);
			Vector3 rot = new Vector3(Random.Range(10,25), rot_Y, 0);
			ObjectPooler.main.SpawnProjectile(poolName, transform.position, Quaternion.Euler(rot), lastSource, damage, weaponIndex);
		}
		
		if (FMODHelper.InstanceIsPlaying(tubeSound))
		{
			tubeSound.setParameterByName("Parameter 1", 2);
			tubeSound.release();
		}
		
		die_part.Play();
		IsDead = true;
	}

	public void TakeDamage(float amount, PlayerStats source, int weaponIndex)
	{
		if(IsDead) return;
		Health -= amount;
		lastSource = source;
		if(Health <= 0)
		{
			Die();
		}
	}
	
}
