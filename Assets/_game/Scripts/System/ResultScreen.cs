using System.Collections.Generic;
using System.Linq;
using _game.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class ResultScreen : MonoBehaviour
{
	[SerializeField] GameObject _resultScreen_object;
	[SerializeField] PlayerScore scorePrefab;
	[SerializeField] Transform scoreParent;
	[SerializeField] EventReference matchFinish_sfx;
	[SerializeField] EventReference matchStart_sfx;
	
	List<PlayerScore> playerScores = new List<PlayerScore>();
	
	bool endGame = false;
	
	void Start()
	{
		SwitchResults(false);
		FMODHelper.PlayNewInstance(matchStart_sfx, transform);
	}
	
	void OnEnable()
	{
		GameManager.main.playerControlls.Player.ResultScreen.started += x => ResultScreenInput(true);
		GameManager.main.playerControlls.Player.ResultScreen.canceled += x => ResultScreenInput(false);
	}
	
	void OnDisable()
	{
		GameManager.main.playerControlls.Player.ResultScreen.started -= x => ResultScreenInput(true);
		GameManager.main.playerControlls.Player.ResultScreen.canceled -= x => ResultScreenInput(false);
	}
	
	void ResultScreenInput(bool on)
	{
		if(endGame) return;
		SwitchResults(on);
	}
	
	void SwitchResults(bool on)
	{
		_resultScreen_object.SetActive(on);
		if(on) RefreshResults();
	}
	
	public void DisplayEndGameResults()
	{
		endGame = true;
		SwitchResults(true);
		FMODHelper.PlayNewInstance(matchFinish_sfx, transform);
		Menu.main.menuLocked = true;
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
		SortScore_Kills(players, out Vector2 minMax_value);
		
		
		
		for (int i = 0; i < players.Count; i++)
		{
			PlayerStats playerStats = players[i];			
			string score = "Kills: " + playerStats.kills + "\nDeaths: "+ playerStats.deaths + "\nK/D: " + GetKillDeathRatio(playerStats.kills, playerStats.deaths).ToString("f2");
			float interp_score = Mathf.Lerp(0.20f, 1, Mathf.InverseLerp(minMax_value.x, minMax_value.y, playerStats.kills));
			Color color = Color.HSVToRGB(Random.Range(0f,1f), 1, 1);
			//Debug.Log("minmax " + minMax_value + " kills " + playerStats.kills + " interpScore " + interp_score);
			playerScores[i].Setup(playerStats.playerName, playerStats.playerProfilePic, score, interp_score, color);
		}
	}
	
	void SortScore_Kills(List<PlayerStats> players, out Vector2 minMax_value)
	{
		players.Sort((x, y) => y.kills.CompareTo(x.kills));
		// Lowest score
		minMax_value = Vector2.zero;
		minMax_value.x = players.Min(player => player.kills);
		//Player lowestScorePlayer = players.First(player => player.Score == lowestScore);

		// Highest score
		minMax_value.y = players.Max(player => player.kills);
		//Player highestScorePlayer = players.First(player => player.Score == highestScore);

	}
	void SortScore_KD(List<PlayerStats> players, out Vector2 minMax_value)
	{
		players.Sort((x, y) => y.kills/y.deaths.CompareTo(x.kills/x.deaths));
		minMax_value = Vector2.zero;
		minMax_value.x = players.Min(player => player.kills / player.deaths);
		//Player lowestScorePlayer = players.First(player => player.Score == lowestScore);

		// Highest score
		minMax_value.y = players.Max(player => player.kills / player.deaths);
	}
	float GetKillDeathRatio(float kills, float deaths)
	{
		if(kills == 0) return -deaths;
		if(deaths == 0) return kills;
		return kills / deaths;
	}
	
}
