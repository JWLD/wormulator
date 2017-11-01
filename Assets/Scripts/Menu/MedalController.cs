using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MedalController : MonoBehaviour
{
	[Header("Score Goals")]
	public int[] huntGoals;
	public int[] soloGoals;
	public int[] floodGoals;
	public static int huntGold;
	public static int soloGold;
	public static int floodGold;

	[Header("Audio")]
	public AudioClip slimeAudio;
	public AudioClip medalPing;

	[Header("Medal Images")]
	public Image huntMedal;
	public Image soloMedal;
	public Image floodMedal;

	[Header("Sprites")]
	public Sprite bronzeMedal;
	public Sprite silverMedal;
	public Sprite goldMedal;

	[Header("Top Score Text")]
	public Text huntText;
	public Text soloText;
	public Text floodText;

	[Header("Goal List")]
	public GameObject goalList;
	public Text bronzeGoalText;
	public Text silverGoalText;
	public Text goldGoalText;

	[Header("Goal Buttons")]
	public Button huntButton;
	public Button soloButton;
	public Button floodButton;

	[Header("Goal Explanations")]
	public GameObject huntExpl;
	public GameObject soloExpl;
	public GameObject floodExpl;

	void Start()
	{
		huntText.text = "" +PlayerPrefs.GetInt("huntScore0");
		soloText.text = "" +PlayerPrefs.GetInt("soloScore0");
		floodText.text = "" +PlayerPrefs.GetInt("floodScore0");

		CheckScore(huntMedal, huntGoals, "huntScore0");
		CheckScore(soloMedal, soloGoals, "soloScore0");
		CheckScore(floodMedal, floodGoals, "floodScore0");

		huntGold = huntGoals[2];
		soloGold = soloGoals[2];
		floodGold = floodGoals[2];
	}

	void CheckScore(Image medalSprite, int[] goals, string topScore)
	{
		if (PlayerPrefs.GetInt(topScore) >= goals[2])
			medalSprite.sprite = goldMedal;
		else if (PlayerPrefs.GetInt(topScore) >= goals[1])
			medalSprite.sprite = silverMedal;
		else if (PlayerPrefs.GetInt(topScore) >= goals[0])
			medalSprite.sprite = bronzeMedal;
		else
			medalSprite.color = new Color(1f, 1f, 1f, 0f);
	}

	public void OpenHelpPanel(string origin)
	{
		if (goalList.activeSelf == false)
			goalList.SetActive(true);

		if (origin == "hunt" || origin == "solo" || origin == "flood")
			SoundPlayer.instance.PlaySingle1(slimeAudio);
		else
			SoundPlayer.instance.PlayNoPitchVariation(medalPing);

		if (origin == "hunt" || origin == "huntMedal")
			SetupHelpPanel(huntGoals, huntButton, huntExpl);
		else if (origin == "solo" || origin == "soloMedal")
			SetupHelpPanel(soloGoals, soloButton, soloExpl);
		else if (origin == "flood" || origin == "floodMedal")
			SetupHelpPanel(floodGoals, floodButton, floodExpl);
	}

	public void SetupHelpPanel(int[] goals, Button goalButton, GameObject explanation)
	{
		// setup goal text
		bronzeGoalText.text = "" +goals[0];
		silverGoalText.text = "" +goals[1];
		goldGoalText.text = "" +goals[2];

		// set up explanations
		huntExpl.SetActive(false);
		soloExpl.SetActive(false);
		floodExpl.SetActive(false);
		explanation.SetActive(true);

		// change button colour
		huntButton.image.color = new Color(0.7f, 0.7f, 0.7f, 1f);
		soloButton.image.color = new Color(0.7f, 0.7f, 0.7f, 1f);
		floodButton.image.color = new Color(0.7f, 0.7f, 0.7f, 1f);
		goalButton.image.color = new Color(1f, 1f, 1f, 1f);
	}

	public void CloseHelpPanel()
	{
		SoundPlayer.instance.PlaySingle1(slimeAudio);
		goalList.SetActive(false);
	}
}