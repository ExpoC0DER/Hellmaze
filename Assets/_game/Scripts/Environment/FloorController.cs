using System.Collections;
using System.Collections.Generic;
using _game.Scripts.Definitions;
using _game.Scripts.System;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class FloorController : MonoBehaviour
{
	[SerializeField] int floorSize = 11;
	[SerializeField] int floorBlockSize = 4; //default wall grid = 4x4
	[SerializeField] int floorPos = 0; //dont spawn holes at 0. floor
	[SerializeField] float refreshInterval = 1f;
	[SerializeField] Transform floorPrefab;
	
	[Header("Floor Properties")]
	List<Transform> allFloors = new List<Transform>();
	List<Transform> allEnabledFloors = new List<Transform>();
	List<Transform> solidFloors = new List<Transform>(); //spawn on these
	
	[SerializeField] Vector2Int noFloorRange;
	[SerializeField] Vector2Int fullFloorRange;
	[SerializeField] Vector2Int acidFloorRange;
	[SerializeField] Vector2Int lavaFloorRange;
	[SerializeField] Vector2Int destructibleFloorRange;
	[SerializeField] Vector2Int glassFloorRange;
	
	[Header("Navigation")]
	[SerializeField] Collider navMeshBlock_col;
	[SerializeField] NavMeshSurface navMeshSurface;
	
	[Header("Objects")]
	[SerializeField] SolidMapObject[] mapObjects_prefabs; // 0-stomper,1-explBarrel,2-stairs,3-jumppad,4-chandelier,5-hangpiano,6-acidtube,7-bunkerlight,8-electricbox
	List<Vector2Int> mapObjectProbs = new List<Vector2Int>();
	Dictionary<SolidMapObject, Transform> objectToFloors = new Dictionary<SolidMapObject, Transform>();
	
	MapSettings _mapSettings;
	
	public void SetFloorAndStart()
	{		
		string mapSettingsJson = PlayerPrefs.GetString("MapSettings", null);
		_mapSettings = string.IsNullOrEmpty(mapSettingsJson) ? new MapSettings() : JsonUtility.FromJson<MapSettings>(mapSettingsJson);

		floorSize = _mapSettings.MazeSize;
		SetObjectProbabilities();
		SetFloorProbabilities();
		CreateFloor();
	}
	
	IEnumerator FloorRoutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(refreshInterval);
			RandomizeFloors();
		}
	}
	
	void CreateFloor()
	{
		//if size is pair number move by half block
		float pairOffset = 0;
		Vector3 navMeshPos = transform.position;
		if(((int)floorSize % 2) == 0)
		{
			pairOffset = 0.5f * floorBlockSize;
			navMeshPos = new Vector3(transform.position.x - 2, transform.position.y, transform.position.z - 2);
		}
		
		//setup navigation and temp floor
		float tempFloorScale = 4 * 0.1f * floorSize;
		navMeshBlock_col.transform.position = navMeshPos;
		navMeshBlock_col.transform.localScale = new Vector3(tempFloorScale, tempFloorScale, tempFloorScale);
		navMeshBlock_col.enabled = true;
		navMeshSurface.BuildNavMesh();
		
		float y = floorSize * 0.5f * floorBlockSize - (0.5f * floorBlockSize)  - pairOffset;
		float x = floorSize * -0.5f * floorBlockSize + (0.5f * floorBlockSize)  - pairOffset;
		int column = 1;
		int row = 1;
		for (int i = 0; i < floorSize * floorSize; i++)
		{
			//spawn number of damaging floors based on percentage
			
			Transform floor = Instantiate(floorPrefab, transform.position, Quaternion.identity, transform);
			floor.transform.position = new Vector3(x, floorPos -0.25f, y); //min half the block size
			//Debug.Log("index" + i + " row " + row + " column " + column + " block pos: " + floor.transform.position);
			column++;
			x += floorBlockSize;
			if(column > floorSize)
			{
				column = 1;
				row++;
				y -= floorBlockSize;
				x = floorSize * -0.5f * floorBlockSize + (0.5f * floorBlockSize) - pairOffset;
			}
			allFloors.Add(floor);
		}
		
		RandomizeFloors();
		
		navMeshBlock_col.enabled = false;
	}
	
	int PickFloorByProbability()
	{
		int x = Random.Range(0, 100);
			
			if (x >= noFloorRange.x && x < noFloorRange.y && floorPos > 0)
				return 0;
			if (x >= fullFloorRange.x && x < fullFloorRange.y)
				if(floorPos == 0) return 1;
				else return Random.Range(1,5);
			if (x >= acidFloorRange.x && x < acidFloorRange.y)
				return 5;
			if (x >= lavaFloorRange.x && x < lavaFloorRange.y)
				return 6;
			if (x >= destructibleFloorRange.x && x < destructibleFloorRange.y && floorPos > 0)
				return 7;
			if (x >= glassFloorRange.x && x < glassFloorRange.y && floorPos > 0)
				return 8;
			

			return 1;
	}
	
	void RandomizeFloors()
	{
		for (int i = 0; i < allFloors.Count; i++)
		{
			Transform floor = allFloors[i];
			int pickedFloor = PickFloorByProbability();
			for (int g = 0; g < floor.childCount; g++)
			{
				floor.GetChild(g).gameObject.SetActive(g == pickedFloor);
			}
			if(pickedFloor != 0)
			{
				if(pickedFloor == 1)
				{
					solidFloors.Add(floor);
					SpawnRandomObject(floor);
				}
				allEnabledFloors.Add(floor);
			}
		}
		
	}
	
	void RandomizeOneFloor()
	{
		//get random floor
		int floorIndex = Random.Range(0, allFloors.Count);
		Transform floor = allFloors[floorIndex];
		
		//randomize it and remove objects
		
		//when reseting random floor -> pick all object associated to it (via dictionary) and respawn them -> assign to new floor on which they respawn
		
	}
	
	void SetFloorProbabilities()
	{
		int lastMax = 0;
			
		fullFloorRange.x = 0;
		fullFloorRange.y = lastMax += (int)_mapSettings.FullFloorProb;
		acidFloorRange.x = fullFloorRange.y;
		acidFloorRange.y = lastMax += (int)_mapSettings.AcidFloorProb;
		lavaFloorRange.x = acidFloorRange.y;
		lavaFloorRange.y = lastMax += (int)_mapSettings.LavaFloorProb;
		destructibleFloorRange.x = lavaFloorRange.y;
		destructibleFloorRange.y = lastMax += (int)_mapSettings.DestructableFloorProb;
		glassFloorRange.x = destructibleFloorRange.y;
		glassFloorRange.y = lastMax += (int)_mapSettings.GlassFloorProb;
		
		noFloorRange.x = (int)_mapSettings.FloorProb;
		noFloorRange.y = 100;
	}
	
	public Vector3 GetSpawnPos()
	{
		Transform floor = solidFloors[Random.Range(0, solidFloors.Count)];
		Vector3 pos = floor.position;// new  Vector3(floor.transform.position.x, floor.transform.position.y + 0.5f, floor.transform.position.z);
		return pos;
	}
	public Vector3 GetSpawnPos(out Transform floor)
	{
		floor = solidFloors[Random.Range(0, solidFloors.Count)];
		Vector3 pos = floor.position;// new  Vector3(floor.transform.position.x, floor.transform.position.y + 0.5f, floor.transform.position.z);
		return pos;
	}
	
	public void SetClockSpeed(float value) => refreshInterval = value;
	
	public void StartChanging()
	{
		if(refreshInterval == 0) return;
		//start changing coroutine
	}
	
	void SetObjectProbabilities()
	{
		int lastMax = 0;
		for (int i = 0; i < mapObjects_prefabs.Length; i++)
		{
			mapObjectProbs.Add(new Vector2Int(lastMax, lastMax += GetObjectProbabilityByIndex(i)));
		}
	}
	
	int GetObjectProbabilityByIndex(int index)
	{
		switch (index)
		{
			case 0: return (int)_mapSettings.StomperProb;
			case 1: return (int)_mapSettings.ExplosiveBarrelProb;
			case 2: return (int)_mapSettings.StairsProb;
			case 3: return (int)_mapSettings.JumppadProb;
			case 4: return (int)_mapSettings.ChandelierProb;
			case 5: return (int)_mapSettings.HangPianoProb;
			case 6: return (int)_mapSettings.AcidTubeProb;
			case 7: return (int)_mapSettings.BunkerLihtProb;
			case 8: return (int)_mapSettings.ElectricboxProb;
			default: return (int)_mapSettings.StomperProb;
		}
	}
	
	Vector3 GetObjectPosition(int index, out Vector3 rotation, Transform relativeTransform)
	{
		Vector3 pos = relativeTransform.position;
		Vector3 add_pos = Vector3.zero;
		Vector3 rot = Vector3.zero;
		switch (index)
		{
			case 1: add_pos = GetRandomFloorCornerPosition(pos, -0.5f, out rot) + new Vector3(0,1.4f,0);
			break;
			case 2: rot = GetRandomRot_4Sides();
			break;
			case 3: add_pos = new Vector3(0,0.3f,0);
			break;
			case 4: add_pos = new Vector3(0,6,0);
			break;
			case 5: add_pos = new Vector3(0,6,0);
			break;
			case 6: add_pos = GetRandomFloorCornerPosition(pos, -0.5f, out rot) + new Vector3(0,1.8f,0);
			break;
			case 7: add_pos = GetRandomFloorCornerPosition(pos, -0.3f, out rot) + new Vector3(0,1.4f,0);
			break;
			case 8: add_pos = GetRandomFloorCornerPosition(pos, -0.3f, out rot) + new Vector3(0,1.4f,0);
			break;
			default: add_pos = Vector3.zero;
			break;
		}
		pos += add_pos;
		
		rotation = rot;
		return pos;
	}
	
	Vector3 GetRandomFloorCornerPosition(Vector3 position, float cornerOffset, out Vector3 rotation)
	{
		int edge = (int)Random.Range(0f,3f);
		switch (edge)
		{
			case 0:
				rotation = new Vector3(0,180,0);
				return new Vector3(position.x + 2 + cornerOffset, position.y, position.z - 2 - cornerOffset);
			case 1:
				rotation = new Vector3(0,90,0);
				return new Vector3(position.x + 2 + cornerOffset, position.y, position.z + 2 + cornerOffset);
			case 2:
				rotation = new Vector3(0,0,0);
				return new Vector3(position.x - 2 - cornerOffset, position.y, position.z + 2 + cornerOffset);
			default:
				rotation = new Vector3(0,270,0);
				return new Vector3(position.x - 2 - cornerOffset, position.y, position.z - 2 - cornerOffset);
		}
	}
	
	Vector3 GetRandomRot_4Sides()
	{
		int ran = (int)Random.Range(0f,3f);
		switch (ran)
		{
			case 0:
				return new Vector3(0,0,0);
			case 1:
				return new Vector3(0,270,0);
			case 2:
				return new Vector3(0,180,0);
			default:
				return new Vector3(0,90,0);
		}
	}
	
	void SpawnRandomObject(Transform floor)
	{
		float objectProb = Random.Range(0f,100f);
		if(objectProb > _mapSettings.ObjectProb) return;
		
		int pickedIndex = 0;
		for (int i = 0; i < mapObjectProbs.Count; i++)
		{
			if(mapObjectProbs[i].x <= objectProb && objectProb <= mapObjectProbs[i].y)
			{
				pickedIndex = i;
			}
		}
		Vector3 objRot;
		SolidMapObject pickedObject = Instantiate(mapObjects_prefabs[pickedIndex], GetObjectPosition(pickedIndex, out objRot, floor), Quaternion.Euler(objRot));
		objectToFloors.Add(pickedObject, floor);
		
	}
	
}
