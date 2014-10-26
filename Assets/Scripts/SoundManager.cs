// CROWD SOUND IS PLAYED BY A SEPARATE SOUND SOURCE IN GAME MANAGER

using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	public AudioClip sndWhistleLong;
	public AudioClip sndWhistleShort;
	public AudioClip sndPlayerConnect;
	public AudioClip sndBallHit;
	public AudioClip sndCrowd;
	public AudioClip[] sndFingerPlay;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void PlaySound(string evt) {
		AudioClip clip = null;

		switch (evt)
		{
			case ("WhistleShort"):
			clip = sndWhistleShort;
			break;

			case ("WhistleLong"):
			clip = sndWhistleLong;
			break;

			case ("PlayerConnect"):
			clip = sndPlayerConnect;
			break;

			case ("BallHit"):
			clip = sndBallHit;
			break;

			case ("FingerPlay"):
			clip = sndFingerPlay[Random.Range(0,3)];
			break;

			default:
			Debug.Log("!!! Sound not recognized !!!");
			break;
		}
		AudioSource.PlayClipAtPoint (clip, transform.position);

	}
}
