using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _game.Scripts;
using _game.Scripts.Definitions;
using _game.Scripts.System;
using UnityEngine;
using UnityEngine.Rendering;
using MapSettings = _game.Scripts.Definitions.MapSettings;

public class MapManager : MonoBehaviour
{
	
	[SerializeField] MazeController[] mazes;
	[SerializeField] FloorController[] floors;
	
	[Header("Gameplay")]
	[SerializeField] Transform BotPrefab;
	//set bot difficulty 0-noob, 1-casual, 2-expert
	[SerializeField] GameObject[] gameModeObjects;
	
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
	
	private MapSettings _mapSettings;

	void Start()
	{
		string mapSettingsJson = PlayerPrefs.GetString("MapSettings", null);
		_mapSettings = string.IsNullOrEmpty(mapSettingsJson) ? new MapSettings() : JsonUtility.FromJson<MapSettings>(mapSettingsJson);
		
		SetupMap();
	}
	
	
	private void SetupMap()
	{
		Physics.gravity = new Vector3(0, _mapSettings.Gravity, 0);
		
		wallMaterial.SetTexture("_Texture2D", wallTextures[_mapSettings.TextureIndex]);
		floorMaterial.SetTexture("_Texture2D", floorTextures[_mapSettings.TextureIndex]);
		
		for (int i = 0; i < floors.Length; i++)
		{
			FloorController floor = floors[i];
			
			floor.gameObject.SetActive(_mapSettings.Floors >= i + 1);
			if(floor.gameObject.activeSelf)
			{
				floor.SetClockSpeed(_mapSettings.FloorChangeSpeed);
				floor.SetFloorAndStart();
			}
		}
		
		for (int i = 0; i < mazes.Length; i++)
		{
			MazeController maze = mazes[i];
			
			maze.gameObject.SetActive(_mapSettings.Floors >= i + 1);
			if(maze.gameObject.activeSelf)
			{
				maze.SetClockSpeed(_mapSettings.WallChangeSpeed);
				maze.SetMazeSizeAndStart(_mapSettings.MazeSize);
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
			if(i == _mapSettings.AmbientParticleIndex) ambientParticles[i].Play();
			else ambientParticles[i].Stop();
		}
		
		RenderSettings.skybox = skyboxMats[_mapSettings.SkyboxIndex];
		RenderSettings.fog = _mapSettings.Fog;
		RenderSettings.fogMode = FogMode.Linear;
		RenderSettings.fogStartDistance = 1;
		RenderSettings.fogEndDistance = 100 - _mapSettings.FogStrength;
		RenderSettings.fogColor = _mapSettings.FogColor;
		RenderSettings.ambientMode = AmbientMode.Skybox;
		if(_mapSettings.SkyboxIndex == 7) // 7 is index of night
		{
			RenderSettings.ambientIntensity = 2.5f;
		}else
		{
			RenderSettings.ambientIntensity = 1f;
		}
		for (int i = 0; i < dir_lights.Length; i++)
		{
			dir_lights[i].gameObject.SetActive(i == _mapSettings.SkyboxIndex);
		}
		DynamicGI.UpdateEnvironment();
		
		for (int i = 0; i < gameModeObjects.Length; i++)
		{
			gameModeObjects[i].SetActive(_mapSettings.GameMode == i);
		}
		
		SpawnBots();
		//set bots with difficulty
		
		//update playerdatabase
		GameManager.Instance.playerDatabase.FirstSetup();
		GameManager.Instance.playerDatabase.SetMapGravityToPlayers(_mapSettings.Gravity);
	}
	
	void SpawnPickups()
	{
		for (int i = 0; i < pickup_prefabs.Length; i++)
		{
			/* int pickupCount = GetPickupCount(i);
			if(pickupCount <= 0) continue; */
			for (int g = 0; g <= GetPickupCount(i) -1; g++)
			{
				Transform pickup = Instantiate(pickup_prefabs[i], GetSpawnPos(0.5f), Quaternion.identity);
				pickupsInScene.Add(pickup);
				Debug.Log("spawned pickup: " + pickup.name);
			}
			
		}
	}
	
	public int GetMapSize() => _mapSettings.MazeSize;
	
	void SpawnBots()
	{
		for (int i = 0; i < _mapSettings.BotCount; i++)
		{
			Instantiate(BotPrefab, GetSpawnPos(0.5f), Quaternion.identity);
		}
	}
	
	int GetPickupCount(int index)
	{
		switch (index)
		{
			case 0: return (int)_mapSettings.HealthPickupCount;
			case 1: return (int)_mapSettings.PistolPickupCount;
			case 2: return (int)_mapSettings.RevolverPickupCount;
			case 3: return (int)_mapSettings.AkPickupCount;
			case 4: return (int)_mapSettings.MinigunPickupCount;
			case 5: return (int)_mapSettings.ShotgunPickupCount;
			case 6: return (int)_mapSettings.SupershotgunPickupCount;
			case 7: return (int)_mapSettings.SniperRiflePickupCount;
			case 8: return (int)_mapSettings.RpgPickupCount;
			case 9: return (int)_mapSettings.GrenadeLauncherPickupCount;
			case 10: return (int)_mapSettings.StickybombLauncherPickupCount;
			case 11: return (int)_mapSettings.LazergunPickupCount;
			case 12: return (int)_mapSettings.AcidBlasterPickupCount;
			case 13: return (int)_mapSettings.PlasmagunPickupCount;
			case 14: return (int)_mapSettings.DroneLauncherPickupCount;
			default: return (int)_mapSettings.HealthPickupCount;
		}
	}

	private Vector3 GetSpawnPos(float offsetY = 0)
	{
		Vector3 pos;
		Transform floor;
		int queueLength = _mapSettings.MazeSize * 2;
		int attempts = 0;
		do
		{
			pos = floors[Random.Range(0, _mapSettings.Floors)].GetSpawnPos(out floor) + new Vector3(0, offsetY + 0.33f, 0);
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
	
	
	
	//respawn remake -> to find floor that is solid (y = _settingsSetter.floorheight +0.3f)

}
