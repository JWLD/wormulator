using UnityEngine;
using System.Collections;

public class MoleMover : MonoBehaviour 
{
	[Header("Scripts")]
	public GameController gameController;

	[Header("Raycasting")]
	public LayerMask collisionLayer;
	private RaycastHit2D moleRay;
	public LayerMask waypointLayer;
	private RaycastHit2D wayPoint;
	public GameObject wayObject;

	private bool oneTime = false;

	void Start()
	{
		GameObject gameControllerObject = GameObject.FindWithTag("GameController");
		gameController = gameControllerObject.GetComponent<GameController>();
	
		// checks that the moles are meant to be active
		if (SceneLoader.gameMode == "hunt")
		{
			gameObject.SetActive(true);
		}
		else if (gameObject.tag == "ScubaMole" && SceneLoader.gameMode == "flood")
		{
			gameObject.transform.position = new Vector3(0f, 1f, 0f);
		}
		else if (gameController.molesActive == false || SceneLoader.gameMode == "solo" || SceneLoader.gameMode == "flood")
		{
			gameObject.SetActive(false);
		}
	}

	void Update()
	{
		// moves the mole constantly
		if (GameController.gameOver == false && SceneLoader.gamePause == false)
		{
			transform.Translate(Vector2.up * gameController.moleSpeed * Time.deltaTime);
		}

		// casts a line in front of the mole to test for obstructions
		Vector3 scanAhead = transform.position + transform.TransformDirection(new Vector3(0f, 0.8f, 0f));
		Debug.DrawLine(transform.position, scanAhead, Color.red, 0.1f);
		moleRay = Physics2D.Linecast(transform.position, scanAhead, collisionLayer);

		// triggers ChangeDirection when the mole hits a wall
		if (moleRay.collider != null)
		{
			if (moleRay.collider.tag == "Wall" || moleRay.collider.tag == "Gate") 
			{
				ChangeDirection();
			}
		}

		// casts a line within the mole to check for waypoints
		Vector3 wayScanStart = transform.position + transform.TransformDirection(new Vector3(0f, -0.2f, 0f));
		Vector3 wayScanEnd = transform.position + transform.TransformDirection(new Vector3(0f, -0.05f, 0f));
		Debug.DrawLine(wayScanStart, wayScanEnd, Color.blue, 0.1f);
		wayPoint = Physics2D.Linecast(wayScanStart, wayScanEnd, waypointLayer);

		// triggers ChangeDirection when the mole hits a waypoint
		if (wayPoint)
		{
			// if a previous waypoint has been deactivated, reactivate it
			if (wayObject != null)
			{
				wayObject.SetActive(true);
			}

			// temporarily turns off the waypoint
			wayObject = wayPoint.collider.gameObject;
			wayObject.SetActive(false);

			ChangeDirection();
		}

		// stops animation at gameover
		if (oneTime == false && GameController.gameOver == true)
		{
			GetComponent<Animator>().enabled = false;
			oneTime = true;
		}
	}

	// function that changes the mole's direction when it hits a wall
	public void ChangeDirection()
	{
		// snaps the mole to the closest coordinate
		float xPos = Mathf.Round (transform.position.x);
		float yPos = Mathf.Round (transform.position.y);
		transform.position = new Vector2 (xPos, yPos);

		// casts a line forward, left and right to test for open paths
		Vector3 lookLeft = transform.position + transform.TransformDirection(Vector3.left);
		Debug.DrawLine (transform.position, lookLeft, Color.green, 1f);
		bool leftOpen = Physics2D.Linecast(transform.position, lookLeft, collisionLayer);

		Vector3 lookRight = transform.position + transform.TransformDirection(Vector3.right);
		Debug.DrawLine (transform.position, lookRight, Color.green, 1f);
		bool rightOpen = Physics2D.Linecast(transform.position, lookRight, collisionLayer);

		Vector3 lookForward = transform.position + transform.TransformDirection(Vector3.up);
		Debug.DrawLine (transform.position, lookForward, Color.green, 1f);
		bool forwardOpen = Physics2D.Linecast(transform.position, lookForward, collisionLayer);
		
		RandomPath(leftOpen, rightOpen, forwardOpen);
	}

	// chooses a random path where multiple are available
	void RandomPath(bool leftOpen, bool rightOpen, bool forwardOpen)
	{
		// stores the possible new z-axis rotations
		float[] targetRot = new float[3] {transform.eulerAngles.z, transform.eulerAngles.z + 90, transform.eulerAngles.z - 90};

		// runs dependings on which paths are open
		if (leftOpen == false && rightOpen == false && forwardOpen == false) 
		{
			int random = Random.Range(0, 3);
			transform.eulerAngles = new Vector3(0, 0, targetRot[random]);
		}
		else if (leftOpen == false && rightOpen == false) 
		{
			int random = Random.Range(1, 3);
			transform.eulerAngles = new Vector3(0, 0, targetRot[random]);
		}
		else if (leftOpen == false && forwardOpen == false) 
		{
			int random = Random.Range(0, 2);
			transform.eulerAngles = new Vector3(0, 0, targetRot[random]);
		}
		else if (rightOpen == false && forwardOpen == false) 
		{
			targetRot = new float[2] {transform.eulerAngles.z, transform.eulerAngles.z - 90};
			int random = Random.Range(0, 2);
			transform.eulerAngles = new Vector3(0, 0, targetRot[random]);
		}
		else if (leftOpen == false) 
		{
			float targetRotZ = transform.eulerAngles.z + 90;
			transform.eulerAngles = new Vector3(0, 0, targetRotZ);
		} 
		else if (rightOpen == false) 
		{
			float targetRotZ = transform.eulerAngles.z - 90;
			transform.eulerAngles = new Vector3(0, 0, targetRotZ);
		}
	}
}