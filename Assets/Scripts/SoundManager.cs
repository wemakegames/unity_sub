using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	public AudioClip sndGoal;
	public AudioClip sndBallHit;
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
			case ("goal"):
			clip = sndGoal;
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
