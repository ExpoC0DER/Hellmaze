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
	
	[SerializeField] public Animator animator;
	[SerializeField] EnemyVision enemyVision;
	[SerializeField] float fovAngle = 90;

	[SerializeField] private float health = 100;
	[SerializeField] private float maxHealth = 100;
	[SerializeField] ParticleSystem death_part;
	
	public NavMeshAgent navMeshAgent { get; private set; }
	
	public float navigationRadius {get; private set;} = 30;
	
	private bool _dead = false;
	public Transform target {get; private set;}
	public Vector3 lastSeenTargetPos {get; private set;}
	
	public static event Action OnDeath;

	Bot_State currentState;
	public Bot_State_Attacking state_Attacking = new Bot_State_Attacking();
	public Bot_State_Tracking state_Tracking = new Bot_State_Tracking();
	public Bot_State_Roam state_Roam = new Bot_State_Roam();
	

	 private void Start()
	{
		navMeshAgent = GetComponent<NavMeshAgent>();
		currentState = state_Roam;
		currentState.EnterState(this);
	}

	void Update()
	{
		HandleMovement();
		currentState.UpdateState();
	}
	
	public void SwitchState(Bot_State state)
	{
		Debug.Log("switch state to: " + state.ToString());
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
	
	void HandleMovement()
	{
		//if seen interactable object (grappling point, explosive barrel) interact accordingly
		
		//if direction to navmesh target raycast doesnt get blocked on distance of 4 -> high jump probability
		//if distance from target > 0.5 and velocity < 1 (get blocked)
		//		if direction to navmesh target blocked, but from origin of feet not blocked -> crouch
		//if in navmesh target direction we see Grappling hook -> use it
		//										breakable wall / barrel -> shoot it								
	} 
	
	public void TrackTarget(Vector3 position)
	{
		lastSeenTargetPos = position;
		SwitchState(state_Tracking);
	}
	
	public void TakeDamage(float damage)
	{
		health -= damage;
		if(health<=0)
		{
			Respawn();
		}
		if(health < 30)
		{
			FindHealthKit();
		}
		//if player shot and not in attack state 
		//		try find player in whole radius and go attack state if there is any
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
		agent.updateRotation = false;
	}
	public override void UpdateState()
	{
		if(agent.remainingDistance < 0.3f)
		{
			SetRandomRoomPosition();
		}
		
		ShootTarget();
		//higher jump probability
		//
		//upper body is focused on target player (with some inaccuracy) and shoot him
	}
	public override void ExitState()
	{
		agent.updateRotation = true;
	}
	
	void ShootTarget()
	{
		//rotate bot and his spine
		Vector3 direction = enemyAI.target.position - enemyAI.transform.position;
		if (direction.magnitude > 0.3f) // Avoid jitter when close
		{
			Quaternion lookRotation = Quaternion.LookRotation(direction);
			
			float cameraAngle = lookRotation.eulerAngles.x;
			if(cameraAngle > 90 && cameraAngle <= 360) cameraAngle -= 360;
			float camRot = Mathf.InverseLerp(90, -90f, cameraAngle);
			enemyAI.animator.SetFloat("SpineRotation", camRot);
			
			lookRotation.z = 0;
			lookRotation.x = 0;
			enemyAI.transform.rotation = Quaternion.Slerp(enemyAI.transform.rotation, lookRotation, Time.deltaTime * 5); // Smooth rotation
		}else
		{
			enemyAI.animator.SetFloat("SpineRotation", 0.5f);
		}
		
		//shoot raycast from head towards player (with innacuracy) in intervals (based on gun scripts)
	}
	
	void SetRandomRoomPosition()
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

public class Bot_State_FindPickup: Bot_State
{
	EnemyAI enemyAI;
	Transform target;
	bool lowHealth;
	public override void EnterState(EnemyAI ai)
	{
		enemyAI = ai;
	}
	public override void UpdateState()
	{
		//low health?
		//		pick last know healthkit location
		//no ammo?
		//		pick last know ammo location
		
		//check raycast if it is not blocked the target location (bot sees it)
		// if not blocked 
		//		check if it still exists in Vision array (not picked up)
		//			if not pick other location	(repeat)
		
		//if distance from target < 0.2f
		//		check if there is need for health / ammo
		//				if not -> free roam
	}
	public override void ExitState()
	{
		
	}
}
