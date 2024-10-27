using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class BackgroundMusic : MonoBehaviour
{
    [EventRef]
    public string backgroundMusicEvent = "event:/Music/Background"; // Set this to your music event path

    private EventInstance musicInstance;

    void Start()
    {
        // Initialize the music instance from the FMOD event
        musicInstance = RuntimeManager.CreateInstance(backgroundMusicEvent);

        // Start playing the music
        musicInstance.start();

        // Ensure music plays on loop
        musicInstance.setParameterByName("loop", 1); // Assumes you've set a parameter for looping in FMOD Studio
    }

    private void OnDestroy()
    {
        // Stop the music and release the instance when the object is destroyed
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
    }
}
