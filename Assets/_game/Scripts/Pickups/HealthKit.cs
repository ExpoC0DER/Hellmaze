using UnityEngine;

public class HealthKit : Pickup
{
	[SerializeField] float healthAmount = 50f;
	public override void OnPickupBot(GameObject gameObject)
	{
		if(gameObject.TryGetComponent(out EnemyAI enemy))
		{
			enemy.AddHealth(healthAmount);
		}
	}

	public override void OnPickupPlayer(GameObject gameObject)
	{
		if(gameObject.TryGetComponent(out PlayerStats player))
		{
			player.ChangeHealth(healthAmount);
		}
	}
}
