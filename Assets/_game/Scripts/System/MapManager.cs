using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _game.Scripts;
using UnityEngine;
using UnityEngine.Rendering;

public class MapManager : MonoBehaviour
{
	
	[SerializeField] MazeController[] mazes;
	[SerializeField] FloorController[] floors;
	
	[Header("Gameplay")]
	[SerializeField] Transform BotPrefab;
	//set bot difficulty 0-noob, 1-casual, 2-expert
	
	Queue<Transform> lastUsedSpawnPositions = new Queue<Transform>();
	
	[Header("Pickups")]
	[SerializeField] Transform[] pickup_prefabs;
	List<Transform> pickupsInScene = new List<Transform>();
	
	[Header("Objects")]
	
	
	[Header("Music")]
	//set music / ambiance to play
	
	[Header("Misc")]
	[SerializeField] Material wallMaterial;
	[SerializeField] Material floorMaterial;
	[SerializeField] Texture2D[] wallTextures;
	[SerializeField] Texture2D[] floorTextures;
	[SerializeField] ParticleSystem[] ambientParticles;
	[SerializeField] Material[] skyboxMats;
	[SerializeField] Light[] dir_lights;
	
	MapSettings settings;
	
	
	void Start()
	{
		ActivateMapSetup();
	}
	
	void ActivateMapSetup() => StartCoroutine(MapSetup());
	
	IEnumerator MapSetup()
	{
		
		yield return new WaitUntil(() => Menu.main != null);
		settings = Menu.main.mapSettings;
		
		Physics.gravity = new Vector3(0, settings.gravity, 0);
		
		wallMaterial.SetTexture("_Texture2D", wallTextures[settings.TextureIndex]);
		floorMaterial.SetTexture("_Texture2D", floorTextures[settings.TextureIndex]);
		
		for (int i = 0; i < floors.Length; i++)
		{
			FloorController floor = floors[i];
			
			floor.gameObject.SetActive(settings.Floors >= i + 1);
			if(floor.gameObject.activeSelf)
			{
				floor.SetClockSpeed(settings.FloorChangeSpeed);
				floor.SetFloorAndStart();
			}
		}
		
		for (int i = 0; i < mazes.Length; i++)
		{
			MazeController maze = mazes[i];
			
			maze.gameObject.SetActive(settings.Floors >= i + 1);
			if(maze.gameObject.activeSelf)
			{
				maze.SetClockSpeed(settings.WallChangeSpeed);
				maze.SetMazeSizeAndStart(settings.MazeSize);
			}
		}
		
		//set destructable wall probability, crouch space, grappling
		//set damagin floor probability
		//set stomper probability
		//set explosive barel probability (barrel spawn on edge of block)
		
		//set weapon pickup and health pickup probability
		SpawnPickups();
		
		//set skybox, particles, ambiance and music, fog
		for (int i = 0; i < ambientParticles.Length; i++)
		{
			if(i == settings.AmbientParticleIndex) ambientParticles[i].Play();
			else ambientParticles[i].Stop();
		}
		
		RenderSettings.skybox = skyboxMats[settings.SkyboxIndex];
		RenderSettings.fog = settings.Fog;
		RenderSettings.fogMode = FogMode.Linear;
		RenderSettings.fogStartDistance = 1;
		RenderSettings.fogEndDistance = 100 - settings.FogStrength;
		RenderSettings.fogColor = new Color(settings.FogColor_r, settings.FogColor_g, settings.FogColor_b);
		RenderSettings.ambientMode = AmbientMode.Skybox;
		if(settings.SkyboxIndex == 7) // 7 is index of night
		{
			RenderSettings.ambientIntensity = 2.5f;
		}else
		{
			RenderSettings.ambientIntensity = 1f;
		}
		for (int i = 0; i < dir_lights.Length; i++)
		{
			dir_lights[i].gameObject.SetActive(i == settings.SkyboxIndex);
		}
		DynamicGI.UpdateEnvironment();
		
		
		SpawnBots();
		//set bots with difficulty
		
		//update playerdatabase
		GameManager.main.playerDatabase.FirstSetup();
		GameManager.main.playerDatabase.SetMapGravityToPlayers(settings.gravity);
	}
	
	void SpawnPickups()
	{
		for (int i = 0; i < pickup_prefabs.Length; i++)
		{
			for (int g = 0; g <= GetPickupCount(i); g++)
			{
				Transform pickup = Instantiate(pickup_prefabs[i], GetSpawnPos(0.5f), Quaternion.identity);
				pickupsInScene.Add(pickup);
			}
			
		}
	}
	
	void SpawnBots()
	{
		for (int i = 0; i < Menu.main.mapSettings.BotCount; i++)
		{
			Instantiate(BotPrefab, GetSpawnPos(0.5f), Quaternion.identity);
		}
	}
	
	int GetPickupCount(int index)
	{
		switch (index)
		{
			case 0: return (int)settings.HealthPickup_Count;
			case 1: return (int)settings.PistolPickup_Count;
			case 2: return (int)settings.RevolverPickup_Count;
			case 3: return (int)settings.AkPickup_Count;
			case 4: return (int)settings.MinigunPickup_Count;
			case 5: return (int)settings.ShotgunPickup_Count;
			case 6: return (int)settings.SupershotgunPickup_Count;
			case 7: return (int)settings.SniperRiflePickup_Count;
			case 8: return (int)settings.RPGPickup_Count;
			case 9: return (int)settings.GrenadeLauncherPickup_Count;
			case 10: return (int)settings.StickybombLauncherPickup_Count;
			case 11: return (int)settings.LazergunPickup_Count;
			case 12: return (int)settings.AcidBlasterPickup_Count;
			case 13: return (int)settings.PlasmagunPickup_Count;
			case 14: return (int)settings.DroneLauncherPickup_Count;
			default: return (int)settings.HealthPickup_Count;
		}
	}
	
	public Vector3 GetSpawnPos(float offset_y = 0)
	{
		Vector3 pos = Vector3.zero;
		Transform floor;
		int queueLength = settings.MazeSize * 2;
		int attempts = 0;
		do
		{
			pos = floors[Random.Range(0, settings.Floors)].GetSpawnPos(out floor) + new Vector3(0, offset_y + 0.33f, 0);
			attempts++;
		}
		while(lastUsedSpawnPositions.Contains(floor) && attempts < queueLength);
		
		if(floor != null)lastUsedSpawnPositions.Enqueue(floor);
		if(lastUsedSpawnPositions.Count >= queueLength)
		{
			lastUsedSpawnPositions.Dequeue();
		}
		
		return pos;
	}
	
	
	
	//respawn remake -> to find floor that is solid (y = settings.floorheight +0.3f)

}
