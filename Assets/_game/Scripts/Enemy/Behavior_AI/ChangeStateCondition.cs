using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "ChangeState", story: "[Current] state has changed", category: "Variable Conditions", id: "e7ec2646055a28b05723b843d7ac5935")]
public partial class ChangeStateCondition : Condition
{
	[SerializeReference] public BlackboardVariable<BotBehaviorState> Current;
	BotBehaviorState currentState;
	
	public override bool IsTrue()
	{
		return currentState != Current;
	}

	public override void OnStart()
	{
		currentState = Current;
	}

	public override void OnEnd()
	{
	}
}
