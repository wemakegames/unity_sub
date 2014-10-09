using UnityEngine;
using System.Collections;
using HappyFunTimes;

public class PlayersMovement : MonoBehaviour {
	public float flickStrenghtTouch;
	public float flickStrenghtMouse;
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

	public void ApplyForce(Vector2 startPos, Vector2 endPos, float duration, string platform){
		if (canPlay) {
			Vector2 dir = CalcDirection (startPos, endPos);
			float dist = CalcSwipeDistance (startPos, endPos);
			float strength = CalcStrength (dist, duration, platform);


			rigidbody.AddForce (new Vector3 (dir.x, 0, -dir.y) * strength);

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

	float CalcStrength (float d, float t, string platform){
		float strength;

		strength = d / t;
		if (platform == "touch") {
			strength *= flickStrenghtTouch;
		} else if (platform == "mouse") {
			strength *= flickStrenghtMouse;
		}
		return strength;
	}

}
