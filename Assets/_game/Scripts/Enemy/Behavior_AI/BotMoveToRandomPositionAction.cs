using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Bot_MoveToRandomPosition", story: "Move [bot] to random position", category: "Action", id: "28547810dd2cff9888643b176e543f9a")]
public partial class BotMoveToRandomPositionAction : Action
{
	[SerializeReference] public BlackboardVariable<NavMeshAgent> Bot;

	protected override Status OnStart()
	{
		SetRandomPosition(Bot);
		return Status.Running;
	}

	protected override Status OnUpdate()
	{
		return Status.Success;
	}

	protected override void OnEnd()
	{
	}
	void SetRandomPosition(NavMeshAgent agent)
	{
		Debug.Log("random pos");
		while(true)
		{
			Vector3 pointInRadius = agent.transform.position + UnityEngine.Random.insideUnitSphere * 10;
			pointInRadius.y += 150;
			RaycastHit hit;

			Physics.Raycast(pointInRadius, Vector3.down, out hit);
			if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 1, NavMesh.AllAreas))
			{
				agent.SetDestination(hit.point);
				return;
			}
		}
		
	}
}

