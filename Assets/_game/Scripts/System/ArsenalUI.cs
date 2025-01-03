using _game.Scripts;
using UnityEngine;

public class ArsenalUI : MonoBehaviour
{
	[SerializeField] ArsenalSlot[] arsenalSlots;
	[SerializeField] WeaponSlots weaponSlots;
	
	[SerializeField] GameObject slotsObj;
	
	float turnOffTime = 0.7f;
	float cd = 0;
	
	private void OnEnable()
	{
		weaponSlots.OnWeaponChange += RefreshArsenalUI;
		cd = 0.1f;
	}
	
	private void OnDisable()
	{
		weaponSlots.OnWeaponChange -= RefreshArsenalUI;
	}
	
	private void Update() {
		if(cd >= 0)
		{
			cd -= Time.deltaTime;
			if(cd <= 0)
			{
				slotsObj.SetActive(false);
			}
		}
	}
	
	void RefreshArsenalUI(int index)
	{
		slotsObj.SetActive(true);
		cd = turnOffTime;
		bool[] gunsThatHaveAmmo = weaponSlots.GunsThatHaveAmmo();
		bool alreadySelected = false;
		int pickedColumn = -1;
		for (int i = 0; i < arsenalSlots.Length; i++)
		{
			arsenalSlots[i].SetAmmoGraphics(gunsThatHaveAmmo[i]);
			arsenalSlots[i].Picked(index == i, alreadySelected, out int typeColumn);
			if(i == index)
			{
				alreadySelected = true;
				pickedColumn = typeColumn;
			}
			if(typeColumn != pickedColumn && alreadySelected)
			{
				alreadySelected = false;
				arsenalSlots[i].Picked(index == i, alreadySelected, out int c);
			}
		}
	}
}
