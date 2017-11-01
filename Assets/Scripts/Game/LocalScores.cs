using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.UI;
//using UnityEngine.SceneManagement;

public class LocalScores : MonoBehaviour 
{
	[Header("Local Scores")]
	public List<topScores> huntScores = new List<topScores>();
	public List<topScores> soloScores = new List<topScores>();
	public List<topScores> floodScores = new List<topScores>();

	[Header("New High Score Screen")]
	public InputField requestName;
	public Text submitButtonText;
	public Button restartButton;
	public GameObject restartButtonText;
	public GameObject gameOverPanel;
	private string newName;

	[Header("HighScore Screen UI")]
	public GameObject highScorePanel;
	public Button huntButton;
	public Button soloButton;
	public Button floodButton;
	public Text localGlobalText;
	public Text[] highScoreText;
	public string currentPage;
	static string lastScoreName = null;
	private bool localScores = true;
	public int randomCode;
	private bool profanityFound;

	[Header("Scripts & Audio")]
	public SceneLoader sceneLoaderScript;
	public GlobalScores globalScores;
	public GameController gameController;
	public AudioClip slimeAudio;

//	[Header("Objects")]
//	public GameObject helperArrows;

	void Start()
	{
		PopulateScoresFromPlayerPrefs();

		// display hunt scores by default
		DisplayScores(huntScores, huntButton);

		// deactivate helper arrows once all 3 game modes have been played
//		if (SceneManager.GetActiveScene().buildIndex == 0)
//		{
//			if (huntScores[0] > 0 && soloScores[0] > 0 && floodScores[0] > 0)
//				helperArrows.SetActive(false);
//		}
	}

	// keyboard inputs for testing
	void Update()
	{
		// clears all scores
		if (Input.GetKeyDown(KeyCode.Alpha0))
		{
			PlayerPrefs.DeleteAll();
			print("Scores cleared");
		}
	}

	// populates highScores from PlayerPrefs
	public void PopulateScoresFromPlayerPrefs()
	{
		for (int i = 0; i < 5; i++)
		{
			huntScores[i].name = PlayerPrefs.GetString("huntName" +i);
			huntScores[i].score = PlayerPrefs.GetInt("huntScore" +i);
			huntScores[i].code = PlayerPrefs.GetInt("huntCode" +i);
			huntScores[i].upload = PlayerPrefs.GetInt("huntUpload" +i); 

			soloScores[i].name = PlayerPrefs.GetString("soloName" +i);
			soloScores[i].score = PlayerPrefs.GetInt("soloScore" +i);
			soloScores[i].code = PlayerPrefs.GetInt("soloCode" +i);
			soloScores[i].upload = PlayerPrefs.GetInt("soloUpload" +i); 

			floodScores[i].name = PlayerPrefs.GetString("floodName" +i);
			floodScores[i].score = PlayerPrefs.GetInt("floodScore" +i);
			floodScores[i].code = PlayerPrefs.GetInt("floodCode" +i);
			floodScores[i].upload = PlayerPrefs.GetInt("floodUpload" +i); 
		}
	}

	// saves scores to PlayerPrefs
	public void SaveScoresToPlayerPrefs()
	{
		for (int i = 0; i < 5; i++)
		{
			PlayerPrefs.SetString("huntName" +i, huntScores[i].name);
			PlayerPrefs.SetInt("huntScore" +i, huntScores[i].score);
			PlayerPrefs.SetInt("huntCode" +i, huntScores[i].code);
			PlayerPrefs.SetInt("huntUpload" +i, huntScores[i].upload);

			PlayerPrefs.SetString("soloName" +i, soloScores[i].name);
			PlayerPrefs.SetInt("soloScore" +i, soloScores[i].score);
			PlayerPrefs.SetInt("soloCode" +i, soloScores[i].code);
			PlayerPrefs.SetInt("soloUpload" +i, soloScores[i].upload);

			PlayerPrefs.SetString("floodName" +i, floodScores[i].name);
			PlayerPrefs.SetInt("floodScore" +i, floodScores[i].score);
			PlayerPrefs.SetInt("floodCode" +i, floodScores[i].code);
			PlayerPrefs.SetInt("floodUpload" +i, floodScores[i].upload);
		}
	}

	// adds new score to the high score list, and saves a unique code for use with dreamlo
	public void AddNewHighScore(int finalScore)
	{
		randomCode = Random.Range(1, 99999);

		if (SceneLoader.gameMode == "hunt")
		{
			huntScores[5].score = finalScore;
			huntScores[5].code = randomCode;
		}
		else if (SceneLoader.gameMode == "solo")
		{
			soloScores[5].score = finalScore;
			soloScores[5].code = randomCode;
		}
		else if (SceneLoader.gameMode == "flood")
		{
			floodScores[5].score = finalScore;
			floodScores[5].code = randomCode;
		}

		requestName.gameObject.SetActive(true);

		// loads the last used name (if there is one)
		if (lastScoreName != null)
			requestName.text = lastScoreName;
	}

	// called when the user presses SUBMIT
	public void SubmitName()
	{
		// stores whatever the player typed in the box
		newName = (requestName.text).ToUpper();

		// check for profanity
		ProfanityCheck(newName);

		if (profanityFound)
		{
			profanityFound = false;
		}
		// checks that the user has only typed letters - then saves the name and sorts the list of scores
		else if (Regex.IsMatch(newName, @"^[a-zA-Z]+$"))
		{
			submitButtonText.text = "SUBMIT";

			// add name to list and sort by score
			if (SceneLoader.gameMode == "hunt")
			{
				huntScores[5].name = newName;
				huntScores.Sort((s1, s2) => s1.score.CompareTo(s2.score));
				huntScores.Reverse();
			}
			else if (SceneLoader.gameMode == "solo")
			{
				soloScores[5].name = newName;
				soloScores.Sort((s1, s2) => s1.score.CompareTo(s2.score));
				soloScores.Reverse();
			}
			else if (SceneLoader.gameMode == "flood")
			{
				floodScores[5].name = newName;
				floodScores.Sort((s1, s2) => s1.score.CompareTo(s2.score));
				floodScores.Reverse();
			}

			// remembers submitted name
			lastScoreName = newName;

			// upload to global scores, and then save all the info to PlayerPrefs
			globalScores.AddNewScore(newName, gameController.finalScore, null, randomCode, 5);
			SaveScoresToPlayerPrefs();

			// sorts out UI buttons
			requestName.gameObject.SetActive(false);
			restartButtonText.SetActive(true);
			restartButton.interactable = true;

			// opens high score window
			gameOverPanel.SetActive(false);
			sceneLoaderScript.LoadHSMenu(1);	

			// display the appropriate scores when opening high scores
			if (SceneLoader.gameMode == "hunt")
			{
				DisplayScores(huntScores, huntButton);
				currentPage = "hunt";
			}
			else if (SceneLoader.gameMode == "solo")
			{
				DisplayScores(soloScores, soloButton);
				currentPage = "solo";
			}
			else if (SceneLoader.gameMode == "flood")
			{
				DisplayScores(floodScores, floodButton);
				currentPage = "flood";
			}
		}
		// if the user has entered something other than a letter
		else
		{
			StartCoroutine(StyleWarning());
		}
	}

	IEnumerator StyleWarning()
	{
		submitButtonText.text = "A-Z ONLY";

		yield return new WaitForSeconds(2);

		submitButtonText.text = "SUBMIT";
	}
		
	public void ProfanityCheck(string name)
	{
		string[] swearWords = new string[] {"FUCK", "SHIT", "CUNT", "ARSE", "ASSHOLE", "BITCH", "BOLLOCK", "BELLEND", "BUGGER", "TWAT", "NIGGER", "BASTARD"};
		string[] replacements = new string[] {"RAINBOW", "FLUFFY", "PUPPIES", "KITTENS"};

		for (int i = 0; i < swearWords.Length; i++)
		{
			if (name.Contains(swearWords[i]))
			{
				profanityFound = true;
				requestName.text = replacements[Random.Range(0, replacements.Length)];
			}
		}
	}

	public void DisplayScores(List<topScores> scoreList, Button scoreButton)
	{
		if (scoreButton == huntButton)
			currentPage = "hunt";
		else if (scoreButton == soloButton)
			currentPage = "solo";
		else if (scoreButton == floodButton)
			currentPage = "flood";

		// sets alpha of buttons
		huntButton.image.color = new Color(1f, 1f, 1f, 0.25f);
		soloButton.image.color = new Color(1f, 1f, 1f, 0.25f);
		floodButton.image.color = new Color(1f, 1f, 1f, 0.25f);
		scoreButton.image.color = new Color(1f, 1f, 1f, 1f);

		// populates text objects with the relevant score
		for (int i = 0; i < 5; i++)
		{
			if (scoreList[i].score > 0)
				highScoreText[i].text = (i + 1) +". " +scoreList[i].name +" - " +scoreList[i].score;
			else
				highScoreText[i].text = (i + 1) +".";
		}
	}

	// functions for buttons
	public void DisplayHuntScores() 
	{
		SoundPlayer.instance.PlaySingle1(slimeAudio);

		if (localScores == true)
			DisplayScores(huntScores, huntButton);
		else
			globalScores.DisplayHuntScores();
	}

	public void DisplaySoloScores() 
	{
		SoundPlayer.instance.PlaySingle1(slimeAudio);

		if (localScores == true)
			DisplayScores(soloScores, soloButton);
		else
			globalScores.DisplaySoloScores();
	}

	public void DisplayFloodScores() 
	{
		SoundPlayer.instance.PlaySingle1(slimeAudio);

		if (localScores == true)
			DisplayScores(floodScores, floodButton);
		else
			globalScores.DisplayFloodScores();
	}

	// button to switch between global and local scores
	public void SwitchLocalGlobal()
	{
		SoundPlayer.instance.PlaySingle1(slimeAudio);

		// switch to global scores
		if (localScores == true)
		{
			localGlobalText.text = "Global";
			localScores = false;

			if (currentPage == "hunt")
				globalScores.DisplayHuntScores();
			else if (currentPage == "solo")
				globalScores.DisplaySoloScores();
			else if (currentPage == "flood")
				globalScores.DisplayFloodScores();
		}
		// switch to local scores
		else if (localScores == false)
		{
			localGlobalText.text = "Local";
			localScores = true;

			if (currentPage == "hunt")
				DisplayScores(huntScores, huntButton);
			else if (currentPage == "solo")
				DisplayScores(soloScores, soloButton);
			else if (currentPage == "flood")
				DisplayScores(floodScores, floodButton);
		}
	}

	// class and struct for storing highscore info
	[System.Serializable]
	public class topScores
	{
		public string name;
		public int score;
		public int code;
		public int upload;
	}
}