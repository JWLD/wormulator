/* Determines which elements of the game are active at startup, and manages them throughout the game.
 * Controls what happens when all food is eaten and the player moves to a new level.
 * Manages game over behaviour. */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour 
{
	[Header("Active Objects")]
	public bool molesActive;
	public bool smartMove;
	public bool hazardsActive;
	public bool floodActive;

	[Header("Object Setup")]
	public int startChunks;
	public Vector3[] moleResetPos;

	[Header("Speeds & Timers")]
	public float moleSpeed;
	public float restartDelay;
	public float bloodFade;
	public float invisTimer;

	[Header("In-Game Variables")]
	public int level = 1;
	public int foodCount = 5;
	public static bool gameOver = false;

	[Header("UI")]
	public Text levelText;
	public Text scoreText;
	private bool restarted = false;
	private float restartTimer;

	[Header("Objects & Prefabs")]
	public GameObject player;
	public GameObject[] moles;
	public GameObject scubaMole;
	public GameObject[] hazards;
	public GameObject floodWaters;
	public GameObject debris;
	public GameObject fallShadow;
	public GameObject shadowObject;

	[Header("Game Over Screen")]
	public int finalScore;
	public GameObject gameOverPanel;
	public Text gameOverText;
	public Text finalScoreText1;
	public Text finalScoreText2;
	public Button restartButton;
	public GameObject restartButtonText;
	public Button settingsButton;

	[Header("Other")]
	public bool firstMove = false;
	public List<GameObject> instaDestroy = new List<GameObject>();

	[Header("Audio & Scripts")]
	public AudioClip whooshAudio;
	public LocalScores localScoreScript;
	public GlobalScores globalScoreScript;
	public FoodSetup foodSetupScript;
	public BoardManager boardManagerScript;
	public SoundManager soundManager;
	public HeadCollision headCollision;
	public AdController adController;

	void Start()
	{
		// lets game run at max frame rate
		Application.targetFrameRate = -1;

		// sets up game components depending on the game mode
		if (SceneLoader.gameMode == "hunt")
		{
			molesActive = true;
			hazardsActive = false;
			floodActive = false;
		}
		else if (SceneLoader.gameMode == "solo")
		{
			hazardsActive = true;
			molesActive = false;
			floodActive = false;
		}
		else if (SceneLoader.gameMode == "flood")
		{
			floodActive = true;
			molesActive = false;
			hazardsActive = false;

			floodWaters.SetActive(true);
			scubaMole.SetActive(true);
		}
	}

	// used here for game over
	void Update() 
	{
		// waits a specific number of seconds before showing the gameover screen
		if (gameOver == true)
		{
			restartTimer += Time.deltaTime;

			// disables the settings button in between gameover and final screen
			if (settingsButton.interactable == true)
				settingsButton.interactable = false;

			if (restartTimer >= restartDelay && restarted == false)
			{
				restarted = true;
				soundManager.ResumeNormalMusic();
				ShowFinalScreen(false, 0);
			}
		}

		// random score for testing
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			SceneLoader.gameMode = "hunt";
			bool testMode = true;
			int testScore = Random.Range(10, 100);
			ShowFinalScreen(testMode, testScore);
		}
	}

	// checks whether all food has been eaten and if so creates a hole to the next level
	public void MakeHole()
	{
		// if all food has been eaten
		if (foodCount == 0)
		{
			// configures hazards if playing solo mode
//			if (SceneLoader.gameMode == "solo")
//			{
//				for (int i = 0; i < hazards.Length; i++)
//				{
//					hazards[i].GetComponent<HazardBehaviour>().LevelComplete();
//				}
//			}

			// calls script to set up hole to next level
			boardManagerScript.LevelComplete();
		}
	}

	public void ChangeLevel()
	{
		// sets true to temporarily enable movement to previous position (see ClickMover)
		firstMove = true;
		headCollision.StopCountdown();

		// increases level, adjusts UI, sets up game board
		level++;
		levelText.text = "L" +level;
		boardManagerScript.ChangeToNextMaze(level);

		// increases food count by one each level if playing flood mode
		if (SceneLoader.gameMode == "flood")
		{
			foodCount = 5 + (level - 1);
		}
		// otherwise just resets it to 5
		else
		{
			foodCount = 5;
		}

		// creates more food, and sets up hazards if playing solo mode
		foodSetupScript.ArrangeFood(foodCount);

		if (SceneLoader.gameMode == "solo")
		{
			for (int i = 0; i < hazards.Length; i++)
			{
				hazards[i].GetComponent<HazardBehaviour>().NewLevel();
			}
		}

		// resets mole positions if active
		if (molesActive == true)
		{
			for (int i = 0; i < 3; i++)
			{
				moles[i].transform.position = moleResetPos[i];
			}
		}
		else if (SceneLoader.gameMode == "flood")
		{
			scubaMole.transform.position = new Vector3(0f, 1f, 0f);
		}

		// resets flood waters if active
		if (floodActive == true)
		{
			floodWaters.transform.position = new Vector3(0f, -18f, 0f);
		}

		// instantly deletes blood splatter if still present from previous level
		if (instaDestroy.Count > 0)
		{
			for (int i = 0; i < instaDestroy.Count; i++)
			{
				Destroy(instaDestroy[i]);
			}

			// clears list
			instaDestroy.Clear();
		}

		// moves body parts off the screen
		for (int i = 0; i < ClickMover.body.Count; i++)
		{
			ClickMover.body[i].transform.position = new Vector3(-14f, 0f, 0f);
		}

		// play levelChange audio and instanties debris at the player's new position
		SoundPlayer.instance.PlaySingle3(whooshAudio);
		Instantiate(debris, player.transform.position, Quaternion.identity);
		GameObject instance = Instantiate(fallShadow, player.transform.position, Quaternion.identity) as GameObject;
		instaDestroy.Add(instance);
		shadowObject = instance;
	}

	// keeps the score updated
	public void UpdateScore()
	{
		if (headCollision.bombActive == false)
			scoreText.text = "" +ClickMover.body.Count();
	}

	// calculates final score, displays it to the user - text changes if it's a new record
	void ShowFinalScreen(bool test, int testScore)
	{
		PrepareAd();

		// calculates final score
		if (test == true)
			finalScore = testScore;
		else
			finalScore = level * ClickMover.body.Count;

		// displays the final score on screen
		gameOverPanel.SetActive(true);
		finalScoreText1.text = "SCORE: " +level +" x " +ClickMover.body.Count;
		finalScoreText2.text = "= " +finalScore;

		// there's a new high score
		if ((SceneLoader.gameMode == "hunt" && finalScore > localScoreScript.huntScores[4].score) || 
			(SceneLoader.gameMode == "solo" && finalScore > localScoreScript.soloScores[4].score) ||
			(SceneLoader.gameMode == "flood" && finalScore > localScoreScript.floodScores[4].score))
		{
			// temporarily disables the restart button (to force the user to enter their name)
			restartButtonText.SetActive(false);
			restartButton.interactable = false;
			gameOverText.text = "HIGH\nSCORE!";

			// update the high score lists
			localScoreScript.AddNewHighScore(finalScore);
		}
		else
		{
			gameOverText.text = "GAME\nOVER";
		}
	}

	public void CheckShadow()
	{
		bool keepShadow = false;

		for (int i = 0; i < ClickMover.body.Count; i++)
		{
			if (ClickMover.body[i].transform.position == new Vector3(-14f, 0f, 0f))
				keepShadow = true;
		}

		if (keepShadow == false)
			Destroy(shadowObject);
	}

	// shows an add on every nth gameover screen
	void PrepareAd()
	{
//		print("AdCount = " +PlayerPrefs.GetInt("AdCounter"));

		if (PlayerPrefs.GetInt("AdCounter") >= adController.adFrequency - 1)
		{
			adController.ShowAd();
			PlayerPrefs.SetInt("AdCounter", 0);
		}
		else
		{
			PlayerPrefs.SetInt("AdCounter", PlayerPrefs.GetInt("AdCounter") + 1);
		}
	}
}