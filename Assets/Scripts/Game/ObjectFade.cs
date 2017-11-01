using UnityEngine;
using System.Collections;

public class ObjectFade : MonoBehaviour
{
	private GameController gameController;
	private float fadeDelay;
	private float fadeTimer;

	void Start() 
	{
		GameObject gameControllerObject = GameObject.FindWithTag("GameController");
		gameController = gameControllerObject.GetComponent<GameController>();

		fadeDelay = gameController.bloodFade;
	}
	
	void Update() 
	{
		fadeTimer += Time.deltaTime;

		if (fadeTimer >= fadeDelay)
		{
			Animator objectAnimator = GetComponent<Animator>();
			objectAnimator.SetBool("isFading", true);

			Destroy(gameObject, 3);
		}
	}
}
