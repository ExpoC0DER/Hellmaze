using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
//[RequireComponent(typeof(AnimationController))] 			//ADD ANIMATION CONTROLLER
public class GoapAgent : MonoBehaviour
{
	[Header("Sensors")]
	[SerializeField] Sensor chaseSensor;
	[SerializeField] Sensor attackSensor;
	
	[Header("Known Locations")]
	[SerializeField] Transform restingPosition;
	[SerializeField] Transform foodShack;
	[SerializeField] Transform doorOnePosition;
	[SerializeField] Transform doorTwoPosition;
	
	NavMeshAgent navMeshAgent;
	//AnimationController animations;			//ADD ANIMATION CONTROLLER
	Rigidbody rb;
	
	[Header("Stats")]
	public float health = 100;
	public float stamina = 100;
	
	CountdownTimer statsTimer;
	
	GameObject target;
	Vector3 destination;
	
	AgentGoal lastGoal;
	
	public AgentGoal currentGoal;
	public ActionPlan actionPlan;
	public AgentAction currentAction;
	
	public Dictionary<string, AgentBelief> beliefs;
	public HashSet<AgentAction> actions;
	public HashSet<AgentGoal> goals;
	
	IGoapPlanner gPlanner;
	
	void Awake()
	{
		navMeshAgent = GetComponent<NavMeshAgent>();
		//animations = GetComponent<AnimationController>();			//ADD ANIMATION CONTROLLER
		rb = GetComponent<Rigidbody>();
		rb.freezeRotation = true;
		
		gPlanner = new GoapPlanner();
	}
	
	void Start()
	{
		SetupTimers();
		SetupBeliefs();
		SetupActions();
		SetupGoals();
	}
	
	void SetupBeliefs()
	{
		beliefs = new Dictionary<string, AgentBelief>();
		BeliefFactory factory = new BeliefFactory(this, beliefs);
		
		factory.AddBelief("Nothing", () => false);
		
		factory.AddBelief("AgentIdle", () => !navMeshAgent.hasPath);
		factory.AddBelief("AgentMoving", () => navMeshAgent.hasPath);
	}
	
	void SetupActions()
	{
		actions = new HashSet<AgentAction>();
		
		actions.Add(new AgentAction.Builder("Relax")
			.WithStrategy(new IdleStrategy(5))
			.AddEffect(beliefs["Nothing"])
			.Build());
			
		actions.Add(new AgentAction.Builder("Wander Around")
			.WithStrategy(new WanderStrategy(navMeshAgent, 10))
			.AddEffect(beliefs["AgentMoving"])
			.Build());
	}
	
	void SetupGoals()
	{
		goals = new HashSet<AgentGoal>();
		goals.Add(new AgentGoal.Builder("ChillOut")
			.WithPriority(1)
			.WithDesiredEffect(beliefs["Nothing"])
			.Build());
		
		goals.Add(new AgentGoal.Builder("Wander")
			.WithPriority(1)
			.WithDesiredEffect(beliefs["AgentMoving"])
			.Build());
	}
	
	void SetupTimers()
	{
		statsTimer = new CountdownTimer(2f);
		statsTimer.OnTimerStop += () => 
		{
			UpdateStats();
			statsTimer.Start();
		};
		statsTimer.Start();
	}
	
	//TODO movestats system
	void UpdateStats()
	{
		//change these based on your bot needs
		stamina += InRangeOf(restingPosition.position, 3f) ? 20 : -10;
		health += InRangeOf(foodShack.position, 3f) ? 20 : -5;
		stamina = Mathf.Clamp(stamina, 0, 100);
		health = Mathf.Clamp(health, 0, 100);
	}
	
	bool InRangeOf(Vector3 pos, float range) => Vector3.Distance(transform.position, pos) < range;
	
	void OnEnable() => chaseSensor.OnTargetChanged += HandleTargetChanged;
	void OnDisable() => chaseSensor.OnTargetChanged -= HandleTargetChanged;
	
	void HandleTargetChanged()
	{
		Debug.Log("Target changed, clearing current action and goal");
		currentAction = null;
		currentGoal = null;
	}
	
	void Update()
	{
		statsTimer.Tick(Time.deltaTime);
		//animations.SetSpeed(navMeshAgent.velocity.magnitude); 			//ADD ANIMATION CONTROLLER
		
		//Update plan and current action if there is one
		if(currentAction != null)
		{
			Debug.Log("Calculating any potential new plan");
			CalculatePlan();
			
			if(actionPlan != null && actionPlan.Actions.Count > 0)
			{
				navMeshAgent.ResetPath();
				
				currentGoal = actionPlan.AgentGoal;
				currentAction = actionPlan.Actions.Pop();
				currentAction.Start();
				
				Debug.Log($"Goal: {currentGoal.Name} with {actionPlan.Actions.Count} actions in plan");
				Debug.Log($"Popped action: {currentAction.Name}");
			}
		}
		
		//if we have current action execute it
		if(actionPlan != null && currentAction != null)
		{
			currentAction.Update(Time.deltaTime);
			if(currentAction.Complete)
			{
				Debug.Log($"{currentAction.Name} complete");
				currentAction.Stop();
				currentAction = null;
				
				if(actionPlan.Actions.Count == 0)
				{
					Debug.Log("Plan Complete");
					lastGoal = currentGoal;
					currentGoal = null;
				}
			}
		}
	}
	
	void CalculatePlan()
	{
		var priorityLevel = currentGoal?.Priority ?? 0;
		
		HashSet<AgentGoal> goalsToCheck = goals;
		
		//if we have a current goal, we only want to check goals with higher priority
		if(currentGoal != null)
		{
			Debug.Log("Current goal exists, checking goals with higher priority");
			goalsToCheck = new HashSet<AgentGoal>(goals.Where(g => g.Priority > priorityLevel));
		}
		
		var potentialPlan = gPlanner.Plan(this, goalsToCheck, lastGoal);
		if(potentialPlan != null)
		{
			actionPlan = potentialPlan;
		}
	}
	
}
