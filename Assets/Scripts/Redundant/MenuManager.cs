using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour 
{
	public GameObject settingsMenu;
	public GameObject highScoreMenu;
	public GameObject highScoreResumeB;
	[HideInInspector] public bool gamePause = false;

	// load settings menu
	public void LoadSettingsMenu()
	{
		gamePause = true;
		settingsMenu.SetActive(true);
	}

	// load high score menu
	public void LoadHSMenu()
	{
		highScoreMenu.SetActive(true);
	}

	// load highscore-restart menu
	public void HSMenuRestart()
	{
		highScoreMenu.SetActive(true);
		highScoreResumeB.SetActive(false);
	}

	// load highscore-back menu
	public void HSMenuBack()
	{
		highScoreMenu.SetActive(false);
	}

	// resume game (from any menu)
	public void ResumeGame()
	{
		gamePause = false;
		settingsMenu.SetActive(false);
		highScoreMenu.SetActive(false);
	}
}