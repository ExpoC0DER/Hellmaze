using System;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;
using _game.Scripts.Definitions;
using _game.Scripts.UI;

public class MusicInMatch : MonoBehaviour
{
    public bool isAmbiance = false;
    public EventReference[] musics; // = "event:/Music/Background"; // Set this to your music event path

    bool isPlaing = false;
    private EventInstance musicInstance;

    private MapSettings _mapSettings;

    private void Start() { SetMusic(); }

    private void SetMusic()
    {
        string mapSettingsJson = PlayerPrefs.GetString("MapSettings", null);
        _mapSettings = string.IsNullOrEmpty(mapSettingsJson) ? new MapSettings(): JsonUtility.FromJson<MapSettings>(mapSettingsJson);

        AudioSettingsSetter audioSettingsSetter;
        int musicIndex = 0;
        if (isAmbiance)
            musicIndex = _mapSettings.Ambiance - 1;
        else
            musicIndex = PlayerPrefs.GetInt("MusicIndex", 0) - 1;

        if (isPlaing)
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            musicInstance.release();
            isPlaing = false;
        }

        if (musicIndex == -1)
            return;

        musicInstance = RuntimeManager.CreateInstance(musics[musicIndex]);

        // Start playing the music
        musicInstance.start();

        // Ensure music plays on loop
        musicInstance.setParameterByName("loop", 1); // Assumes you've set a parameter for looping in FMOD Studio
        isPlaing = true;
    }

    private void UpdateMusic(int musicIndex) { SetMusic(); }

    private void OnEnable()
    {
        if (!isAmbiance) UIEvents.OnMusicIndexChanged += UpdateMusic;
    }

    private void OnDisable()
    {
        if (!isAmbiance) UIEvents.OnMusicIndexChanged -= UpdateMusic;
    }

    private void OnDestroy()
    {
        // Stop the music and release the instance when the object is destroyed
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
    }
}
