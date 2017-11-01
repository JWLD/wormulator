using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
	[Header("Main Menu Worms")]
	public static string gameMode;
	public GameObject playerTop;
	public GameObject playerMid;
	public GameObject playerBot;

	[Header("Menus")]
	public GameObject settingsMenu;
	public GameObject highScoreMenu;
	public static bool gamePause = false;

	[Header("Buttons")]
	public GameObject highScoreBackB;
	public GameObject highScoreRestartB;
	public Text highScoreButtonText;

	[Header("Scripts & Audio")]
	public AudioClip slimeAudio;
	public AudioClip metalClank;
	public AudioClip levelChange;

	/***** SCENE LOADING *****/

	// loads a game mode from the main menu
	public void LoadGameMode()
	{
		// loads HUNT Mode
		if (playerTop.transform.position == new Vector3(5, 1, 0))
		{
			SoundPlayer.instance.PlaySingle1(levelChange);
			gameMode = "hunt";
			ClickMover.body.Clear();
			SceneManager.LoadScene(1);
			gamePause = false;
		}

		// loads SOLO Mode
		if (playerMid.transform.position == new Vector3(5, -1, 0))
		{
			SoundPlayer.instance.PlaySingle1(levelChange);
			gameMode = "solo";
			ClickMover.body.Clear();
			SceneManager.LoadScene(1);
			gamePause = false;
		}

		// loads FLOOD Mode
		if (playerBot.transform.position == new Vector3(5, -3, 0))
		{
			SoundPlayer.instance.PlaySingle1(levelChange);
			gameMode = "flood";
			ClickMover.body.Clear();
			SceneManager.LoadScene(1);
			gamePause = false;
		}
	}
		
	// loads the main menu
	public void LoadMainMenu(int sceneBuildIndex)
	{
		SoundPlayer.instance.PlaySingle1(slimeAudio);
		SceneManager.LoadScene(0);
	}

	// restarts the game (without changing the gameMode)
	public void RestartGame()
	{
		SoundPlayer.instance.PlaySingle1(slimeAudio);
		ClickMover.body.Clear();
		SceneManager.LoadScene(1);
		GameController.gameOver = false;
		gamePause = false;
	}

	/***** INGAME MENUS *****/

	// resume game (from any menu)
	public void ResumeGame()
	{
		SoundPlayer.instance.PlaySingle1(slimeAudio);
		gamePause = false;
		settingsMenu.SetActive(false);
		highScoreMenu.SetActive(false);
	}

	// load settings menu
	public void LoadSettingsMenu()
	{
		SoundPlayer.instance.PlaySingle1(metalClank);
		gamePause = true;
		settingsMenu.SetActive(true);
	}

	// load high score menu
	public void LoadHSMenu(int origin)
	{
		SoundPlayer.instance.PlaySingle1(slimeAudio);
		highScoreMenu.SetActive(true);

		// if opened from main menu
		if (origin == 0)
		{
			highScoreBackB.SetActive(true);
			highScoreRestartB.SetActive(false);
			highScoreButtonText.fontSize = 50;
			highScoreButtonText.text = "BACK";
		}
		// if opened from gameover / new highscore
		else if (origin == 1)
		{
			highScoreBackB.SetActive(false);
			highScoreRestartB.SetActive(true);
			highScoreButtonText.fontSize = 37;
			highScoreButtonText.text = "RESTART";
		}
	}
		
	// go back from highscore menu
	public void HSMenuBack()
	{
		SoundPlayer.instance.PlaySingle1(slimeAudio);
		highScoreMenu.SetActive(false);
	}

	// load and close settings menu
	public void LoadSettingsMenuMain()
	{
		SoundPlayer.instance.PlaySingle1(slimeAudio);

		if (settingsMenu.activeSelf == false)
		{
			SoundPlayer.instance.PlaySingle1(metalClank);
			settingsMenu.SetActive(true);
		}
		else
			settingsMenu.SetActive(false);
	}

	// load and close high score menu
	public void LoadHSMenuMain()
	{
		SoundPlayer.instance.PlaySingle1(slimeAudio);

		if (highScoreMenu.activeSelf == false)
			highScoreMenu.SetActive(true);
		else
			highScoreMenu.SetActive(false);
	}
}