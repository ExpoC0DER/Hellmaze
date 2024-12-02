using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KillFeedLog : MonoBehaviour
{
	public TextMeshProUGUI killerName_txt;
	public TextMeshProUGUI victimName_txt;
	public Image killIcon;
	
	public void Setup(string killerName, string victimName, Sprite icon)
	{
		killerName_txt.text = killerName;
		victimName_txt.text = victimName;
		killIcon.sprite = icon;
		StopAllCoroutines();
		StartCoroutine(TurnOff());
	}
	
	IEnumerator TurnOff()
	{
		yield return new WaitForSeconds(4);
		gameObject.SetActive(false);
	}
	
}
