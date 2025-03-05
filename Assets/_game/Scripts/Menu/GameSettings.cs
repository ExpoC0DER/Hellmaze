using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using _game.Scripts;
using TMPro;

public class GameSettings : MonoBehaviour
{
	[SerializeField] Image crosshairPreview;
	[SerializeField] TextMeshProUGUI playerName;
	[SerializeField] Image sprayPreview;
	[SerializeField] TextMeshProUGUI sprayPathText;
	[SerializeField] Material sprayMat;
	
	
	public static event Action<Image> SetCrosshair;
	public static event Action<string> SetPlayerName;
	
	public void SetCrosshairSize(float size)
	{
		crosshairPreview.rectTransform.sizeDelta = new Vector2(size, size);
	}
	
	public void SetCrosshairRed(float red)
	{
		crosshairPreview.color = new Color(red, crosshairPreview.color.g, crosshairPreview.color.b, crosshairPreview.color.a);
	}
	public void SetCrosshairGreen(float green)
	{
		crosshairPreview.color = new Color(crosshairPreview.color.r, green, crosshairPreview.color.b, crosshairPreview.color.a);
	}
	public void SetCrosshairBlue(float blue)
	{
		crosshairPreview.color = new Color(crosshairPreview.color.r, crosshairPreview.color.g, blue, crosshairPreview.color.a);
	}
	public void SetCrosshairAlpha(float alpha)
	{
		crosshairPreview.color = new Color(crosshairPreview.color.r, crosshairPreview.color.g, crosshairPreview.color.b, alpha);
	}
	public void LoadSpriteFromPath()
	{
		string path = sprayPathText.text;
		if (File.Exists(path))
		{
			byte[] imageBytes = File.ReadAllBytes(path);
			Texture2D texture = new Texture2D(2, 2); // Create a new texture
			if (texture.LoadImage(imageBytes)) // Load the image data into the texture
			{
				// Convert the texture to a sprite
				Sprite newSprite = Sprite.Create(
					texture,
					new Rect(0, 0, texture.width, texture.height),
					new Vector2(0.5f, 0.5f)
				);
				sprayPreview.sprite = newSprite;
				
			}
			else
			{
				Debug.LogError("Failed to load image into texture.");
			}
		}
		else
		{
			Debug.LogError($"File does not exist at path: {path}");
		}
	}
	void SetSpray()
	{
		//sprayMat.SetTexture("Base_Map", sprayPreview.sprite.texture);
	}
	public void Apply()
	{
		SetCrosshair?.Invoke(crosshairPreview);
		SetPlayerName?.Invoke(playerName.text);
		SetSpray();
	}
	
	
}
