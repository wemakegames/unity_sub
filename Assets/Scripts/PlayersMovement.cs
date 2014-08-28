using UnityEngine;
using System.Collections;

public class PlayersMovement : MonoBehaviour {
	public float flickStrenghtTouch;
	public float flickStrenghtMouse;
	public float maxSpeed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		
		LimitVelocity ();
		//ouchDebug.Log (rigidbody.velocity);
	}

	void LimitVelocity(){
		if (rigidbody.velocity.magnitude > maxSpeed) {
			rigidbody.velocity = rigidbody.velocity.normalized * maxSpeed;
		}
	}

	public void ApplyForce(Vector2 dir, string platform){
		if (platform == "touch") {
			rigidbody.AddForce (new Vector3 (dir.x, 0, -dir.y) * flickStrenghtTouch);
		} else if (platform == "mouse") {
			rigidbody.AddForce (new Vector3 (dir.x, 0, -dir.y) * flickStrenghtMouse);
		}
	}
}
