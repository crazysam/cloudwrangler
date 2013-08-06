using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour 
{
	public GUIStyle titleTextStyle;
	public GUIStyle howToPlayTextStyle;
	public GUIStyle controlsTextStyle;
	public Texture howToPlayStep1;
	public Texture howToPlayStep2;
	
	// Use this for initialization
	void Start()
	{
	
	}
	
	// Update is called once per frame
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
		else if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
		{
			LoadGame();
		}
	}
	
	void LoadGame()
	{
		Application.LoadLevel("GameScene");
	}
	
	void OnGUI()
	{
		string txt = "Cloud Wrangler";
		Vector2 textSize = titleTextStyle.CalcSize(new GUIContent(txt));
		GUI.Label(new Rect(Screen.width / 2 - (textSize.x / 2), 20, textSize.x, textSize.x), txt, titleTextStyle);
		
		txt = "How to Play:";
		textSize = howToPlayTextStyle.CalcSize(new GUIContent(txt));
		GUI.Label(new Rect(Screen.width / 2 - (textSize.x / 2), 180, textSize.x, textSize.x), txt, howToPlayTextStyle);
		
		txt = "WASD - Move Cowboy\nLeftClick - Use Lasso";
		textSize = controlsTextStyle.CalcSize(new GUIContent(txt));
		GUI.Label(new Rect(Screen.width / 2 - (textSize.x / 2), 230, textSize.x, textSize.x), txt, controlsTextStyle);
		
		GUI.DrawTexture(new Rect(Screen.width / 2 - 400, 120, 507/2,599/2), howToPlayStep1);
		GUI.DrawTexture(new Rect(Screen.width / 2 + 160, 140, 499/1.8f,526/1.8f), howToPlayStep2);
		
		if(GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height - 60, 100, 40), "Play"))
			LoadGame();
	}
}
