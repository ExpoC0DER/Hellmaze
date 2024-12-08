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
	
	public void Setup(string playerName, Sprite playerPicture, string playerScore, float ratio, Color color)
	{
		if(playerPicture != null)
		{
			profilePic.sprite = playerPicture;
		}
		nameText.text = playerName;
		scoreText.text = playerScore;
		scoreSlider.value = ratio;
		fillImage.color = color;
	}
}
