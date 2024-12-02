using UnityEngine.UI;
using UnityEngine;
using System;

public class KillFeed : MonoBehaviour
{
	[SerializeField] private Sprite[] weaponIcons;
	[SerializeField] private Sprite suicideIcon;
	[SerializeField] private KillFeedLog[] killLogs;
	int currentLog_index = 0;
	
	
	public void AddKillLog(string killerName, int weaponIndex, string victimName)
	{
		KillFeedLog killLog = killLogs[currentLog_index];
		Sprite weaponIcon;
		
		if(killerName == "")
		{
			weaponIcon = suicideIcon;
		}else
		{
			weaponIcon = weaponIcons[weaponIndex];
		}
		killLog.gameObject.SetActive(true);
		killLog.Setup(killerName, victimName, weaponIcon);
		
		killLog.transform.SetSiblingIndex(killLogs.Length - 1);
		
		currentLog_index++;
		if(currentLog_index >= weaponIcons.Length) currentLog_index = 0;
	}
	
	private void OnEnable()
	{
		PlayerStats.UpdateKillFeed += AddKillLog;
	}
	
	private void OnDisable()
	{
		PlayerStats.UpdateKillFeed -= AddKillLog;
	}
}


