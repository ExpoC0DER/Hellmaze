using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

public class MusicInMatch : MonoBehaviour
{
	public bool isAmbiance = false;
	public EventReference[] musics;// = "event:/Music/Background"; // Set this to your music event path

	bool isPlaing = false;
	private EventInstance musicInstance;
	
	IEnumerator Start()
	{
		
		yield return new WaitUntil(() => Menu.main != null);
		if(!isAmbiance) Menu.main.clientSettings.OnUpdate_MusicIndex += UpdateMusic;
		
		int musicIndex = 0;
		if(isAmbiance) musicIndex = Menu.main.mapSettings.Ambiance -1;
		else musicIndex = Menu.main.clientSettings.MusicIndex -1;
		
		if(isPlaing)
		{
			musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			musicInstance.release();
			isPlaing = false;
		}
		if(musicIndex == -1)
		{
			yield break;
		}
		
		musicInstance = RuntimeManager.CreateInstance(musics[musicIndex]);

		// Start playing the music
		musicInstance.start();

		// Ensure music plays on loop
		musicInstance.setParameterByName("loop", 1); // Assumes you've set a parameter for looping in FMOD Studio
		isPlaing = true;
	}
	
	void UpdateMusic()
	{
		StartCoroutine(Start());
	}
	
	private void OnDestroy()
	{
		// Stop the music and release the instance when the object is destroyed
		if(!isAmbiance) Menu.main.clientSettings.OnUpdate_MusicIndex -= UpdateMusic;
		musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		musicInstance.release();
	}
}
