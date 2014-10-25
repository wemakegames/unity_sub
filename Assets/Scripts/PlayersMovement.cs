using UnityEngine;
using System.Collections;
using HappyFunTimes;

public class PlayersMovement : MonoBehaviour {
	public float maxStrength;
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

		if (_player != null) {
			if (canPlay && !hasPlayed) {
				_player.SendCmd (new MessageMyTurn ("MY TURN"));
			} else {
				_player.SendCmd (new MessageMyTurn ("NOT MY TURN"));
			}
		}
	}

	public void ApplyForce(Vector3 kickDir, float strength){
		if (canPlay) {
			rigidbody.AddForce(kickDir * ((strength/100) * maxStrength));

			ChangeAlpha(0.25f);
			canPlay = false;
			hasPlayed = true;
		}

	}

	void ChangeAlpha(float f){
		Material m;
		for (var i = 0; i < transform.FindChild("player").renderer.materials.Length; i++) {
			m = transform.FindChild("player").renderer.materials[i];
			Color c = m.color;
			c.a = f;
			m.color = c;
		}
	}
}
