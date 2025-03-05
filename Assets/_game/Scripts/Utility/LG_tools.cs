using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LG_tools : MonoBehaviour
{
	public static void DrawPoint(Vector3 point, float duration = 60)
	{
		Debug.DrawRay(point, Vector3.forward * 0.1f, Color.cyan, duration);
		Debug.DrawRay(point, Vector3.right * 0.1f, Color.red, duration);
		Debug.DrawRay(point, Vector3.up * 0.1f, Color.green, duration);
		
		Debug.DrawRay(point, -Vector3.forward * 0.1f, Color.blue, duration);
		Debug.DrawRay(point, -Vector3.right * 0.1f, Color.magenta, duration);
		Debug.DrawRay(point, -Vector3.up * 0.1f, Color.yellow, duration);
	}
	
	public static void DrawPoint(Vector3 point, float duration = 60, Color? color = null)
	{
		Color logcolor = color ?? Color.white;
		Debug.DrawRay(point, Vector3.forward * 0.1f, logcolor, duration);
		Debug.DrawRay(point, Vector3.right * 0.1f, logcolor, duration);
		Debug.DrawRay(point, Vector3.up * 0.1f, logcolor, duration);
		
		Debug.DrawRay(point, -Vector3.forward * 0.1f, logcolor, duration);
		Debug.DrawRay(point, -Vector3.right * 0.1f, logcolor, duration);
		Debug.DrawRay(point, -Vector3.up * 0.1f, logcolor, duration);
	}
	
	
		
	public static void DrawPoint(Vector3 point, Transform relative_transf, float duration = 60)
	{
		Debug.DrawRay(point, relative_transf.forward * 0.1f, Color.cyan, duration);
		Debug.DrawRay(point, relative_transf.right * 0.1f, Color.red, duration);
		Debug.DrawRay(point, relative_transf.up * 0.1f, Color.green, duration);
		
		Debug.DrawRay(point, -relative_transf.forward * 0.1f, Color.blue, duration);
		Debug.DrawRay(point, -relative_transf.right * 0.1f, Color.magenta, duration);
		Debug.DrawRay(point, -relative_transf.up * 0.1f, Color.yellow, duration);
	}
	
	public static void DrawRay(Vector3 origin, Vector3 direction, Color? color = null, float duration = 60)
	{
		Debug.DrawRay(origin, direction, color ?? Color.magenta, duration);
	}
	
	public static void DrawLine(Vector3 start, Vector3 end, Color? color = null, float duration = 60)
	{
		Debug.DrawLine(start, end, color ?? Color.magenta, duration);
	}
	
	public static void DebugLog_Colored(string message, Color? color = null)
	{
		Color logcolor = color ?? Color.white;
		string hexColor = ColorUtility.ToHtmlStringRGB(logcolor);
		Debug.Log($"<color=#{hexColor}>{message}</color>");
	}
	
	public static Color orange => new Color(1f, 0.5f, 0f);
	public static Color lightGreen => new Color(0.56f, 0.93f, 0.56f);
	public static Color pink => new Color(1f, 0.41f, 0.71f);
	public static Color purple => new Color(0.5f, 0f, 0.5f);
	public static Color cyan => new Color(0f, 1f, 1f); 
	public static Color magenta => new Color(1f, 0f, 1f);
	public static Color gold => new Color(1f, 0.84f, 0f); 
	public static Color lime => new Color(0.5f, 1f, 0f); 
	public static Color navy => new Color(0f, 0f, 0.5f);
	public static Color maroon => new Color(0.5f, 0f, 0f);
	public static Color olive => new Color(0.5f, 0.5f, 0f);
	public static Color teal => new Color(0f, 0.5f, 0.5f); 
	public static Color silver => new Color(0.75f, 0.75f, 0.75f);
	public static Color darkGray => new Color(0.25f, 0.25f, 0.25f);
	public static Color lightGray => new Color(0.83f, 0.83f, 0.83f);
	public static Color beige => new Color(0.96f, 0.96f, 0.86f); 
	public static Color coral => new Color(1f, 0.5f, 0.31f); 
	public static Color turquoise => new Color(0.25f, 0.88f, 0.82f);
	public static Color lavender => new Color(0.9f, 0.9f, 0.98f);

}
