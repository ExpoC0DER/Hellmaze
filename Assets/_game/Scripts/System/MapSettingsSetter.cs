using System;
using _game.Scripts.Definitions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _game.Scripts.System
{
    public class MapSettingsSetter : MonoBehaviour
    {
        [Header("Maze")]
        private readonly MapSettings _mapSettings = new MapSettings();

        public void SetMapSize(float value) => _mapSettings.MazeSize = (int)value;
        public Slider MapSize_Slider;
        public void SetMapFloors(float value) => _mapSettings.Floors = (int)value;
        public Slider Floors_Slider;

        public void SetWallChangeSpeed(float value) => _mapSettings.WallChangeSpeed = (int)value;
        public Slider WallChangeSpeed_Slider;

        public void SetFloorChangeSpeed(float value) => _mapSettings.FloorChangeSpeed = (int)value;
        public Slider FloorChangeSpeed_Slider;

        /* [Header("GameMode")]
    public bool[] GameModes;
    public void SetGameModes(int index, bool setUp) => GameModes[index] = setUp;
 */
        public void SetBotCount(float value) => _mapSettings.BotCount = (int)value;
        [Header("Bots")]
        public Slider BotCount_Slider;

        public void SetBotDifficulty(int value) => _mapSettings.BotDifficulty = value;
        public TMP_Dropdown BotDifficulty_Dropdown;

        [Header("Preset")]
        public TMP_Dropdown MapPreset_Dropdown;
        public TMP_Dropdown GameMode_Dropdown;
        public TextMeshProUGUI ObjectiveInfo_text;

        [Header("Custom")]
        [Header("Items & Objects")]
        public Slider Pickup_Count_Slider;
        public void SetPickupCount(float value) => _mapSettings.PickupCount = (int)value;

        public void SetHealthPickupCount(float value) => _mapSettings.HealthPickupCount = (int)value;
        public Slider HealthPickup_Count_Slider;

        public void SetPistolPickupCount(float value) => _mapSettings.PistolPickupCount = (int)value;
        public Slider PistolPickup_Count_Slider;

        public void SetRevolverPickupCount(float value) => _mapSettings.RevolverPickupCount = (int)value;
        public Slider RevolverPickup_Count_Slider;

        public void SetAkPickupCount(float value) => _mapSettings.AkPickupCount = (int)value;
        public Slider AkPickup_Count_Slider;

        public void SetMinigunPickupCount(float value) => _mapSettings.MinigunPickupCount = (int)value;
        public Slider MinigunPickup_Count_Slider;

        public void SetShotgunPickupCount(float value) => _mapSettings.ShotgunPickupCount = (int)value;
        public Slider ShotgunPickup_Count_Slider;

        public void SetSuperShotgunPickupCount(float value) => _mapSettings.SupershotgunPickupCount = (int)value;
        public Slider SupershotgunPickup_Count_Slider;

        public void SetSniperRiflePickupCount(float value) => _mapSettings.SniperRiflePickupCount = (int)value;
        public Slider SniperRiflePickup_Count_Slider;

        public void SetRPGPickupCount(float value) => _mapSettings.RpgPickupCount = (int)value;
        public Slider RPGPickup_Count_Slider;

        public void SetGrenadeLauncherPickupCount(float value) => _mapSettings.GrenadeLauncherPickupCount = (int)value;
        public Slider GrenadeLauncherPickup_Count_Slider;

        public void SetStickybombLauncherPickupCount(float value) => _mapSettings.StickybombLauncherPickupCount = (int)value;
        public Slider StickybombLauncherPickup_Count_Slider;

        public void SetLazergunPickupCount(float value) => _mapSettings.LazergunPickupCount = (int)value;
        public Slider LazergunPickup_Count_Slider;

        public void SetAcidBlasterPickupCount(float value) => _mapSettings.AcidBlasterPickupCount = (int)value;
        public Slider AcidBlasterPickup_Count_Slider;

        public void SetPlasmagunPickupCount(float value) => _mapSettings.PlasmagunPickupCount = (int)value;
        public Slider PlasmagunPickup_Count_Slider;

        public void SetDroneLauncherPickupCount(float value) => _mapSettings.DroneLauncherPickupCount = (int)value;
        public Slider DroneLauncherPickup_Count_Slider;

        public void SetObjectProb(float value) => _mapSettings.ObjectProb = (int)value;
        public Slider Object_Prob_Slider;

        public void SetExplosiveBarrelProb(float value) => _mapSettings.ExplosiveBarrelProb = (int)value;
        public Slider ExplosiveBarrel_Prob_Slider;

        public void SetStomperProb(float value) => _mapSettings.StomperProb = (int)value;
        public Slider Stomper_Prob_Slider;

        public void SetStairsProb(float value) => _mapSettings.StairsProb = (int)value;
        public Slider Stairs_Prob_Slider;

        public void SetJumppadProb(float value) => _mapSettings.JumppadProb = (int)value;
        public Slider Jumppad_Prob_Slider;

        public void SetChandelierProb(float value) => _mapSettings.ChandelierProb = (int)value;
        public Slider Chandelier_Prob_Slider;

        public void SetHangPianoProb(float value) => _mapSettings.HangPianoProb = (int)value;
        public Slider HangPiano_Prob_Slider;

        public void SetAcidTubeProb(float value) => _mapSettings.AcidTubeProb = (int)value;
        public Slider AcidTube_Prob_Slider;

        public void SetBunkerLihtProb(float value) => _mapSettings.BunkerLihtProb = (int)value;
        public Slider BunkerLiht_Prob_Slider;

        public void SetElectricboxProb(float value) => _mapSettings.ElectricboxProb = (int)value;
        public Slider Electricbox_Prob_Slider;

        public void SetGrapplingHookProb(float value) => _mapSettings.GrapplingHookProb = (int)value;
        public Slider GrapplingHook_Prob_Slider;

        public void SetCrouchSpaceProb(float value) => _mapSettings.CrouchSpaceProb = (int)value;
        public Slider CrouchSpace_Prob_Slider;

        public void SetDestructableWallProb(float value) => _mapSettings.DestructableWallProb = (int)value;
        public Slider DestructableWall_Prob_Slider;

        public void SetGlassWallProb(float value) => _mapSettings.GlassWallProb = (int)value;
        public Slider GlassWall_prob_Slider;

        public void SetWallProb(float value) => _mapSettings.WallProb = (int)value;
        public Slider Wall_prob_Slider;

        public void SetFullWallProb(float value) => _mapSettings.FullWallProb = (int)value;
        public Slider FullWall_prob_Slider;

        public void SetLavaFloorProb(float value) => _mapSettings.LavaFloorProb = (int)value;
        public Slider LavaFloor_prob_Slider;

        public void SetAcidFloorProb(float value) => _mapSettings.AcidFloorProb = (int)value;
        public Slider AcidFloor_prob_Slider;

        public void SetGlassFloorProb(float value) => _mapSettings.GlassFloorProb = (int)value;
        public Slider GlassFloor_prob_Slider;

        public void SetDestructableFloorProb(float value) => _mapSettings.DestructableFloorProb = (int)value;
        public Slider DestructableFloor_prob_Slider;

        public void SetFloorProb(float value) => _mapSettings.FloorProb = (int)value;
        public Slider Floor_prob_Slider;

        public void SetFullFloorProb(float value) => _mapSettings.FullFloorProb = (int)value;
        public Slider FullFloor_prob_Slider;

        public void SetAmbianceIndex(int value) => _mapSettings.Ambiance = value;
        [Header("Audio")]
        public TMP_Dropdown Ambiance_Dropdown;

        public void SetTexturePreset(int value) => _mapSettings.TextureIndex = value;
        [Header("Miscellaneous")]
        public TMP_Dropdown TextureIndex_Dropdown;

        public void SetSkyboxIndex(int value) => _mapSettings.SkyboxIndex = value;
        public TMP_Dropdown SkyboxIndex_Dropdown;

        public void SetAmbientParticleIndex(int value) => _mapSettings.AmbientParticleIndex = value;
        public TMP_Dropdown AmbientParticleIndex_Dropdown;

        public void SetGravity(float value) => _mapSettings.Gravity = (int)value;
        public Slider Gravity_Slider;

        public void SetFog(bool value) => _mapSettings.Fog = value;
        public Toggle Fog_Toggle;

        public void SetFogStrength(float value) => _mapSettings.FogStrength = value;
        public Slider FogStrength_Slider;

        public void SetFogColorRed(float value) => _mapSettings.SetFogColor(r: value);
        public Slider FogColor_r_Slider;

        public void SetFogColoGreen(float value) => _mapSettings.SetFogColor(g: value);
        public Slider FogColor_g_Slider;

        public void SetFogColorBlue(float value) => _mapSettings.SetFogColor(b: value);
        public Slider FogColor_b_Slider;

        public Image ShowFogColor_Image;
        public void ShowFogColorImage() { ShowFogColor_Image.color = _mapSettings.FogColor; }

        [Header("Visual Presets")]
        [SerializeField] private MapPreset _stoneHallVisuals;
        [SerializeField] private MapPreset _snowyVisuals;
        [SerializeField] private MapPreset _labsVisuals;
        [SerializeField] private MapPreset _eliteCorridorVisuals;
        [SerializeField] private MapPreset _outsideNiceDayVisuals;
        [SerializeField] private MapPreset _outsideBadDayVisuals;
        [SerializeField] private MapPreset _metalVisuals;
        [SerializeField] private MapPreset _hellVisuals;

        [Header("GameMode Presets")]
        [SerializeField] private MapPreset _classic;
        [SerializeField] private MapPreset _floorIsLava;


        private void OnDisable() { PlayerPrefs.SetString("MapSettings", JsonUtility.ToJson(_mapSettings)); }

        public void SetMapPreset(int index)
        {
            _mapSettings.MapPreset = index;
            //add ambiant music when ready
            //also add object probabilities based on locations
            switch (index)
            {
                case 0: //stone hall
                    SetVisuals(_stoneHallVisuals);
                    break;
                case 1: // snowy
                    SetVisuals(_snowyVisuals);
                    break;
                case 2: // labs
                    SetVisuals(_labsVisuals);
                    break;
                case 3: //elite corridor
                    SetVisuals(_eliteCorridorVisuals);
                    break;
                case 4: //outside niceday
                    SetVisuals(_outsideNiceDayVisuals);
                    break;
                case 5: //outside badday
                    SetVisuals(_outsideBadDayVisuals);
                    break;
                case 6: // metal
                    SetVisuals(_metalVisuals);
                    break;
                case 7: // hell
                    SetVisuals(_hellVisuals);
                    break;
            }
        }
        
        public void SetGameMode(int value)
        {
            _mapSettings.GameMode = value;
            switch (value)
            {
                case 0: // classic timed dm
                    SetGameModePreset(_classic);
                    break;

                case 1: // floor is lava
                    SetGameModePreset(_floorIsLava);
                    break;

                default:
                    SetGameModePreset(_classic);
                    break;
            }
        }
        
        private void SetVisuals(MapPreset visualPreset)
        {
            TextureIndex_Dropdown.value = visualPreset.MapSettings.TextureIndex;
            SkyboxIndex_Dropdown.value = visualPreset.MapSettings.SkyboxIndex;
            AmbientParticleIndex_Dropdown.value = visualPreset.MapSettings.AmbientParticleIndex;
            Ambiance_Dropdown.value = visualPreset.MapSettings.Ambiance;
            Fog_Toggle.isOn = visualPreset.MapSettings.Fog;
            FogStrength_Slider.value = visualPreset.MapSettings.FogStrength;
        }

        private void SetGameModePreset(MapPreset gameModePreset)
        {
            Wall_prob_Slider.value = gameModePreset.MapSettings.WallProb;
            FullWall_prob_Slider.value = gameModePreset.MapSettings.FullWallProb;
            CrouchSpace_Prob_Slider.value = gameModePreset.MapSettings.CrouchSpaceProb;
            GrapplingHook_Prob_Slider.value = gameModePreset.MapSettings.GrapplingHookProb;
            DestructableWall_Prob_Slider.value = gameModePreset.MapSettings.DestructableWallProb;
            GlassWall_prob_Slider.value = gameModePreset.MapSettings.GlassWallProb;

            Floor_prob_Slider.value = gameModePreset.MapSettings.FloorProb;
            FullFloor_prob_Slider.value = gameModePreset.MapSettings.FullFloorProb;
            AcidFloor_prob_Slider.value = gameModePreset.MapSettings.AcidFloorProb;
            LavaFloor_prob_Slider.value = gameModePreset.MapSettings.LavaFloorProb;
            DestructableFloor_prob_Slider.value = gameModePreset.MapSettings.DestructableFloorProb;
            GlassFloor_prob_Slider.value = gameModePreset.MapSettings.GlassFloorProb;

            Gravity_Slider.value = gameModePreset.MapSettings.Gravity;
            
            ObjectiveInfo_text.text = gameModePreset.MapSettings.Objective;
        }
    }
}
