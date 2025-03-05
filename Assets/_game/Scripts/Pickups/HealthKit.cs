using UnityEngine;

public class HealthKit : Pickup
{
	[SerializeField] float healthAmount = 50f;
	public override void OnPickupBot(GameObject gameObject)
	{
		if(gameObject.TryGetComponent(out PlayerStats player))
		{
			player.AddHealth(healthAmount);
		}
	}

	public override void OnPickupPlayer(GameObject gameObject)
	{
		if(gameObject.TryGetComponent(out PlayerStats player))
		{
			player.AddHealth(healthAmount);
		}
	}
}
