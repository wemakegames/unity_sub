using UnityEngine;
using System.Collections;
using HappyFunTimes;

public class PlayersMovement : MonoBehaviour {
	private float flickStrenghtTouch;
	private float flickStrenghtMouse;
	public float maxSpeed;
	public bool canPlay;
	public bool hasPlayed;
	public Vector3 initialPosition;

	private NetPlayer _player;
	
	[CmdName ("myTurn")]
	private class MessageMyTurn : MessageCmdData {

		public MessageMyTurn(string _turnText) {
			turnText = _turnText;
		}		
		public string turnText;
	};

	void Start () {


		canPlay = false;
		hasPlayed = false;
		ChangeAlpha (0.25f);

		_player = GetComponent<HappyController>()._player;

		flickStrenghtMouse = 2.0f;
		flickStrenghtTouch = 1.0f;
	}
	
	// Update is called once per frame
	void FixedUpdate () {		
		LimitVelocity ();

		if (_player != null) {
			if (canPlay && !hasPlayed) {
				_player.SendCmd (new MessageMyTurn ("MY TURN"));
			} else {
				_player.SendCmd (new MessageMyTurn ("NOT MY TURN"));
			}
		}
	}

	void LimitVelocity(){
		if (rigidbody.velocity.magnitude > maxSpeed) {
			rigidbody.velocity = rigidbody.velocity.normalized * maxSpeed;
		}
	}

	public void ApplyForce(Vector3 kickDir, float strength, string platform){
		if (canPlay) {

			strength = CalcStrength(strength, platform);

			rigidbody.AddRelativeForce (kickDir * strength);

			ChangeAlpha(0.25f);
			canPlay = false;
			hasPlayed = true;
		}

	}

	void ChangeAlpha(float f){
		Material m = GetComponent<Renderer> ().material;
		Color c = m.color;
		c.a = f;
		m.color = c;
	}

	Vector2 CalcDirection(Vector2 s, Vector2 e){
		Vector2 dir = e-s;
		return dir; 
	}


	float CalcSwipeDistance(Vector2 s, Vector2 e){
		float xd = e.x - s.x;
		float yd = e.y - s.y;
		float dist = Mathf.Sqrt(Mathf.Pow(xd,2) + Mathf.Pow(yd,2));
		return dist;
	}

	float CalcStrength (float s, string platform){

		switch (platform) {
		case "mouse":
			s *= flickStrenghtMouse;
			break;

		case "touch":
			s *= flickStrenghtTouch;
			break;
		}
		return s;
	}

}
