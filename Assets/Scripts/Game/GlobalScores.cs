using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GlobalScores : MonoBehaviour 
{
	// normal mode scores
	const string privateCodeH = "fgUaU_xpSkesEvOdD9l24wDPOLcgYi00-JY2EyIdbfQg";
	const string publicCodeH = "570cdfd56e51b618b0ce21dd";
	const string webURL = "http://dreamlo.com/lb/";

	// solo mode scores
	const string privateCodeS = "v9rFzsBFqkyd4zGQIHotkgNfL-lywMekigi3-OSCQVFQ";
	const string publicCodeS = "570f79d46e51b61b3c611525";

	// flood mode scores
	const string privateCodeF = "MHpyQk2SB0WzreNnE9lKtAtqI6MRJ1skywkVs1Z8AYHg";
	const string publicCodeF = "570f79e56e51b61b3c611540";

	[Header("Global Scores")]
	public List<topScores> huntScoresG = new List<topScores>();
	public List<topScores> soloScoresG = new List<topScores>();
	public List<topScores> floodScoresG = new List<topScores>();
//	private int scoreArrayIndex;

	[Header("UI Buttons")]
	public Button huntButton;
	public Button soloButton;
	public Button floodButton;

	[Header("Scripts")]
	public LocalScores localScores;

	void Start()
	{
		// updates all scores from PlayerPrefs to online at start
		UploadAllScores();

		// download scores for all 3 game modes on awake
		DownloadScores(publicCodeH);
		DownloadScores(publicCodeS);
		DownloadScores(publicCodeF);
	}

	void UploadAllScores()
	{
		for (int i = 0; i < 5; i++)
		{
			if (localScores.huntScores[i].score > 0 && localScores.huntScores[i].upload == 0) {
				AddNewScore(localScores.huntScores[i].name, localScores.huntScores[i].score, privateCodeH, localScores.huntScores[i].code, i);
			}
			if (localScores.soloScores[i].score > 0 && localScores.soloScores[i].upload == 0) {
				AddNewScore(localScores.soloScores[i].name, localScores.soloScores[i].score, privateCodeS, localScores.soloScores[i].code, i);
			}
			if (localScores.floodScores[i].score > 0 && localScores.floodScores[i].upload == 0) {
				AddNewScore(localScores.floodScores[i].name, localScores.floodScores[i].score, privateCodeF, localScores.floodScores[i].code, i);
			}
		}
	}

	public void AddNewScore(string username, int score, string privateCode, int nameCode, int arrayIndex)
	{
		if (privateCode == null)
		{
			if (SceneLoader.gameMode == "hunt")
				privateCode = privateCodeH;
			else if (SceneLoader.gameMode == "solo")
				privateCode = privateCodeS;
			else if (SceneLoader.gameMode == "flood")
				privateCode = privateCodeF;
		}
		
		StartCoroutine(UploadNewScore(username, score, privateCode, nameCode, arrayIndex));
	}

	IEnumerator UploadNewScore(string username, int score, string privateCode, int nameCode, int arrayIndex)
	{
		// attaches a random code to the username to allow duplicates
		string codedUsername = username + "-" +nameCode;

		WWW www = new WWW(webURL + privateCode + "/add/" + WWW.EscapeURL(codedUsername) + "/" + score + "/" + 0 + "/" + WWW.EscapeURL(username));

		yield return www;

		if (string.IsNullOrEmpty(www.error))
		{
			print("Upload successful.");

			// remember that this score has been uploaded so it's never uploaded twice
			if (privateCode == privateCodeH)
				localScores.huntScores[arrayIndex].upload = 1;
			else if (privateCode == privateCodeS)
				localScores.soloScores[arrayIndex].upload = 1;
			else if (privateCode == privateCodeF)
				localScores.floodScores[arrayIndex].upload = 1;

			localScores.SaveScoresToPlayerPrefs();
		}
		else
		{
			print("Error uploading. " +www.error);
		}

		// download the relevant scores after updating
		if (privateCode == privateCodeH)
			DownloadScores(publicCodeH);
		else if (privateCode == privateCodeS)
			DownloadScores(publicCodeS);		
		else if (privateCode == privateCodeF)
			DownloadScores(publicCodeF);		
	}

	public void DownloadScores(string publicCode)
	{
		StartCoroutine(DownloadScoresFromDatabase(publicCode));
	}

	IEnumerator DownloadScoresFromDatabase(string publicCode)
	{
		WWW www = new WWW(webURL + publicCode + "/pipe/" + 5);

		yield return www;

//		print(www.text);

		if (string.IsNullOrEmpty(www.error))
		{
			if (publicCode == publicCodeH)
				FormatGlobalScores(www.text, huntScoresG);
			else if (publicCode == publicCodeS)
				FormatGlobalScores(www.text, soloScoresG);
			else if (publicCode == publicCodeF)
				FormatGlobalScores(www.text, floodScoresG);
		}
		else
		{
			print("Error downloading. " +www.error);

			for (int i = 0; i < 5; i++)
			{
				huntScoresG[i].name = "ERROR404";
				soloScoresG[i].name = "ERROR404";
				floodScoresG[i].name = "ERROR404";
			}
		}
	}

	void FormatGlobalScores(string textStream, List<topScores> scoreList)
	{
		string[] entries = textStream.Split(new char[] {'\n'}, System.StringSplitOptions.RemoveEmptyEntries);

		for (int i = 0; i < entries.Length && i < 5; i++)
		{
			string[] entryInfo = entries[i].Split(new char[] {'|'});

			int score = int.Parse(entryInfo[1]);
			string name = entryInfo[3];

			scoreList[i].name = name;
			scoreList[i].score = score;
		}			
	}

	void DisplayScores(List<topScores> scoreList, Button scoreButton)
	{
		if (scoreButton == huntButton)
			localScores.currentPage = "hunt";
		else if (scoreButton == soloButton)
			localScores.currentPage = "solo";
		else if (scoreButton == floodButton)
			localScores.currentPage = "flood";

		// sets alpha of buttons
		huntButton.image.color = new Color(1f, 1f, 1f, 0.25f);
		soloButton.image.color = new Color(1f, 1f, 1f, 0.25f);
		floodButton.image.color = new Color(1f, 1f, 1f, 0.25f);
		scoreButton.image.color = new Color(1f, 1f, 1f, 1f);

		// populates text objects with the relevant score
		for (int i = 0; i < 5; i++)
		{
			if (scoreList[i].name == "ERROR404")
				localScores.highScoreText[i].text = (i + 1) +". ERROR DOWNLOADING";
			else if (scoreList[i].score > 0)
				localScores.highScoreText[i].text = (i + 1) +". " +scoreList[i].name +" - " +scoreList[i].score;
			else
				localScores.highScoreText[i].text = (i + 1) +".";
		}
	}

	// public functions to call from LocalScores script
	public void DisplayHuntScores() {
		DisplayScores(huntScoresG, huntButton);
	}

	public void DisplaySoloScores() {
		DisplayScores(soloScoresG, soloButton);
	}

	public void DisplayFloodScores() {
		DisplayScores(floodScoresG, floodButton);
	}

	// class and struct for storing highscore info
	[System.Serializable]
	public class topScores
	{
		public string name;
		public int score;
	}
}