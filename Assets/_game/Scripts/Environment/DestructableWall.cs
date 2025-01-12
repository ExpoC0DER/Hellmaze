using UnityEngine;
using UnityEngine.AI;
using FMODUnity;
using FMOD.Studio;
using _game.Scripts;

public class DestructableWall : MonoBehaviour, IDestructable
{
	[field: SerializeField] public float Health { get; set; } = 100;
	[field: SerializeField] public float MaxHealth { get; set; } = 100;
	public bool IsDead { get; set; } = false;
	
	[SerializeField] bool isFloor = false;
	
	[SerializeField] ParticleSystem die_part;
	[SerializeField] MeshRenderer meshRenderer;
	[SerializeField] NavMeshObstacle obstacle;
	[SerializeField] Collider col;
	
	[SerializeField] EventReference destroy_sfx;

	public void Respawn()
	{
		meshRenderer.enabled = true;
		Health = MaxHealth;
		IsDead = false;
	}
	
	public void Die()
	{
		//meshRenderer.material.SetFloat(Shader.PropertyToID("_Dissolve"), 0f);
		FMODHelper.PlayNewInstance(destroy_sfx, transform);
		obstacle.enabled = isFloor;
		meshRenderer.enabled = false;
		col.enabled = false;
		die_part.Play();
		IsDead = true;
	}

	public void TakeDamage(float amount, PlayerStats source, int weaponIndex)
	{
		if(IsDead) return;
		Health -= amount;
		if(Health <= 0)
		{
			Die();
			
		}
	}
	
	void OnEnable()
	{
		if(isFloor) Respawn();
	}
}
