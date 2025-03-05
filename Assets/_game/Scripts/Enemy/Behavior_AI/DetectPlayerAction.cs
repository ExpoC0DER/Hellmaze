using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "DetectPlayer", story: "[Vision] check if players are visible, get closest [player]", category: "Action", id: "fca257dfc073cccc5675c3dbb8360554")]
public partial class DetectPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<EnemyVision> Vision;
    [SerializeReference] public BlackboardVariable<Transform> Player;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

