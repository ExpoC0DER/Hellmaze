using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScore : MonoBehaviour
{
	[SerializeField] private Image profilePic;
	[SerializeField] private TextMeshProUGUI nameText;
	[SerializeField] private TextMeshProUGUI scoreText;
	[SerializeField] private Slider scoreSlider;
	[SerializeField] private Image fillImage;
	
	public void Setup(string playerName, Sprite playerPicture, string playerScore, Color color)
	{
		if(playerPicture != null)
		{
			profilePic.sprite = playerPicture;
		}
		nameText.text = playerName;
		scoreText.text = playerScore;
		fillImage.color = color;
	}
	
	public void SetUpRatio(float ratio)
	{
		scoreSlider.value = ratio;
	}
}
