using UnityEngine;
using System.Collections;

public class PlayersMovement : MonoBehaviour {
	public float flickStrenght;
	public float maxSpeed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Input.GetKeyDown("up")) {		
			rigidbody.AddForce(Vector3.forward * flickStrenght);
		}
		if (Input.GetKeyDown("down")) {		
			rigidbody.AddForce(Vector3.back * flickStrenght);
		}
		if (Input.GetKeyDown("left")) {		
			rigidbody.AddForce(Vector3.left * flickStrenght);
		}
		if (Input.GetKeyDown("right")) {		
			rigidbody.AddForce(Vector3.right * flickStrenght);
		}
		
		LimitVelocity ();
		Debug.Log (rigidbody.velocity);
	}

	void LimitVelocity(){
		if (rigidbody.velocity.magnitude > maxSpeed) {
			rigidbody.velocity = rigidbody.velocity.normalized * maxSpeed;
		}
	}

	void GetControl(){

	}
}
