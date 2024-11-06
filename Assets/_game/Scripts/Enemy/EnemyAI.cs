using System;
using System.Collections;
using _game.Scripts;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour
{
	[SerializeField] private NavMeshAgent navMeshAgent;
	[SerializeField] private float _navigationRadius = 30;
	[SerializeField] Transform target;

	[SerializeField] float fovAngle = 90;

	[SerializeField] private float health = 100;
	private bool _dead = false;

	public bool visibleTarget { get; private set; }
	
	public static event Action OnDeath;

	Bot_State currentState;
	public Bot_State_Attacking state_Attacking = new Bot_State_Attacking();
	public Bot_State_Tracking state_Tracking = new Bot_State_Tracking();
	public Bot_State_Roam state_Roam = new Bot_State_Roam();
	

	private void Start()
	{
		visibleTarget = false;
		target = FindFirstObjectByType<PlayerController>().gameObject.transform;
		StartCoroutine(RandomMovement());
		
		currentState = state_Roam;
		currentState.EnterState(this);
	}

	private IEnumerator RandomMovement()
	{
		float randomTime;
		while (!_dead)
		{
			SetRandomPos();
			randomTime = Random.Range(2f, 5f);
			yield return new WaitForSeconds(randomTime);
		}
	}

	void Update()
	{
		visibleTarget = SeePlayer();
		
		if (visibleTarget)
		{
			FollowPlayer();
		}
		
		currentState.UpdateState();
	}
	
	public void SwitchState(Bot_State state)
	{
		currentState.ExitState();
		currentState = state;
		currentState.EnterState(this);
	}

	private void SetRandomPos()
	{
		Vector3 pointInRadius = transform.position + Random.insideUnitSphere * _navigationRadius;
		pointInRadius.y += 150;
		RaycastHit hit;

		Physics.Raycast(pointInRadius, Vector3.down, out hit);
		if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 1, NavMesh.AllAreas))
		{
			navMeshAgent.SetDestination(hit.point);
		}
	}

	private bool SeePlayer()
	{
		Vector3 directionToTarget = (target.position - transform.position).normalized;

		if (Vector3.Angle(transform.forward, directionToTarget) < fovAngle / 2)
		{
			float distanceToTarget = Vector3.Distance(transform.position, target.position);

			if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget))
			{
				return true;
			}
			else
				return false;
		}
		else
			return false;
	}

	void FollowPlayer()
	{
		if (Vector3.Distance(target.position, transform.position) > 5)
		{
			navMeshAgent.SetDestination(target.position);

		}
		else
		{
			SetRandomPos();
		}
	}

	public void TakeDamage(float damage)
	{
		health -= damage;
		if(health<=0)
			Respawn();
	}

	private void Respawn()
	{
		OnDeath?.Invoke();
		health = 100;
		int side = Random.Range(0, 4);
		int cell = Random.Range(-5, 6);
		switch (side)
		{
			case 0:
				navMeshAgent.Warp(new Vector3(20, 0, cell * 4));
				break;
			case 1:
				navMeshAgent.Warp(new Vector3(-20, 0, cell * 4));
				break;
			case 2:
				navMeshAgent.Warp(new Vector3(cell * 4, 0, 20));
				break;
			case 3:
				navMeshAgent.Warp(new Vector3(cell * 4, 0, -20));
				break;
		}
	}
}

public abstract class Bot_State
{
	public abstract void EnterState(EnemyAI ai);
	public abstract void UpdateState();
	public abstract void ExitState();
}

public class Bot_State_Roam : Bot_State
{
	EnemyAI enemyAI;
	public override void EnterState(EnemyAI ai)
	{
		enemyAI = ai;
	}
	public override void UpdateState()
	{
		
	}
	public override void ExitState()
	{
		
	}
}

public class Bot_State_Tracking : Bot_State
{
	EnemyAI enemyAI;
	public override void EnterState(EnemyAI ai)
	{
		enemyAI = ai;
	}
	public override void UpdateState()
	{
		
	}
	public override void ExitState()
	{
		
	}
}

public class Bot_State_Attacking : Bot_State
{
	EnemyAI enemyAI;
	public override void EnterState(EnemyAI ai)
	{
		enemyAI = ai;
	}
	public override void UpdateState()
	{
		
	}
	public override void ExitState()
	{
		
	}
}
