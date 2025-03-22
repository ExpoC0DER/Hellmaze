using _game.Scripts.Definitions;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace _game.Scripts.UI
{
    public class AudioSettingsSetter : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] optionTabs;
        public int MusicIndex = 1;

        private float masterVolume = 1;
        private float musicVolume = 1;
        private float sfxVolume = 1;
        private float ambientVolume = 1;
        private Bus masterBus;
        private Bus ambientBus;
        private Bus sfxBus;
        private Bus musicBus;



        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
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
            for(int i = 0; i < optionTabs.Length; i++)
            {
                optionTabs[i].SetActive(i == index);
            }
        }

        public void SetMusicIndex(int value)
        {
            MusicIndex = value;
            PlayerPrefs.GetInt("MusicIndex", value);
            UIEvents.OnMusicIndexChanged?.Invoke(MusicIndex);
        }

        private void OnEnable() { SetTab(0); }
    }
}
