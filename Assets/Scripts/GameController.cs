using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
	public float roundLength;
	public GUIStyle scoreTextStyle;
	public GUIStyle timerTextStyle;
	public GUIStyle roundOverTextStyle;
	public GUIStyle roundOverScoreTextStyle;
	public GUIStyle newHighScoreTextStyle;
	public GUIStyle highScoreTableBGStyle;
	public GUIStyle highScoreTableTextStyle;
	
	[HideInInspector]
	public static int score;
	private float gameTimer;
	private string playerName;
	private List<Scores> highScores;
	private bool restartingGame;
	private bool gotNewHighScore
	{
		get { return score > 0 && (highScores != null && (highScores.Count < HighScoreManager.LeaderboardLength || score > highScores[highScores.Count - 1].score)); }
	}
		
	public enum GameState
	{
		intro,
		play,
		roundOver
	}
    public static GameState state;
	
	// Use this for initialization
	void Start()
	{
		score = 0;
		gameTimer = 0;
		state = GameState.play;
		
		// Debug code
		//HighScoreManager.instance.ClearLeaderBoard();
		//EndRound();
	}
	
	void Update()
	{
		if(state == GameState.play)
		{
			gameTimer += Time.deltaTime;
			if(gameTimer >= roundLength)
			{
				EndRound();
			}
			
			if(Input.GetKeyDown(KeyCode.Escape))
				Application.LoadLevel("MenuScene");
		}
		else if(state == GameState.roundOver)
		{
			if(Input.GetKeyDown(KeyCode.Escape))
			{
				Application.Quit();
			}
			else if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
			{
				RestartGame();
			}
		}
	}
	
	void EndRound()
	{
		state = GameState.roundOver;
		highScores = HighScoreManager.instance.GetHighScore();
		playerName = "Player Name";
	}
	
	void RestartGame()
	{
		if(gotNewHighScore && !restartingGame)
			HighScoreManager.instance.SaveHighScore(playerName, score);
		
		Application.LoadLevel("MenuScene");
		restartingGame = true;
	}
	
	// Update is called once per frame
	void OnGUI()
	{
		switch(state)
		{
		case GameState.intro:	
			DrawIntroUI();
			break;
		case GameState.play:
			DrawPlayUI();
			break;
		case GameState.roundOver:
			DrawRoundOverUI();
			break;
		}
	}
	
	void DrawIntroUI()
	{
		
	}
	
	void DrawPlayUI()
	{
		GUI.Label(new Rect(10, 10, 100, 100), "Score: " + score, scoreTextStyle);
		GUI.Label(new Rect(/*Screen.width / 2 - 50*/ 10, 35, 100, 100), (roundLength - gameTimer).ToString("F1"), timerTextStyle);
	}
	
	void DrawRoundOverUI()
	{
		GUI.Label(new Rect(Screen.width / 2 - 50, 10, 100, 100), "Round Over!", roundOverTextStyle);
		GUI.Label(new Rect(Screen.width / 2 - 50, 50, 100, 100), (gotNewHighScore ? "New High Score: " : "Score: ") + score, roundOverScoreTextStyle);
		
		if(gotNewHighScore)
		{
			GUI.SetNextControlName("PlayerInputControl");
			Event e = Event.current;
        	if (e.keyCode == KeyCode.Return)
			{
				RestartGame();
			}
			else if (e.keyCode == KeyCode.Escape)
			{
				Application.Quit();
			}
        	else
			{
				Vector2 textSize = newHighScoreTextStyle.CalcSize(new GUIContent(playerName));
				playerName = GUI.TextField(new Rect(Screen.width / 2 - (textSize.x / 2), 110, textSize.x, textSize.x), playerName, newHighScoreTextStyle);
			}
			GUI.FocusControl("PlayerInputControl");
		}
		
		if(GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height - 60, 100, 40), "Continue"))
			RestartGame();
		
		// Draw High Scores
		string highScoresText = "";
		for(var i = 0; i < HighScoreManager.LeaderboardLength; ++i)
		{
			if(i < highScores.Count)
				highScoresText += highScores[i].name + " - " + highScores[i].score;
			else
				highScoresText += "---";
			
			if(i < HighScoreManager.LeaderboardLength - 1)
				highScoresText += "\n";
		}
		
		if(highScoresText != "")
		{
			Vector2 textSize = highScoreTableTextStyle.CalcSize(new GUIContent(highScoresText));
			float boxWidth = Mathf.Max(textSize.x * 1.5f, 100);
			GUI.Box(new Rect(Screen.width / 2 - (boxWidth / 2), 160, boxWidth, 215), "High Scores:");
			GUI.Label(new Rect(Screen.width / 2 - (textSize.x / 2), 185, textSize.x, textSize.y), highScoresText, highScoreTableTextStyle);
		}
	}
}
