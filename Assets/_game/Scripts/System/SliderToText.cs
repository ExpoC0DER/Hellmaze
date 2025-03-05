using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderToText : MonoBehaviour
{
	[SerializeField] Slider slider;
	[SerializeField] TextMeshProUGUI text;
	[SerializeField] string extraText = "";
	
	void Start()
	{
		UpdateValue(slider.value);
		slider.onValueChanged.AddListener(delegate {UpdateValue(slider.value);});
	}
	
	void UpdateValue(float value)
	{
		if(slider.wholeNumbers)
			text.text = value.ToString() + extraText;
		else
			text.text = value.ToString("f2") + extraText;
	}
	
	void OnDestroy()
	{
		slider.onValueChanged.RemoveAllListeners();
	}
}
