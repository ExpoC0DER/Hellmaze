using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using Unity.AppUI.UI;
using System;

public class Settings : MonoBehaviour
{
	[SerializeField] GameObject[] optionTabs;
	
	float masterVolume = 1;
	float musicVolume = 1;
	float sfxVolume = 1;
	float ambientVolume = 1;
	Bus masterBus;
	Bus ambientBus;
	Bus sfxBus;
	Bus musicBus;
	
	
	
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		masterBus = RuntimeManager.GetBus("bus:/");
		ambientBus = RuntimeManager.GetBus("bus:/Ambiance");
		sfxBus = RuntimeManager.GetBus("bus:/Sfx");
		musicBus = RuntimeManager.GetBus("bus:/Music");
	}

	public void SetMasterVolume(float value)
	{
		masterVolume = value;
		masterBus.setVolume(masterVolume);
	}
	public void SetMusicVolume(float value)
	{
		musicVolume = value;
		musicBus.setVolume(musicVolume);
	}
	public void SetSFXVolume(float value)
	{
		sfxVolume = value;
		sfxBus.setVolume(sfxVolume);
	}
	public void SetAmbientVolume(float value)
	{
		ambientVolume = value;
		ambientBus.setVolume(ambientVolume);
	}
	
	public void SetTab(int index)
	{
		for (int i = 0; i < optionTabs.Length; i++)
		{
			optionTabs[i].SetActive(i == index);
		}
	}
	
	public int MusicIndex = 1;
	public event Action OnUpdate_MusicIndex;
	public void SetMusicIndex(int value)
	{
		MusicIndex = value;
		OnUpdate_MusicIndex?.Invoke();
	}
	
	void OnEnable()
	{
		SetTab(0);
	}
}
