using UnityEngine;

public class WeaponPickup : Pickup
{
	[SerializeField] int weaponIndex = 1;
	[SerializeField] int ammo = 1;
	public override void OnPickupBot(GameObject gameObject)
	{
		if(gameObject.TryGetComponent(out WeaponSlots weapons))
		{
			weapons.PickWeapon(weaponIndex, ammo);
		}
	}

	public override void OnPickupPlayer(GameObject gameObject)
	{
		if(gameObject.TryGetComponent(out WeaponSlots weapons))
		{
			weapons.PickWeapon(weaponIndex, ammo);
		}
	}
}
