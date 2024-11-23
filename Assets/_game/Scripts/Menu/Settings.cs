using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class Settings : MonoBehaviour
{
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
}
