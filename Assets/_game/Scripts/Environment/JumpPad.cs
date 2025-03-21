using _game.Scripts;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class JumpPad : MonoBehaviour
{
	[SerializeField] float force;
	[SerializeField] EventReference jump_sfx;
	
	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			if(other.TryGetComponent(out PlayerController playerController))
			{
				playerController.ApplyForce(Vector3.up * force);
				FMODHelper.PlayNewInstance(jump_sfx, transform);
			}
		}
		else if(other.CompareTag("Bot"))
		{
			if(other.TryGetComponent(out EnemyAI enemyAI))
			{
				enemyAI.SimulateImpulsePhysics(GetRandomBotDirection() * force * 0.5f);
				FMODHelper.PlayNewInstance(jump_sfx, transform);
			}
		}
	}
	
	Vector3 GetRandomBotDirection()
	{
	    return new Vector3(Random.Range(0.2f, 1), 1, Random.Range(0.2f, 1));
	}
}
