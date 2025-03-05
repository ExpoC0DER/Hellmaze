using UnityEngine;
using UnityEngine.UI;

public class SliderHierarchy : MonoBehaviour
{
	[SerializeField] Slider masterSlider;
	[SerializeField] Slider[] subSliders;
	[SerializeField] float masterValueMultiplier = 1;
	
	void OnEnable()
	{
		RecalculateValues_WithRatio();
	}
	
	public void RecalculateValues()
	{
		float availableValue = masterSlider.value * masterValueMultiplier;
		for (int i = 0; i < subSliders.Length; i++)
		{
			Slider slider = subSliders[i];
			//float valueRatio = Mathf.InverseLerp(slider.minValue, slider.maxValue, slider.value);
			slider.maxValue = availableValue;
			//slider.value = Mathf.Lerp(slider.minValue, slider.maxValue, valueRatio);
			availableValue -= slider.value;
			
		}
	}
	
	public void RecalculateValues_WithRatio()
	{
		float availableValue = masterSlider.value * masterValueMultiplier;
		for (int i = 0; i < subSliders.Length; i++)
		{
			Slider slider = subSliders[i];
			float valueRatio = Mathf.InverseLerp(slider.minValue, slider.maxValue, slider.value);
			slider.maxValue = availableValue;
			slider.value = Mathf.Lerp(slider.minValue, slider.maxValue, valueRatio);
			availableValue -= slider.value;
			
		}
	}
	
	float GetValueRatio(float min, float max, float current)
	{
		return Mathf.InverseLerp(min, max, current);
	}
	
	public void MasterSliderUpdated()
	{
		for (int i = 0; i < subSliders.Length; i++)
		{
			subSliders[i].maxValue = masterSlider.value;
		}
		RecalculateValues();
	}
}
