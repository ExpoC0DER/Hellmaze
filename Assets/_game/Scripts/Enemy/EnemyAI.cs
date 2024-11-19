using System;
using System.Collections;
using System.Runtime.InteropServices;
using _game.Scripts;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour
{
	//bot difficulty idea: less likely they will flee, faster reaction time, better accuracy
	[SerializeField] public NavMeshAgent navMeshAgent {get; private set;}
	[SerializeField] public float navigationRadius {get; private set;} = 30;
	
	[SerializeField] EnemyVision enemyVision;
	[SerializeField] float fovAngle = 90;

	[SerializeField] private float health = 100;
	[SerializeField] private float maxHealth = 100;
	[SerializeField] ParticleSystem death_part;
	[SerializeField] ParticleSystem hit_part;
	private bool _dead = false;
	public Transform target {get; private set;}
	public Vector3 lastSeenTargetPos {get; private set;}
	
	public static event Action OnDeath;

	Bot_State currentState;
	public Bot_State_Attacking state_Attacking = new Bot_State_Attacking();
	public Bot_State_Tracking state_Tracking = new Bot_State_Tracking();
	public Bot_State_Roam state_Roam = new Bot_State_Roam();
	

	/* private void Start()
	{
		currentState = state_Roam;
		currentState.EnterState(this);
	}

	void Update()
	{
		currentState.UpdateState();
	} */
	
	public void SwitchState(Bot_State state)
	{
		currentState.ExitState();
		currentState = state;
		currentState.EnterState(this);
	}
	
	public void SetTarget(Transform target)
	{
		this.target = target;
		SwitchState(state_Attacking);
	}
	public void SeenHealthKit(Transform pickup)
	{
		//if health is low go to this position
	}
	public void SeenWeaponPickup(Transform pickup)
	{
		//if ammo is low go to this position
	}
	public void FindWeaponPickup()
	{
		// call this from weapon script when ammo is low
	}
	void FindHealthKit()
	{
		//if health is bellow 30 go to this position
		if(enemyVision.CheckClosestHealthKit(out Transform position))
		{
			
		}
	}
	
	public void TrackTarget(Vector3 position)
	{
		lastSeenTargetPos = position;
		SwitchState(state_Tracking);
	}
	
	public void TakeDamage(float damage)
	{
		hit_part.Play();//make it play correctly on pos and rot
		health -= damage;
		if(health<=0)
		{
			Respawn();
		}
		if(health < 30)
		{
			FindHealthKit();
		}
	}
	
	public void AddHealth(float amount)
	{
		health += amount;
		if(health > maxHealth) health = maxHealth;
		if(health < 30)
		{
			FindHealthKit();
		}
	}

	private void Respawn()
	{
		death_part.Play();
		OnDeath?.Invoke();
		
		//reset state
		health = maxHealth;
		target = null;
		currentState = state_Roam;
		currentState.EnterState(this);
		
		//reset position
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
	NavMeshAgent agent;
	public override void EnterState(EnemyAI ai)
	{
		enemyAI = ai;
	}
	public override void UpdateState()
	{
		if(agent.remainingDistance < 0.3f)
		{
			SetRandomPosition();
		}
			//jump or crouch randomly
			//crouch through small spaces
			//if seen interactable object (grappling point, explosive barrel) interact accordingly
		
	}
	public override void ExitState()
	{
		
	}
	
	void SetRandomPosition()
	{
		Vector3 pointInRadius = enemyAI.transform.position + Random.insideUnitSphere * enemyAI.navigationRadius;
		pointInRadius.y += 150;
		RaycastHit hit;

		Physics.Raycast(pointInRadius, Vector3.down, out hit);
		if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 1, NavMesh.AllAreas))
		{
			agent.SetDestination(hit.point);
		}
	}
}

public class Bot_State_Tracking : Bot_State
{
	Vector3 lastSeenPos;
	EnemyAI enemyAI;
	NavMeshAgent agent;
	public override void EnterState(EnemyAI ai)
	{
		lastSeenPos = ai.lastSeenTargetPos;
		enemyAI = ai;
		agent = enemyAI.navMeshAgent;
		if (NavMesh.SamplePosition(lastSeenPos, out NavMeshHit navHit, 1, NavMesh.AllAreas))
		{
			agent.SetDestination(lastSeenPos);
		}else
		{
			enemyAI.SwitchState(enemyAI.state_Roam);
		}
	}
	public override void UpdateState()
	{
		if(agent.remainingDistance < 0.3f)
		{
			enemyAI.SwitchState(enemyAI.state_Roam);
		}
			//jump or crouch randomly
			//crouch through small spaces
			//if seen interactable object (grappling point, explosive barrel) interact accordingly
	}
	public override void ExitState()
	{
		
	}
}

public class Bot_State_Attacking : Bot_State
{
	NavMeshAgent agent;
	EnemyAI enemyAI;
	public override void EnterState(EnemyAI ai)
	{
		enemyAI = ai;
		agent = enemyAI.navMeshAgent;
	}
	public override void UpdateState()
	{
		if(agent.remainingDistance < 0.3f)
		{
			SetRandomPosition();
		}
			//jump or crouch randomly
			//crouch through small spaces
			//if seen interactable object (grappling point, explosive barrel) interact accordingly
		//upper body is focused on target player (with some inaccuracy) and shoot him
	}
	public override void ExitState()
	{
		
	}
	
	void SetRandomPosition()
	{
		bool pointInRoom = false;
		Vector3 pointInRadius = Vector3.zero;
		
		//check if random position is in the same room (bot wont leave fight)
		while(!pointInRoom)
		{
			pointInRadius = enemyAI.target.position + Random.insideUnitSphere * 10;
			pointInRadius.y += 3;
			if(!Physics.Raycast(enemyAI.transform.position, (pointInRadius - enemyAI.transform.position).normalized))
			{
				pointInRoom = true;
			}
		}
		
		RaycastHit hit;
		Physics.Raycast(pointInRadius, Vector3.down, out hit);
		if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 1, NavMesh.AllAreas))
		{
			agent.SetDestination(hit.point);
		}
	}
}

