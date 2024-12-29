using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MapSettings : MonoBehaviour
{
	[Header("Maze")]
	public int MazeSize = 11;
	public void SetMapSize(float value) => MazeSize = (int)value;
	public Slider MapSize_Slider;
	public int Floors = 1;
	public void SetMapFloors(float value) => Floors = (int)value;
	public Slider Floors_Slider;
	public float WallChangeSpeed = 0.3f;
	public void SetWallChangeSpeed(float value) => WallChangeSpeed = (int)value;
	public Slider WallChangeSpeed_Slider;
	public float FloorChangeSpeed = 1;
	public void SetFloorChangeSpeed(float value) => FloorChangeSpeed = (int)value;
	public Slider FloorChangeSpeed_Slider;
	public float gravity = -9.81f;
	public void SetGravity(float value) => gravity = (int)value;
	public Slider Gravity_Slider;
	
	[Header("Bots")]
	public int BotCount = 5;
	public void SetBotCount(float value) => BotCount = (int)value;
	public Slider BotCount_Slider;
	public int BotDifficulty = 1;
	public void SetBotDifficulty(int value) => BotDifficulty = value;
	public TMP_Dropdown BotDifficulty_Dropdown;
	
	[Header("Preset")]
	public int MapPreset = 0;
	public void SetMapPreset(float value) => MapPreset = (int)value;
	public TMP_Dropdown MapPreset_Dropdown;
	public int GameMode = 0;
	public void SetGameMode(int value) => GameMode = value;
	public TMP_Dropdown GameMode_Dropdown;
	public TextMeshProUGUI ObjectiveInfo_text;
	
	[Header("Custom")]
	[Header("Items & Objects")]
	public float Pickup_Count = 20;
	public void SetPickupCount(float value) => Pickup_Count = (int)value;
	public Slider Pickup_Count_Slider;
	
	public float HealthPickup_Count = 6;
	public void SetHealthPickupCount(float value) => HealthPickup_Count = (int)value;
	public Slider HealthPickup_Count_Slider;
	
	public float PistolPickup_Count = 1;
	public void SetPistolPickupCount(float value) => PistolPickup_Count = (int)value;
	public Slider PistolPickup_Count_Slider;
	public float RevolverPickup_Count = 1;
	public void SetRevolverPickupCount(float value) => RevolverPickup_Count = (int)value;
	public Slider RevolverPickup_Count_Slider;
	public float AkPickup_Count = 1;
	public void SetAkPickupCount(float value) => AkPickup_Count = (int)value;
	public Slider AkPickup_Count_Slider;
	public float MinigunPickup_Count = 1;
	public void SetMinigunPickupCount(float value) => MinigunPickup_Count = (int)value;
	public Slider MinigunPickup_Count_Slider;
	public float ShotgunPickup_Count = 1;
	public void SetShotgunPickupCount(float value) => ShotgunPickup_Count = (int)value;
	public Slider ShotgunPickup_Count_Slider;
	public float SupershotgunPickup_Count = 1;
	public void SetSuperShotgunPickupCount(float value) => SupershotgunPickup_Count = (int)value;
	public Slider SupershotgunPickup_Count_Slider;
	public float SniperRiflePickup_Count = 1;
	public void SetSniperRiflePickupCount(float value) => SniperRiflePickup_Count = (int)value;
	public Slider SniperRiflePickup_Count_Slider;
	public float RPGPickup_Count = 1;
	public void SetRPGPickupCount(float value) => RPGPickup_Count = (int)value;
	public Slider RPGPickup_Count_Slider;
	public float GrenadeLauncherPickup_Count = 1;
	public void SetGrenadeLauncherPickupCount(float value) => GrenadeLauncherPickup_Count = (int)value;
	public Slider GrenadeLauncherPickup_Count_Slider;
	public float StickybombLauncherPickup_Count = 1;
	public void SetStickybombLauncherPickupCount(float value) => StickybombLauncherPickup_Count = (int)value;
	public Slider StickybombLauncherPickup_Count_Slider;
	public float LazergunPickup_Count = 1;
	public void SetLazergunPickupCount(float value) => LazergunPickup_Count = (int)value;
	public Slider LazergunPickup_Count_Slider;
	public float AcidBlasterPickup_Count = 1;
	public void SetAcidBlasterPickupCount(float value) => AcidBlasterPickup_Count = (int)value;
	public Slider AcidBlasterPickup_Count_Slider;
	public float PlasmagunPickup_Count = 1;
	public void SetPlasmagunPickupCount(float value) => PlasmagunPickup_Count = (int)value;
	public Slider PlasmagunPickup_Count_Slider;
	public float DroneLauncherPickup_Count = 1;
	public void SetDroneLauncherPickupCount(float value) => DroneLauncherPickup_Count = (int)value;
	public Slider DroneLauncherPickup_Count_Slider;
	
	public float Object_Prob = 70;
	public void SetObjectProb(float value) => Object_Prob = (int)value;
	public Slider Object_Prob_Slider;
	public float ExplosiveBarrel_Prob = 12;
	public void SetExplosiveBarrelProb(float value) => ExplosiveBarrel_Prob = (int)value;
	public Slider ExplosiveBarrel_Prob_Slider;
	public float Stomper_Prob = 7;
	public void SetStomperProb(float value) => Stomper_Prob = (int)value;
	public Slider Stomper_Prob_Slider;
	public float Stairs_Prob = 7;
	public void SetStairsProb(float value) => Stairs_Prob = (int)value;
	public Slider Stairs_Prob_Slider;
	public float Jumppad_Prob = 10;
	public void SetJumppadProb(float value) => Jumppad_Prob = (int)value;
	public Slider Jumppad_Prob_Slider;
	public float Chandelier_Prob = 5;
	public void SetChandelierProb(float value) => Chandelier_Prob = (int)value;
	public Slider Chandelier_Prob_Slider;
	public float HangPiano_Prob = 4;
	public void SetHangPianoProb(float value) => HangPiano_Prob = (int)value;
	public Slider HangPiano_Prob_Slider;
	public float AcidTube_Prob = 7;
	public void SetAcidTubeProb(float value) => AcidTube_Prob = (int)value;
	public Slider AcidTube_Prob_Slider;
	public float BunkerLiht_Prob = 10;
	public void SetBunkerLihtProb(float value) => BunkerLiht_Prob = (int)value;
	public Slider BunkerLiht_Prob_Slider;
	public float Electricbox_Prob = 8;
	public void SetElectricboxProb(float value) => Electricbox_Prob = (int)value;
	public Slider Electricbox_Prob_Slider;
	
	public float GrapplingHook_Prob = 10;
	public void SetGrapplingHookProb(float value) => GrapplingHook_Prob = (int)value;
	public Slider GrapplingHook_Prob_Slider;
	public float CrouchSpace_Prob = 10;
	public void SetCrouchSpaceProb(float value) => CrouchSpace_Prob = (int)value;
	public Slider CrouchSpace_Prob_Slider;
	public float DestructableWall_Prob = 7;
	public void SetDestructableWallProb(float value) => DestructableWall_Prob = (int)value;
	public Slider DestructableWall_Prob_Slider;
	public float GlassWall_prob = 7;
	public void SetGlassWallProb(float value) => GlassWall_prob = (int)value;
	public Slider GlassWall_prob_Slider;
	public float Wall_prob = 70;
	public void SetWallProb(float value) => Wall_prob = (int)value;
	public Slider Wall_prob_Slider;
	public float FullWall_prob = 36;
	public void SetFullWallProb(float value) => FullWall_prob = (int)value;
	public Slider FullWall_prob_Slider;
	
	public float LavaFloor_prob = 7;
	public void SetLavaFloorProb(float value) => LavaFloor_prob = (int)value;
	public Slider LavaFloor_prob_Slider;
	public float AcidFloor_prob = 7;
	public void SetAcidFloorProb(float value) => AcidFloor_prob = (int)value;
	public Slider AcidFloor_prob_Slider;
	public float GlassFloor_prob = 8;
	public void SetGlassFloorProb(float value) => GlassFloor_prob = (int)value;
	public Slider GlassFloor_prob_Slider;
	public float DestructableFloor_prob = 8;
	public void SetDestructableFloorProb(float value) => DestructableFloor_prob = (int)value;
	public Slider DestructableFloor_prob_Slider;
	public float Floor_prob = 70;
	public void SetFloorProb(float value) => Floor_prob = (int)value;
	public Slider Floor_prob_Slider;
	public float FullFloor_prob = 40;
	public void SetFullFloorProb(float value) => FullFloor_prob = (int)value;
	public Slider FullFloor_prob_Slider;
	
	[Header("Audio")]
	public int Ambiance = 1;
	public void SetAmbianceIndex(int value) => Ambiance = value;
	public TMP_Dropdown Ambiance_Dropdown;
	
	[Header("Miscellaneous")]
	public int TextureIndex = 0;
	public void SetTexturePreset(int value) => TextureIndex = value;
	public TMP_Dropdown TextureIndex_Dropdown;
	public int SkyboxIndex = 0;
	public void SetSkyboxIndex(int value) => SkyboxIndex = value;
	public TMP_Dropdown SkyboxIndex_Dropdown;
	public int AmbientParticleIndex = 1;
	public void SetAmbientParticleIndex(int value) => AmbientParticleIndex = value;
	public TMP_Dropdown AmbientParticleIndex_Dropdown;
	
	//check if changespeed of maze change has effect
	
	//set bot difficulty
	
	//set gamemode
	
	
	public void SetMapPreset(int index)
	{
		//add ambiant music when ready
		//also add object probabilities based on locations
		switch (index)
		{
			case 0: //stone hall
			TextureIndex_Dropdown.value = 0;
			SkyboxIndex_Dropdown.value = 0;
			AmbientParticleIndex_Dropdown.value = 1;
			break;
			case 1: // snowy
			TextureIndex_Dropdown.value = 1;
			SkyboxIndex_Dropdown.value = 7;
			AmbientParticleIndex_Dropdown.value = 3;
			break;
			case 2: // labs
			TextureIndex_Dropdown.value = 2;
			SkyboxIndex_Dropdown.value = 2;
			AmbientParticleIndex_Dropdown.value = 1;
			break;
			case 3: //elite corridor
			TextureIndex_Dropdown.value = 3;
			SkyboxIndex_Dropdown.value = 6;
			AmbientParticleIndex_Dropdown.value = 0;
			break;
			case 4: //outside niceday
			TextureIndex_Dropdown.value = 4;
			SkyboxIndex_Dropdown.value = 4;
			AmbientParticleIndex_Dropdown.value = 0;
			break;
			case 5: //outside badday
			TextureIndex_Dropdown.value = 4;
			SkyboxIndex_Dropdown.value = 2;
			AmbientParticleIndex_Dropdown.value = 2;
			break;
			case 6: // metal
			TextureIndex_Dropdown.value = 5;
			SkyboxIndex_Dropdown.value = 5;
			AmbientParticleIndex_Dropdown.value = 4;
			break;
			case 7: // hell
			TextureIndex_Dropdown.value = 6;
			SkyboxIndex_Dropdown.value = 1;
			AmbientParticleIndex_Dropdown.value = 4;
			break;
		}
	}
	
}
