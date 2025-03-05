using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using _game.Scripts;

public class PlayAudioOnCollision : MonoBehaviour
{
	[SerializeField] EventReference sfx;
	[SerializeField] bool playOnce = true;
	
	bool played = false;
	
	public void Reset() => played = false;
	
	void OnTriggerEnter(Collider other)
	{
		if(!other.isTrigger && !played)
		{
			FMODHelper.PlayNewInstance(sfx, transform);
			if(playOnce) played = true;
		} 
	}
}
