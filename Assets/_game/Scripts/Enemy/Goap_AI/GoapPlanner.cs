using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

public interface IGoapPlanner
{
	ActionPlan Plan(GoapAgent agent, HashSet<AgentGoal> goals, AgentGoal mostRecentGoal = null);
}

public class GoapPlanner : IGoapPlanner
{
	public ActionPlan Plan(GoapAgent agent, HashSet<AgentGoal> goals, AgentGoal mostRecentGoal = null)
	{
		//order goals by priority, descending
		List<AgentGoal> orderGoals = goals
			.Where(g => g.DesiredEffects.Any(b => !b.Evaluate()))
			.OrderByDescending(g => g == mostRecentGoal ? g.Priority - 0.01 : g.Priority)
			.ToList();
		//try solve each goal in order
		foreach (var goal in orderGoals)
		{
			Node goalNode = new Node(null, null, goal.DesiredEffects, 0);
			//if we find a path to the goal, return plan
			if(FindPath(goalNode, agent.actions))
			{
				//if goal has no leaves and no action to perform try a different goal
				if(goalNode.IsLeafDead) continue;
				
				Stack<AgentAction> actionStack = new Stack<AgentAction>();
				while(goalNode.Leaves.Count > 0)
				{
					var cheapestLeaf = goalNode.Leaves.OrderBy(leaf => leaf.Cost).First();
					goalNode = cheapestLeaf;
					actionStack.Push(cheapestLeaf.Action);
				}
				
				return new ActionPlan(goal, actionStack, goalNode.Cost);					
			}
		}
		Debug.LogWarning("No Plan Found");
		return null;
	}
	
	bool FindPath(Node parent, HashSet<AgentAction> actions)
	{
		foreach (var action in actions)
		{
			var requiredEffects = parent.RequiredEffects;
			
			//Remove any effects that evaluate to true, there is no action to take
			requiredEffects.RemoveWhere(b => b.Evaluate());
			
			//if there are no required effects to fulfill, we have a plan
			if(requiredEffects.Count == 0)
			{
				return true;
			}
			
			if(action.Effects.Any(requiredEffects.Contains))
			{
				var newRequiredEffects = new HashSet<AgentBelief>(requiredEffects);
				newRequiredEffects.ExceptWith(action.Effects);
				newRequiredEffects.UnionWith(action.Preconditions);
				
				var newAvailableActions = new HashSet<AgentAction>(actions);
				newAvailableActions.Remove(action);
				
				var newNode = new Node(parent, action, newRequiredEffects, parent.Cost + action.Cost);
				
				//Explore the new node recursively
				if(FindPath(newNode, newAvailableActions))
				{
					parent.Leaves.Add(newNode);
					newRequiredEffects.ExceptWith(newNode.Action.Preconditions);
				}
				//if all efffect at this depth have been satisfied, return true
				if(newRequiredEffects.Count == 0)
				{
					return true;
				}
				
			}
			
		}
		return false;
	}
}

public class Node
{
	public Node Parent {get;}
	public AgentAction Action {get;}
	public HashSet<AgentBelief> RequiredEffects {get;}
	public List<Node> Leaves {get;}
	public float Cost {get;}
	
	public bool IsLeafDead => Leaves.Count == 0 && Action == null;

	public Node(Node parent, AgentAction action, HashSet<AgentBelief> effects, float cost)
	{
		Parent = parent;
		Action = action;
		RequiredEffects = new HashSet<AgentBelief>(effects);
		Leaves = new List<Node>();
		Cost = cost;
	}
}

public class ActionPlan
{
	public AgentGoal AgentGoal {get;}
	public Stack<AgentAction> Actions {get;}
	public float TotalCost {get;set;}
	
	public ActionPlan(AgentGoal goal, Stack<AgentAction> actions, float totalCost)
	{
		AgentGoal = goal;
		Actions = actions;
		TotalCost = totalCost;
	}	
}