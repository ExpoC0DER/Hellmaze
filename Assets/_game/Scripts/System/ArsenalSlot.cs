using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ArsenalSlot : MonoBehaviour
{
	[SerializeField] int typeColumn;
	[SerializeField] Image slotImage;
	
	[SerializeField] Sprite selected_sprite;
	[SerializeField] Sprite base_sprite;
	[SerializeField] Color baseColor;
	[SerializeField] Color selectedColor;
	[SerializeField] Color noAmmoColor;
	
	[SerializeField] Vector2 defaultSize;
	Vector2 selectedSize;
	[SerializeField] RectTransform parentRect;
	
	private void Start() {
		//selectedSize = new Vector2(defaultSize.x * 2, defaultSize.y * 2);
	}
	
	public void SetAmmoGraphics(bool hasAmmo)
	{
		if(hasAmmo) slotImage.color = baseColor;
		else slotImage.color = noAmmoColor;
	}
	
	public void Picked(bool picked, bool alreadySelectedColumn, out int pickedColumn)
	{
		/* if(picked) slotImage.sprite = selected_sprite;
		else slotImage.sprite = base_sprite; */
		if(picked)
		{
			//slotImage.rectTransform.sizeDelta = selectedSize;
			//parentRect.sizeDelta = new Vector2(selectedSize.x, parentRect.sizeDelta.y);
			slotImage.color = selectedColor;
		} 
		else
		{
			//slotImage.rectTransform.sizeDelta = defaultSize;
			//if(!alreadySelectedColumn) parentRect.sizeDelta = new Vector2(defaultSize.x, parentRect.sizeDelta.y);
			slotImage.color = baseColor;
		} 
		
		pickedColumn = typeColumn;
	}
}
