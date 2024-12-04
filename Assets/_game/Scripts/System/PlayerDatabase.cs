using System.Collections.Generic;
using UnityEngine;

public class PlayerDatabase : MonoBehaviour
{
	[SerializeField] List<PlayerStats> players = new List<PlayerStats>();
	
	
	void Awake()
	{
		FindAllPlayers();
		PickRandomBotNames();
	}
	
	public PlayerStats GetPlayerByName(string name)
	{
		for (int i = 0; i < players.Count; i++)
		{
			if (players[i].playerName == name)
			{
				return players[i];
			}
		}
		Debug.LogError("Haven't found player with name: " + name);
		return null;
	}
	
	public List<PlayerStats> GetPlayerList()
	{
		FindAllPlayers();
		return players;
	}
	
	void FindAllPlayers()
	{
		players.Clear();
		
		PlayerStats[] p = FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);
		foreach (PlayerStats player in p)
		{
			players.Add(player);
		}
	}
	
	void PickRandomBotNames()
	{
		string[] botnames =
		{
		"Maniacal",
		"Adin",
		"Kixa",
		"Perdek",
		"Mattytiahu",
		"Panasonic",
		"Expogamer",
		"LG",
		"Pizderius",
		"Ajko",
		"Vololo",
		"Zucker"
		};
		
		List<int> pickedIndexes = new List<int>();
		for (int i = 0; i < players.Count; i++)
		{
			PlayerStats player = players[i];
			if(!player.isPlayer)
			{
				int randomIndex = Random.Range(0, botnames.Length);
				while(pickedIndexes.Contains(randomIndex))
				{
					randomIndex = Random.Range(0, botnames.Length);
				}
				pickedIndexes.Add(randomIndex);
				player.playerName = botnames[randomIndex];
			}
		}
	}
	
}
