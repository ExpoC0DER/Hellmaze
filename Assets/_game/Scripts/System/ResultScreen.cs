using System.Collections.Generic;
using System.Linq;
using _game.Scripts;
using Unity.VisualScripting;
using UnityEngine;

public class ResultScreen : MonoBehaviour
{
	[SerializeField] GameObject _resultScreen_object;
	[SerializeField] KeyCode key;
	[SerializeField] PlayerScore scorePrefab;
	[SerializeField] Transform scoreParent;
	
	List<PlayerScore> playerScores = new List<PlayerScore>();
	
	void Start()
	{
		SwitchResults(false);
	}
	
	void Update()
	{
		if(Input.GetKeyDown(key))
		{
			SwitchResults(true);
		}
		if(Input.GetKeyUp(key))
		{
			SwitchResults(false);
		}
	}
	
	void SwitchResults(bool on)
	{
		_resultScreen_object.SetActive(on);
		if(on) RefreshResults();
	}
	
	void RefreshResults()
	{
		List<PlayerStats> players = GameManager.main.playerDatabase.GetPlayerList();
		
		if(playerScores.Count > players.Count) // playerScores is more than players => delete a remaining playerScores
		{
			float difference = playerScores.Count - players.Count;
			for (int i = 0; i < difference; i++)
			{
				PlayerScore score = playerScores[i + players.Count];
				playerScores.Remove(score);
				Destroy(score.gameObject);
			}
		}else if(playerScores.Count < players.Count)
		{
			float difference = players.Count - playerScores.Count;
			for (int i = 0; i < difference; i++)
			{
				PlayerScore score = Instantiate(scorePrefab, scoreParent);
				playerScores.Add(score);
			}
		}
		
		RearrangeList_RemoveEmpty();
		
		SetupScores(players);
	}
	
	void RearrangeList_RemoveEmpty()
	{
		List<PlayerScore> newList = new List<PlayerScore>();
		
		for (int i = 0; i < playerScores.Count; i++)
		{
			if(playerScores[i] != null)
			{
				newList.Add(playerScores[i]);
			}
		}
		
		playerScores.Clear();
		for (int i = 0; i < newList.Count; i++)
		{
			playerScores.Add(newList[i]);
		}
		
	}
	
	void SetupScores(List<PlayerStats> players)
	{
		SortScore_Kills(players);
		for (int i = 0; i < players.Count; i++)
		{
			PlayerStats playerStats = players[i];
			string score = "Kills: " + playerStats.kills + "\nDeaths: "+ playerStats.deaths + "\nK/D: " + playerStats.kills / playerStats.deaths;
			playerScores[i].Setup(playerStats.playerName, playerStats.playerProfilePic, score, Color.red);
		}
	}
	
	void SortScore_Kills(List<PlayerStats> players, out Vector2 minMax_value)
	{
		players.Sort((x, y) => y.kills.CompareTo(x.kills));
		// Lowest score
		minMax_value = new Vector2(0,0);
		minMax_value.x = players.Min(player => player.kills);
		//Player lowestScorePlayer = players.First(player => player.Score == lowestScore);

		// Highest score
		minMax_value.y = players.Max(player => player.kills);
		//Player highestScorePlayer = players.First(player => player.Score == highestScore);

	}
	void SortScore_KD(List<PlayerStats> players, out Vector2 minMax_value)
	{
		players.Sort((x, y) => y.kills/y.deaths.CompareTo(x.kills/x.deaths));
	}
	
	
}
