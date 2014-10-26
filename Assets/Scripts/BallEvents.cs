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
			StartCoroutine (CenterBall ());
	
		} else if (collision.transform.tag == "GoalRed") {
			gameManager.IncreaseGoalCount("IncreaseBlue");
			StartCoroutine (CenterBall ());
		}
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.transform.tag == "playerTeam1" || collision.transform.tag == "playerTeam2"){
			Vector3 vF = collision.rigidbody.velocity;
			float i;
			if (vF.x > vF.z){
				i = vF.x;
			}else{
				i = vF.z;
			}
			soundManager.PlaySound ("BallHit");
			rigidbody.AddForce (new Vector3 (0, i, 0)* 50);
			
		}
	}

	IEnumerator CenterBall() {
		if (!ballIsIn) {
			ballIsIn = !ballIsIn;
			yield return new WaitForSeconds (2);
			soundManager.PlaySound ("WhistleLong");
			rigidbody.angularVelocity = Vector3.zero;
			rigidbody.velocity = Vector3.zero;
			transform.position = new Vector3 (0, 2.5f, 0);		
			ballIsIn = !ballIsIn;

			gameManager.ResetAllPlayersPositions();

		}

	}
}
