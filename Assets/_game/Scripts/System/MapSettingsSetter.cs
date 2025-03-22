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
        private MapSettings _mapSettings = new MapSettings();

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
        public int MapPreset = 0;
        public TMP_Dropdown MapPreset_Dropdown;
        public int GameMode = 0;
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

        private void OnDisable() { PlayerPrefs.SetString("MapSettings", JsonUtility.ToJson(_mapSettings)); }

        //check if changespeed of maze change has effect

        //set bot difficulty

        //set gamemode


        public Image ShowFogColor_Image;
        public void ShowFogColorImage() { ShowFogColor_Image.color = _mapSettings.FogColor; }

        public void SetMapPreset(int index)
        {
            MapPreset = index;
            //add ambiant music when ready
            //also add object probabilities based on locations
            switch (index)
            {
                case 0: //stone hall
                    TextureIndex_Dropdown.value = 0;
                    SkyboxIndex_Dropdown.value = 0;
                    AmbientParticleIndex_Dropdown.value = 1;
                    Ambiance_Dropdown.value = 1;
                    Fog_Toggle.isOn = false;

                    break;
                case 1: // snowy
                    TextureIndex_Dropdown.value = 1;
                    SkyboxIndex_Dropdown.value = 7;
                    AmbientParticleIndex_Dropdown.value = 3;
                    Ambiance_Dropdown.value = 2;
                    Fog_Toggle.isOn = false;

                    break;
                case 2: // labs
                    TextureIndex_Dropdown.value = 2;
                    SkyboxIndex_Dropdown.value = 2;
                    AmbientParticleIndex_Dropdown.value = 1;
                    Ambiance_Dropdown.value = 1;
                    Fog_Toggle.isOn = true;
                    FogStrength_Slider.value = 70;

                    break;
                case 3: //elite corridor
                    TextureIndex_Dropdown.value = 3;
                    SkyboxIndex_Dropdown.value = 6;
                    AmbientParticleIndex_Dropdown.value = 0;
                    Ambiance_Dropdown.value = 4;
                    Fog_Toggle.isOn = false;

                    break;
                case 4: //outside niceday
                    TextureIndex_Dropdown.value = 4;
                    SkyboxIndex_Dropdown.value = 4;
                    AmbientParticleIndex_Dropdown.value = 0;
                    Ambiance_Dropdown.value = 3;
                    Fog_Toggle.isOn = false;

                    break;
                case 5: //outside badday
                    TextureIndex_Dropdown.value = 4;
                    SkyboxIndex_Dropdown.value = 2;
                    AmbientParticleIndex_Dropdown.value = 2;
                    Ambiance_Dropdown.value = 7;
                    Fog_Toggle.isOn = false;

                    break;
                case 6: // metal
                    TextureIndex_Dropdown.value = 5;
                    SkyboxIndex_Dropdown.value = 5;
                    AmbientParticleIndex_Dropdown.value = 4;
                    Ambiance_Dropdown.value = 9;
                    Fog_Toggle.isOn = false;

                    break;
                case 7: // hell
                    TextureIndex_Dropdown.value = 6;
                    SkyboxIndex_Dropdown.value = 1;
                    AmbientParticleIndex_Dropdown.value = 4;
                    Ambiance_Dropdown.value = 8;
                    Fog_Toggle.isOn = false;

                    break;
            }
        }

        public void SetGameMode(int value)
        {
            _mapSettings.GameMode = value;
            string objective = "<b>Objective</b>\n";
            switch (value)
            {
                case 0: // classic timed dm
                    GameMode_Classic();
                    objective += "Kill as many enemies as possible within time limit in ever changing maze";
                    break;

                case 1: // floor is lava
                    GameMode_FloorIsLava();
                    objective += "Kill as many enemies as possible within time limit in ever changing maze. The Floor is Lava!";
                    break;

                default:
                    GameMode_Classic();
                    objective += "Kill as many enemies as possible within time limit in ever changing maze";
                    break;
            }

            ObjectiveInfo_text.text = objective;
        }

        void GameMode_FloorIsLava()
        {
            Wall_prob_Slider.value = 60;
            FullWall_prob_Slider.value = 10;
            CrouchSpace_Prob_Slider.value = 0;
            GrapplingHook_Prob_Slider.value = 30;
            DestructableWall_Prob_Slider.value = 5;
            GlassWall_prob_Slider.value = 15;

            Floor_prob_Slider.value = 40;
            FullFloor_prob_Slider.value = 5;
            AcidFloor_prob_Slider.value = 0;
            LavaFloor_prob_Slider.value = 30;
            DestructableFloor_prob_Slider.value = 2;
            GlassFloor_prob_Slider.value = 3;

            Gravity_Slider.value = -5;
        }
        void GameMode_Classic()
        {
            Wall_prob_Slider.value = 60;
            FullWall_prob_Slider.value = 10;
            CrouchSpace_Prob_Slider.value = 0;
            GrapplingHook_Prob_Slider.value = 30;
            DestructableWall_Prob_Slider.value = 5;
            GlassWall_prob_Slider.value = 15;

            Floor_prob_Slider.value = 40;
            FullFloor_prob_Slider.value = 5;
            AcidFloor_prob_Slider.value = 0;
            LavaFloor_prob_Slider.value = 30;
            DestructableFloor_prob_Slider.value = 2;
            GlassFloor_prob_Slider.value = 3;

            Gravity_Slider.value = -9.81f;
        }

    }
}
