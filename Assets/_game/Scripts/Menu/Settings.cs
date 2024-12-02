using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using Unity.AppUI.UI;

public class Settings : MonoBehaviour
{
	[SerializeField] GameObject[] optionTabs;
	
	float masterVolume = 1;
	Bus masterBus;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		masterBus = RuntimeManager.GetBus("bus:/");
	}

	public void SetMasterVolume(float value)
	{
		masterVolume = value;
		masterBus.setVolume(masterVolume);
	}
	
	public void SetTab(int index)
	{
		for (int i = 0; i < optionTabs.Length; i++)
		{
			optionTabs[i].SetActive(i == index);
		}
	}
	
	void OnEnable()
	{
		SetTab(0);
	}
}
