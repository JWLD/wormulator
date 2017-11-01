using UnityEngine;
using System.Collections;

public class FloodControl : MonoBehaviour 
{
	[Header("Setup")]
	public float floodSpeed;
	public float drownTime;

	[Header("Variables")]
	public Vector3 floodStartPos;
	public float waterLine;
	public bool drowning = false;
	public float airAnimSpeed;

	[Header("Objects")]
	public GameObject player;
	public GameObject airPuff;
	public GameObject scoreText;
	public GameObject scoreHole;
	public Sprite drownedWorm;
	public Sprite drownedBodyReg;
	public Sprite drownedBodyGreen;

	[Header("Scripts")]
	public ClickMover clickMover;
	public GameController gameController;

	[Header("Audio")]
	public AudioClip splashIn;
	public AudioClip splashOut;
	public AudioClip gargle;

	// Animation
	private Animation floodAnimation;
	private IEnumerator playerDrowning;

	void Start()
	{
		// sets the start position of the flood waters
		transform.position = floodStartPos;

		// loads animation component
		floodAnimation = airPuff.GetComponent<Animation>();

		// pauses animation at start
		floodAnimation["airShrink"].speed = 0f;
	}

	void Update () 
	{
		// script runs while not gameover
		if (GameController.gameOver == false && gameController.floodActive == true)
		{
			// moves the floodWaters up at a steady rate
			if (transform.position.y < 0 && GameController.gameOver == false && SceneLoader.gamePause == false)
			{
				transform.Translate(Vector2.up * floodSpeed * Time.deltaTime);
			}

			// keeps track of the waterline
			waterLine = transform.position.y + 8.5f;

			if (player != null)
			{	
				// pauses and resumes animation when game is paused / resumed
				if (SceneLoader.gamePause)
					floodAnimation["airShrink"].speed = 0;
				else if (drowning == true)
					floodAnimation["airShrink"].speed = airAnimSpeed;
				else if (drowning == false)
					floodAnimation["airShrink"].speed = -1.5f;


				// detects when the player goes underwater
				if (player.transform.position.y < waterLine && drowning == false)
				{
					// ensures this if statement only runs once
					drowning = true;
					SoundPlayer.instance.PlaySingle1(splashIn);

					// (re)starts air shrinking animation
					floodAnimation["airShrink"].speed = airAnimSpeed;
					floodAnimation.Play("airShrink");

					// begins drowning coroutine
					playerDrowning = StartDrowning();
					StartCoroutine(playerDrowning);
				}

				// detects when the player resurfaces
				if (player.transform.position.y > waterLine && drowning == true)
				{
					// ensures this if statement only runs once
					drowning = false;

					if (waterLine > -8)
						SoundPlayer.instance.PlaySingle2(splashOut);

					// reverses shrinking animation
					floodAnimation["airShrink"].speed = -1.5f;

					// cancels drowning coroutine
					StopCoroutine(playerDrowning);
				}
			}
		}

		if (GameController.gameOver == true && playerDrowning != null)
		{
			StopCoroutine(playerDrowning);
		}
	}

	// called when the player goes underwater
	IEnumerator StartDrowning()
	{
		// counts down to drowning (while the game isn't paused or over)
		float timeCount = 0;

		while (timeCount < drownTime)
		{
			if (!SceneLoader.gamePause && !GameController.gameOver)
			{
				timeCount += Time.deltaTime;
			}

			yield return null;
		}
			
		// once drowned - changes player sprite
		SpriteRenderer playerSprite = player.GetComponent<SpriteRenderer>();
		playerSprite.sprite = drownedWorm;
		SoundPlayer.instance.PlaySingle1(gargle);

		// once drowned - changes body sprites
		for (int i = 0; i < ClickMover.body.Count; i++)
		{
			SpriteRenderer bodySprite = ClickMover.body[i].GetComponentInChildren<SpriteRenderer>();

			if (ClickMover.body[i].tag == "LiveBody")
				bodySprite.sprite = drownedBodyReg;
			else
				bodySprite.sprite = drownedBodyGreen;

			ClickMover.body[i].GetComponent<Animator>().SetBool("isDrowned", true);
		}
			
		// calls game over
		if (GameController.gameOver == false && SceneLoader.gamePause == false)
		{
			GameController.gameOver = true;
		}
	}
}