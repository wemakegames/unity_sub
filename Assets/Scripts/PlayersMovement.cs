using UnityEngine;
using System.Collections;
using HappyFunTimes;

public class PlayersMovement : MonoBehaviour {
	public float maxStrength;
	public bool canPlay;
	public bool hasPlayed;
	public Vector3 initialPosition;

	private SoundManager soundManager;
	private CameraShake cameraShake;

	private NetPlayer _player;
	
	[CmdName ("myTurn")]
	private class MessageMyTurn : MessageCmdData {

		public MessageMyTurn(bool _turnBool) {
			myTurn = _turnBool;
		}		
		public bool myTurn;
	};

	void Start () {


		canPlay = false;
		hasPlayed = false;
		ChangeAlpha (0.25f);

		_player = GetComponent<HappyController>()._player;

		soundManager = GameObject.Find("GameManager").GetComponent<SoundManager> ();
		cameraShake = GameObject.Find ("Main Camera").GetComponent<CameraShake> ();

		soundManager.PlaySound ("PlayerConnect");
	}
	
	// Update is called once per frame
	public void ChangeTurn (bool _turn) {		

		if (_player != null) {
			if (canPlay && !hasPlayed & _turn) {
				_player.SendCmd (new MessageMyTurn (true));
			} else {
				_player.SendCmd (new MessageMyTurn (false));
			}
		}
	}

	public void ApplyForce(Vector3 kickDir, float strength){			
		if (canPlay) {
			canPlay = false;
			soundManager.PlaySound ("FingerPlay");
			rigidbody.AddForce(kickDir * ((strength/100) * maxStrength));
			cameraShake.shake = 0.03f;
			ChangeAlpha(0.25f);
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
