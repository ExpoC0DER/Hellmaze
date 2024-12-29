using _game.Scripts;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
	[SerializeField] float force;
	
	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			if(other.TryGetComponent(out PlayerController playerController))
			{
				playerController.ApplyForce(Vector3.up * force);
			}
		}
		else if(other.CompareTag("Bot"))
		{
			if(other.TryGetComponent(out EnemyAI enemyAI))
			{
				enemyAI.SimulateImpulsePhysics(Vector3.up * force * 0.5f);
			}
		}
	}
}
