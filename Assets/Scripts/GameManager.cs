using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public string gameState;
	public Color team1Color = Color.blue;
	public Color team2Color = Color.red;
	public string team1Name = "Cerulean Broncos";
	public string team2Name = "Lightning Pepperoni";

	private GameObject goalArrowLeft;
	private GameObject goalArrowRight;
	private GameObject goalLeft;
	private GameObject goalRight;

	private int activePlayerIndexTeam1;
	private int activePlayerIndexTeam2;
	private GameObject activePlayer;

	private int goalCountRed = 0;
	private int goalCountBlue = 0;

	private Text timeCounter;
	private float gameTime;
	private bool timerActive = false;

	private GameObject turnAnnouncerContainer;
	private GameObject waitingUIContainer;
	private Text waitingTeam1;
	private Text waitingTeam2;
	private Text turnAnnouncer;

	private Text redCounter;
	private Text blueCounter;

	private SoundManager soundManager;

	public int playersRequired;

	[HideInInspector]
	public GameObject[] team1; 
	public GameObject[] team2;
	public int nextTeam;
	private GameObject[] playersRoster;

	// Use this for initialization
	void Start () {

		gameState = "teamSelection";
		gameTime = 10.0f;
		timeCounter = GameObject.Find ("CounterTurnText").GetComponent<Text>();
		timeCounter.text = gameTime.ToString();

		turnAnnouncer = GameObject.Find ("TurnAnnouncerText").GetComponent<Text>();
		redCounter = GameObject.Find ("CounterRedText").GetComponent<Text>();
		blueCounter = GameObject.Find ("CounterBlueText").GetComponent<Text>();
		soundManager = GetComponent<SoundManager> ();

		turnAnnouncerContainer = GameObject.Find ("TurnAnnouncer");
		turnAnnouncerContainer.SetActive(false);

		waitingUIContainer = GameObject.Find ("WaitingUI");

		waitingTeam1 = GameObject.Find ("waitingTeam1").GetComponent<Text>();
		waitingTeam2 = GameObject.Find ("waitingTeam2").GetComponent<Text>();


		activePlayerIndexTeam1 = 0;
		activePlayerIndexTeam2 = 0;

		nextTeam = 1;

		goalArrowLeft = GameObject.Find("GoalArrowLeft");
		goalArrowRight = GameObject.Find("GoalArrowRight");

		goalLeft = GameObject.Find("Goal1");
		goalRight = GameObject.Find("Goal2");
	}
	
	// Update is called once per frame
	void Update () {

		UpdatePlayerRoster();

		switch (gameState) {
			case "teamSelection":
			UpdateTeamScreen();
			if ((team1.Length + team2.Length) >= playersRequired) {			
				gameState = "start";
				StartCoroutine(GameStart());
			}
			break;

			case "start":
			//currently methods are triggered from code
			break;

			case "announcer":
			//currently methods are triggered from code
			break;

			case "team1":
			CheckEndOfTurn (team1); //checks if active player has played
			ActivateGoalFeedback(nextTeam);
			break;

			case "team2":
			CheckEndOfTurn (team2); //checks if players in active team have playerd or not
			ActivateGoalFeedback(nextTeam);
			break;
		}

		UpdateGameTimer();
	}

	void UpdateTeamScreen(){
		waitingTeam1.text = team1.Length.ToString();
		waitingTeam2.text = team2.Length.ToString();
	}

	IEnumerator GameStart(){
		yield return new WaitForSeconds(2);
		waitingUIContainer.SetActive(false);
		StartCoroutine(AnnounceNextTurn());
		timerActive = true;
	}

	void UpdatePlayerRoster(){
		team1 = GameObject.FindGameObjectsWithTag("playerTeam1");
		team2 = GameObject.FindGameObjectsWithTag("playerTeam2");

		playersRoster = new GameObject[team1.Length + team2.Length];
		team1.CopyTo(playersRoster, 0);
		team2.CopyTo(playersRoster, team1.Length);
	}

	IEnumerator AnnounceNextTurn(){
		
		if (turnAnnouncerContainer.activeSelf == false){
			yield return new WaitForSeconds(1);
			
			Color c = Color.white;
			String t = "";
			
			if (gameState == "start") {
				c = Color.white;
				t = "Kick Off!";
				turnAnnouncerContainer.SetActive(true);
				turnAnnouncer.color = c;
				turnAnnouncer.text = t;
				soundManager.PlaySound ("WhistleLong");
				yield return new WaitForSeconds(2);				
				turnAnnouncerContainer.SetActive(false);
			} else {
				soundManager.PlaySound ("WhistleShort");
			}
			SetNewActivePlayer(nextTeam);			
			if (nextTeam == 1) {
				gameState = "team1";
				nextTeam = 2;
			} else if (nextTeam == 2){
				gameState = "team2";
				nextTeam = 1;
			}
		}	
	}

	void SetNewActivePlayer(int nt){

		if (activePlayer == null){
			activePlayer = team1[0];
		}

		switch(nt){
		case 1:
			activePlayer = team1[activePlayerIndexTeam1];
			activePlayerIndexTeam1 = GetNewPlayerIndex(team1, activePlayerIndexTeam1);
			gameState = "team1";
			break;

		case 2:
			activePlayer = team2[activePlayerIndexTeam2];
			activePlayerIndexTeam2 = GetNewPlayerIndex(team2, activePlayerIndexTeam2) ;
			gameState = "team2";
			break;
		}

		ActivateNewPlayer();
	}

	int GetNewPlayerIndex(GameObject[] team, int playerIndex){
		
		if (playerIndex == team.Length - 1){
			return 0;
			
		} else if (team.Length == 0){
			Debug.Log ("error, no players in team");
			return 0;
		} else {
			return playerIndex + 1;
		}
	}

	void ActivateNewPlayer (){
		foreach(GameObject obj in playersRoster) {
			if (obj == activePlayer) {
				ChangeAlpha(1.0f, obj);
				activePlayer.GetComponent<PlayersMovement>().canPlay = true;
				activePlayer.GetComponent<PlayersMovement>().hasPlayed = false;
				ParticleSystem part = obj.GetComponentInChildren<ParticleSystem>();
				if (nextTeam == 1){
					part.renderer.material.color = team1Color;
				} else {
					part.renderer.material.color = team2Color;
				}

				part.Play();

			} else {
				ChangeAlpha(0.25f, obj);
				ParticleSystem part = obj.GetComponentInChildren<ParticleSystem>();
				part.Stop();				
			}
		}
	}

	void ChangeAlpha(float f, GameObject obj){
		Material m;
		for (var i = 0; i < obj.transform.FindChild("player").renderer.materials.Length; i++) {
			m = obj.transform.FindChild("player").renderer.materials [i];
			Color c = m.color;
			c.a = f;
			m.color = c;
		}
	}
	

	void CheckEndOfTurn(GameObject[] team) {   //check if players is still there and if it has played
		if (team.Length > 0) {
			bool canPlay = activePlayer.GetComponent<PlayersMovement>().canPlay;
			bool hasPlayed = activePlayer.GetComponent<PlayersMovement>().hasPlayed;

			if (!canPlay && hasPlayed) {

				ParticleSystem part = activePlayer.GetComponentInChildren<ParticleSystem>();
				part.Stop();

				gameState = "announcer";
				activePlayer.GetComponent<PlayersMovement>().hasPlayed = false;
				StartCoroutine(AnnounceNextTurn());
			}
		} else {
			Debug.Log ("No player in current team :(");
		}
	}

	void UpdateGameTimer(){
		if (timerActive && gameTime > 0) {
			gameTime -= Time.deltaTime;
			timeCounter.text = gameTime.ToString();
		} else if (gameTime <= 0) {
			gameTime = 0;
			timeCounter.text = gameTime.ToString();
			FinishGame ();
		}
	}

	void FinishGame(){
		Debug.Log ("GAME FINISHED!");
	}

	public void ResetAllPlayersPositions(){
		foreach(GameObject obj in team1) {
			obj.transform.position = obj.GetComponent<PlayersMovement>().initialPosition;
		}

		foreach(GameObject obj in team2) {
			obj.transform.position = obj.GetComponent<PlayersMovement>().initialPosition;
		}
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

	void ActivateGoalFeedback(int team){
		Color c;
		switch (team){
		case 1:
			c = team1Color;
			c.a = 0.5f;
			goalLeft.renderer.material.color = c;
			goalArrowLeft.renderer.material.color = team1Color;
			goalArrowLeft.transform.renderer.enabled = true;
			goalArrowRight.renderer.enabled = false;
			//inactive goal
			c = Color.grey;
			c.a = 0.5f;
			goalRight.renderer.material.color = c;

			break;

		case 2:
			c = team2Color;
			c.a = 0.5f;
			goalRight.renderer.material.color = c;
			goalArrowRight.renderer.material.color = team2Color;
			goalArrowLeft.renderer.enabled = false;
			goalArrowRight.renderer.enabled = true;
			//inactive goal
			c = Color.grey;
			c.a = 0.5f;
			goalLeft.renderer.material.color = c;
			break;
		}
	}
}
