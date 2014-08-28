using UnityEngine;
using System.Collections;

public class BallEvents : MonoBehaviour {

	private GameManager gameManager;
	private SoundManager soundManager;
	private bool ballIsIn = false;


	// Use this for initialization
	void Start () {
		gameManager = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		soundManager = gameManager.GetComponent<SoundManager> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider collision) {
		if (collision.transform.tag == "GoalBlue") {
			gameManager.IncreaseGoalCount("IncreaseRed");
	
		} else if (collision.transform.tag == "GoalRed") {
			gameManager.IncreaseGoalCount("IncreaseBlue");
		}
		StartCoroutine (CenterBall ());
	}

	IEnumerator CenterBall() {
	 if (!ballIsIn) {
		ballIsIn = !ballIsIn;
		yield return new WaitForSeconds (2);
		soundManager.PlaySound ("goal");
		rigidbody.angularVelocity = Vector3.zero;
		rigidbody.velocity = Vector3.zero;
		transform.position = new Vector3 (0, 2.5f, 0);		
		ballIsIn = !ballIsIn;
	}

	}
}
