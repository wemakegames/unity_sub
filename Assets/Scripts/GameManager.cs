using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	private int goalCountRed = 0;
	private int goalCountBlue = 0;
	private Text turnCounter;
	private GameObject turnAnnouncerContainer;
	private Text turnAnnouncer;
	private bool announcingTurn;
	private Text redCounter;
	private Text blueCounter;
	private string currentTurn;   //false is team one, true is team 2
	private bool goToNextTurn;
	private SoundManager soundManager;

	[HideInInspector]
	public GameObject[] team1; 
	public GameObject[] team2;
	public GameObject[] activeTeam;

	// Use this for initialization
	void Start () {
		announcingTurn = false;
		goToNextTurn = false;
		currentTurn = "start";
		turnCounter = GameObject.Find ("CounterTurnText").GetComponent<Text>();
		turnAnnouncer = GameObject.Find ("TurnAnnouncerText").GetComponent<Text>();
		redCounter = GameObject.Find ("CounterRedText").GetComponent<Text>();
		blueCounter = GameObject.Find ("CounterBlueText").GetComponent<Text>();
		soundManager = GetComponent<SoundManager> ();
		//soundManager.PlaySound ("goal");

		turnAnnouncerContainer = GameObject.Find ("TurnAnnouncer");
		turnAnnouncerContainer.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		UpdateTeams ();  //checks if new players have been added
		CheckActiveTeam(); //cheks if players in active team have playerd or not

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

	void UpdateTeams(){
		team1 = GameObject.FindGameObjectsWithTag("playerTeam1");
		team2 = GameObject.FindGameObjectsWithTag("playerTeam2");

		if (currentTurn == "team1" || currentTurn =="start"){
			activeTeam = team1;
			UpdateTurnCounter(1);
		} else if (currentTurn == "team2"){
			activeTeam = team2;
			UpdateTurnCounter(2);
		}

		if (goToNextTurn) {
			foreach(GameObject obj in activeTeam) {
				obj.GetComponent<PlayersMovement>().canPlay = true;
			}
			goToNextTurn = false;
		}
	}

	void CheckActiveTeam() {
		if (activeTeam.Length > 0) { 

			if (!announcingTurn) {
				int j = 0;
				foreach(GameObject obj in activeTeam) {
					j += Convert.ToInt32(obj.GetComponent<PlayersMovement>().canPlay);
				}

				if (j == 0) {   // if no player in active team can still play
					StartCoroutine(AnnounceNextTurn());

				}
			}
		}
	}

	IEnumerator AnnounceNextTurn(){
		announcingTurn = !announcingTurn;
		yield return new WaitForSeconds(2);
		turnAnnouncerContainer.SetActive(true);
		soundManager.PlaySound ("goal");
		string t = "";
		Color c = Color.black;
		switch (currentTurn) {

		case "start":
			c = Color.white;
			t = "MATCH START!";
			break;

		case "team1":
			c = Color.red;
			t = "red team!";
			break;

		case "team2":
			c = Color.blue;
			t = "blue team!";
			break;
		}

		turnAnnouncer.color = c;
		turnAnnouncer.text = t;

		yield return new WaitForSeconds(2);

		turnAnnouncerContainer.SetActive(false);
		goToNextTurn = true;
		if (currentTurn == "team2" || currentTurn == "start") {
			currentTurn = "team1";
		} else {
			currentTurn = "team2";
		}
		

		announcingTurn = !announcingTurn;

	}


	void UpdateTurnCounter(int team){
		switch (team) {
			case 1:
			turnCounter.text = "Team 1";
			turnCounter.color = Color.blue;
			break;

			case 2:
			turnCounter.text = "Team 2";
			turnCounter.color = Color.red;
			break;
		}
	}

}
