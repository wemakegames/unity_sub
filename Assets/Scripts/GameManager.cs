using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	private int goalCountRed = 0;
	private int goalCountBlue = 0;
	private Text redCounter;
	private Text blueCounter;
	private SoundManager soundManager;

	// Use this for initialization
	void Start () {
		redCounter = GameObject.Find ("CounterRedText").GetComponent<Text>();
		blueCounter = GameObject.Find ("CounterBlueText").GetComponent<Text>();
		soundManager = GetComponent<SoundManager> ();
		soundManager.PlaySound ("goal");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void IncreaseGoalCount(string team){
		if (team == "IncreaseRed") {
			goalCountRed ++;
			redCounter.text = "Red: " + goalCountRed;
		} else if (team == "IncreaseBlue") {
			goalCountBlue ++;
			blueCounter.text = "Blue: " + goalCountBlue;
		}
	}
}
