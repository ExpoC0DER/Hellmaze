using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KillFeedLog : MonoBehaviour
{
	public RectTransform rect;
	public TextMeshProUGUI killerName_txt;
	public TextMeshProUGUI victimName_txt;
	public Image killIcon;
	
	public void Setup(string killerName, string victimName, Sprite icon)
	{
		killerName_txt.text = killerName;
		victimName_txt.text = victimName;
		killIcon.sprite = icon;
		ResizeRect(icon);
		StopAllCoroutines();
		StartCoroutine(TurnOff());
	}
	
	IEnumerator TurnOff()
	{
		yield return new WaitForSeconds(4);
		gameObject.SetActive(false);
	}
	
	void ResizeRect(Sprite sprite)
	{
		float w = sprite.textureRect.width;
		float h = sprite.textureRect.height;
		float aspectRatio = w / h;
		float heigth = 50;
		float width = heigth * aspectRatio;
	    rect.sizeDelta = new Vector2(width, heigth);
	    
	}
	
}
