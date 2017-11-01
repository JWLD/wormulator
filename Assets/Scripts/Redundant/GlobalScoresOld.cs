using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalScoresOld : MonoBehaviour 
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

	// arrays for storing scores in-game
	public topScoresGlobal[] huntScoresGlobal;
	public topScoresGlobal[] soloScoresGlobal;
	public topScoresGlobal[] floodScoresGlobal;

//	[Header("Global Scores")]
//	public List<topScores> huntScoresList = new List<topScores>();
//	public List<topScores> soloScoresList = new List<topScores>();
//	public List<topScores> floodScoresList = new List<topScores>();

	void Awake()
	{
//		AddNewScore("Jack", 5000, privateCodeH);
//		AddNewScore("Tom", 4000, privateCodeS);
//		AddNewScore("Ben", 3000, privateCodeF);

		// download scores for all 3 game modes on awake, then convert them to lists in the highScore script
		DownloadScores(publicCodeH);
		DownloadScores(publicCodeS);
		DownloadScores(publicCodeF);
	}

	public void AddNewScore(string username, int score, string privateCode)
	{
		StartCoroutine(UploadNewScore(username, score, privateCode));
	}

	IEnumerator UploadNewScore(string username, int score, string privateCode)
	{
		WWW www = new WWW(webURL + privateCode + "/add/" + WWW.EscapeURL(username) + "/" + score);

		yield return www;

		if (string.IsNullOrEmpty(www.error))
			print("Upload successful.");
		else
			print("Error uploading. " +www.error);
	}

	public void DownloadScores(string publicCode)
	{
		StartCoroutine(DownloadScoresFromDatabase(publicCode));
	}

	IEnumerator DownloadScoresFromDatabase(string publicCode)
	{
		WWW www = new WWW(webURL + publicCode + "/pipe/");

		yield return www;

		if (string.IsNullOrEmpty(www.error))
			FormatGlobalScores(www.text, publicCode);
		else
			print("Error downloading. " +www.error);
	}

	void FormatGlobalScores(string textStream, string publicCode)
	{
		string[] entries = textStream.Split(new char[] {'\n'}, System.StringSplitOptions.RemoveEmptyEntries);

		if (publicCode == publicCodeH)
			huntScoresGlobal = new topScoresGlobal[entries.Length];
		else if (publicCode == publicCodeS)
			soloScoresGlobal = new topScoresGlobal[entries.Length];
		else if (publicCode == publicCodeF)
			floodScoresGlobal = new topScoresGlobal[entries.Length];

		for (int i = 0; i < entries.Length; i++)
		{
			string[] entryInfo = entries[i].Split(new char[] {'|'});

			string name = entryInfo[0];
			int score = int.Parse(entryInfo[1]);

			if (publicCode == publicCodeH)
				huntScoresGlobal[i] = new topScoresGlobal(name, score);
			else if (publicCode == publicCodeS)
				soloScoresGlobal[i] = new topScoresGlobal(name, score);
			else if (publicCode == publicCodeF)
				floodScoresGlobal[i] = new topScoresGlobal(name, score);

//			print(huntScoresGlobal[i].name + ": " +huntScoresGlobal[i].score);
		}			
	}

	// converts the array data from the global script into a list that can be used in this script (there must be an alternative...)
//	public void ConvertScoreArrayToList()
//	{
//		// convert global hunt scores
//		for (int i = 0; i < huntScoresGlobal.Length; i++)
//		{
//			huntScoresList[i].name = huntScoresGlobal[i].name;
//			huntScoresList[i].score = huntScoresGlobal[i].score;
//		}
//
//		// convert solo hunt scores
//		for (int i = 0; i < soloScoresGlobal.Length; i++)
//		{
//			soloScoresList[i].name = soloScoresGlobal[i].name;
//			soloScoresList[i].score = soloScoresGlobal[i].score;
//		}
//
//		// convert flood hunt scores
//		for (int i = 0; i < floodScoresGlobal.Length; i++)
//		{
//			floodScoresList[i].name = floodScoresGlobal[i].name;
//			floodScoresList[i].score = floodScoresGlobal[i].score;
//		}
//	}

	public struct topScoresGlobal
	{
		public string name;
		public int score;

		public topScoresGlobal(string _name, int _score)
		{
			name = _name;
			score = _score;
		}
	}

	// class and struct for storing highscore info
	[System.Serializable]
	public class topScores
	{
		public string name;
		public int score;
	}
}