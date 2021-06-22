using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	private int level = 1;
	private int score = 0;
	[SerializeField]
	private Text scoreText;
	[SerializeField]
	private GameObject statusPanel;
	[SerializeField]
	private Text gameStatusText;
	[SerializeField]
	private Text levelText;
	[SerializeField]
	private Text gameStartCountDown;
	// Start is called before the first frame update
	void Start()
	{
		GameManager.instance.scoreUpdate += UpdateScore;
		GameManager.instance.gameStatus += UpdateGameStatus;
	}
	void UpdateScore()
	{
		score += 10;
		scoreText.text = "Score:" + score.ToString();
	}
	void UpdateGameStatus(bool isWon)
	{
		if (!isWon)
		{
			score = 0;
			scoreText.text = "";
			gameStatusText.text = "YOU LOST";
		}
		else
		{
			level += 1;
			gameStatusText.text = "YOU WON";
		}
		GameManager.instance.RestartGame();
	}
	public IEnumerator LevelIntro(System.Action action)
	{
		statusPanel.SetActive(true);
		float introTimer = 3;
		while (introTimer > 0)
		{
			gameStartCountDown.text = "STARTS IN:" + ((int)introTimer).ToString();
			levelText.text = "LEVEL:" + level;
			introTimer -= Time.deltaTime;
			yield return null;
		}
		statusPanel.SetActive(false);
		action?.Invoke();
	}
}
