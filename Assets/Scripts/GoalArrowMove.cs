using UnityEngine;
using System.Collections;

public class GoalArrowMove : MonoBehaviour {

	private Vector3 initialPosition;
	private Vector3 maxHeight;
	private Vector3 minHeight;

	private bool goingUp;

	private int variation = 1;
	private float speed = 0.075f;

	// Use this for initialization
	void Start () {
		initialPosition = gameObject.transform.position;
		maxHeight = initialPosition + new Vector3(0, variation, 0);
		minHeight = initialPosition + new Vector3(0, -variation, 0);
		goingUp = true;
	}
	
	// Update is called once per frame
	void Update () {

		switch (goingUp){
		case true:
			gameObject.transform.position += new Vector3(0,speed,0);
			if (gameObject.transform.position.y >= maxHeight.y){
				goingUp = false;
			}
			break;

		case false:
			gameObject.transform.position -= new Vector3(0,speed,0);
			if (gameObject.transform.position.y <= minHeight.y){
				goingUp = true;
			}
			break;
		}
	}
}
