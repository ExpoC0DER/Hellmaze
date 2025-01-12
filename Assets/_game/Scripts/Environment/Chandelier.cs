using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using _game.Scripts;

public class Chandelier : MonoBehaviour, IDestructable
{
	[field: SerializeField] public float Health { get; set; } = 100;
	[field: SerializeField] public float MaxHealth { get; set; } = 100;
	public bool IsDead { get; set; } = false;
	
	[SerializeField] Rigidbody rb;
	[SerializeField] DamagingFloor damagingFloor;
	[SerializeField] Collider dmgFloor_col;
	Vector3 startPos;
	
	[SerializeField] EventReference chainBreak_sfx;
	[SerializeField] PlayAudioOnCollision playAudioOnCollision;
	
	void Start() 
	{
		startPos = transform.localPosition;
	}
	
	public void Die()
	{
		FMODHelper.PlayNewInstance(chainBreak_sfx, transform);
		
		IsDead = true;
		rb.isKinematic = false;
		damagingFloor.enabled = true;
		dmgFloor_col.enabled = true;
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

	public void Respawn()
	{
		transform.localPosition = startPos;
		rb.isKinematic = true;
		IsDead = false;
		damagingFloor.enabled = false;
		dmgFloor_col.enabled = false;
		playAudioOnCollision.Reset();
	}
}
