using System;
using TMPro;
using UnityEngine;

namespace _game.Scripts.Definitions
{
    [Serializable]
    public class MapSettings
    {
        [field: Header("Maze")]
        [field: SerializeField] public int MazeSize { get; set; } = 11;
        [field: SerializeField] public int Floors { get; set; } = 1;
        [field: SerializeField] public float WallChangeSpeed { get; set; } = 0.3f;
        [field: SerializeField] public float FloorChangeSpeed { get; set; } = 1;


        [field: Header("Bots")]
        [field: SerializeField] public int BotCount { get; set; } = 3;
        [field: SerializeField] public int BotDifficulty { get; set; } = 1;


        [field: Header("Preset")]
        [field: SerializeField] public int MapPreset { get; set; } = 0;
        [field: SerializeField] public int GameMode { get; set; } = 0;


        [field: Header("Items & Objects")]
        [field: SerializeField] public float PickupCount { get; set; } = 20;
        [field: SerializeField] public float HealthPickupCount { get; set; } = 6;
        [field: SerializeField] public float PistolPickupCount { get; set; } = 1;
        [field: SerializeField] public float RevolverPickupCount { get; set; } = 1;
        [field: SerializeField] public float AkPickupCount { get; set; } = 1;
        [field: SerializeField] public float MinigunPickupCount { get; set; } = 1;
        [field: SerializeField] public float ShotgunPickupCount { get; set; } = 1;
        [field: SerializeField] public float SupershotgunPickupCount { get; set; } = 1;
        [field: SerializeField] public float SniperRiflePickupCount { get; set; } = 1;
        [field: SerializeField] public float RpgPickupCount { get; set; } = 1;
        [field: SerializeField] public float GrenadeLauncherPickupCount { get; set; } = 1;
        [field: SerializeField] public float StickybombLauncherPickupCount { get; set; } = 1;
        [field: SerializeField] public float LazergunPickupCount { get; set; } = 1;
        [field: SerializeField] public float AcidBlasterPickupCount { get; set; } = 1;
        [field: SerializeField] public float PlasmagunPickupCount { get; set; } = 1;
        [field: SerializeField] public float DroneLauncherPickupCount { get; set; } = 1;
        [field: SerializeField] public float ObjectProb { get; set; } = 70;
        [field: SerializeField] public float ExplosiveBarrelProb { get; set; } = 12;
        [field: SerializeField] public float StomperProb { get; set; } = 7;
        [field: SerializeField] public float StairsProb { get; set; } = 7;
        [field: SerializeField] public float JumppadProb { get; set; } = 10;
        [field: SerializeField] public float ChandelierProb { get; set; } = 5;
        [field: SerializeField] public float HangPianoProb { get; set; } = 4;
        [field: SerializeField] public float AcidTubeProb { get; set; } = 7;
        [field: SerializeField] public float BunkerLihtProb { get; set; } = 10;
        [field: SerializeField] public float ElectricboxProb { get; set; } = 8;
        [field: SerializeField] public float GrapplingHookProb { get; set; } = 10;
        [field: SerializeField] public float CrouchSpaceProb { get; set; } = 10;
        [field: SerializeField] public float DestructableWallProb { get; set; } = 7;
        [field: SerializeField] public float GlassWallProb { get; set; } = 7;
        [field: SerializeField] public float WallProb { get; set; } = 70;
        [field: SerializeField] public float FullWallProb { get; set; } = 36;
        [field: SerializeField] public float LavaFloorProb { get; set; } = 7;
        [field: SerializeField] public float AcidFloorProb { get; set; } = 7;
        [field: SerializeField] public float GlassFloorProb { get; set; } = 8;
        [field: SerializeField] public float DestructableFloorProb { get; set; } = 8;
        [field: SerializeField] public float FloorProb { get; set; } = 70;
        [field: SerializeField] public float FullFloorProb { get; set; } = 40;


        [field: Header("Audio")]
        [field: SerializeField] public int Ambiance { get; set; } = 1;


        [field: Header("Miscellaneous")]
        [field: SerializeField] public int TextureIndex { get; set; } = 0;
        [field: SerializeField] public int SkyboxIndex { get; set; } = 0;
        [field: SerializeField] public int AmbientParticleIndex { get; set; } = 1;
        [field: SerializeField] public float Gravity { get; set; } = -9.81f;
        [field: SerializeField] public bool Fog { get; set; } = false;
        [field: SerializeField] public float FogStrength { get; set; } = 70;
        [field: SerializeField] public Color FogColor { get; set; } = new Color(0.8f, 0.8f, 0.8f);
        
        
        public void SetFogColor(float? r = null, float? g = null, float? b = null)
        {
            Vector4 tempColor = FogColor;
            if (r != null)
                tempColor.x = (float)r;
            if (g != null)
                tempColor.y = (float)g;
            if (b != null)
                tempColor.z = (float)b;
            FogColor = tempColor;
        }
    }
}
