using UnityEngine;

public class WeaponPickup : Pickup
{
	public override void OnPickupBot(GameObject gameObject)
	{
		//bot -> add weapon if slot is available, if weapon is in inventory add ammo
	}

	public override void OnPickupPlayer(GameObject gameObject)
	{
		//player -> add weapon if slot is available, if weapon is in inventory add ammo
	}
}
