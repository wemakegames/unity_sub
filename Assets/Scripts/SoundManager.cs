using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	public AudioClip sndGoal;

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
		
			default:
			Debug.Log("!!! Sound not recognized !!!");
			break;
		}

		AudioSource.PlayClipAtPoint (clip, transform.position);

	}
}
