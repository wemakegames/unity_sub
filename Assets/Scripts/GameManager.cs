using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Color
        team1Color = Color.blue,
        team2Color = Color.red;

    public string
        team1Name = "Cerulean Broncos",
        team2Name = "Lightning Pepperoni";

    private GameObject
        goalArrowLeft,
        goalArrowRight,
        goalLeft,
        goalRight;

    private GameObject goalDisplay;

    private int
        activePlayerIndexTeam1,
        activePlayerIndexTeam2,
        goalCountRed = 0,
        goalCountBlue = 0;

    private GameObject

        activePlayer,
        turnAnnouncerContainer,
        waitingUIContainer;

    private GameObject[]
        playersRoster;

    private float
        gameTime;

    private bool
        timerActive = false;

    private Text
        timeCounter,
        waitingTeam1,
        waitingTeam2,
        turnAnnouncer,
        redCounter,
        blueCounter;

    private SoundManager
        soundManager;

    private enum state
    {
        TEAM_SELECTION,
        START,
        ANNOUNCER,
        PLAY,
    }

    private state
        gameState;

    public int playersRequired;

    [HideInInspector]
    public GameObject[] team1;

    [HideInInspector]
    public GameObject[] team2;

    // Use this for initialization
    private void Start()
    {
        gameState = state.TEAM_SELECTION;

        //SET TIMER DISPLAY
        gameTime = 1200.0f;
        timeCounter = GameObject.Find("CounterTurnText").GetComponent<Text>();
        TimeSpan timeSpan = TimeSpan.FromSeconds(gameTime);
        string timeText = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        timeCounter.text = timeText;
        //END TIMER DISPLAY

        InitializeGameObjects();

        soundManager = GetComponent<SoundManager>();

        turnAnnouncerContainer.SetActive(false);

        activePlayerIndexTeam1 = 0;
        activePlayerIndexTeam2 = 0;
    }

    private void InitializeGameObjects()
    {
        goalDisplay = GameObject.Find("GoalDisplay");
        turnAnnouncer = GameObject.Find("TurnAnnouncerText").GetComponent<Text>();
        redCounter = GameObject.Find("CounterRedText").GetComponent<Text>();
        blueCounter = GameObject.Find("CounterBlueText").GetComponent<Text>();
        turnAnnouncerContainer = GameObject.Find("TurnAnnouncer");
        waitingUIContainer = GameObject.Find("WaitingUI");
        waitingTeam1 = GameObject.Find("waitingTeam1").GetComponent<Text>();
        waitingTeam2 = GameObject.Find("waitingTeam2").GetComponent<Text>();
        goalArrowLeft = GameObject.Find("GoalArrowLeft");
        goalArrowRight = GameObject.Find("GoalArrowRight");
        goalLeft = GameObject.Find("Goal1");
        goalRight = GameObject.Find("Goal2");
    }

    // Update is called once per frame
    private void Update()
    {
        UpdatePlayerRoster();

        switch (gameState)
        {
            case state.TEAM_SELECTION:
                UpdateTeamScreen();
                if ((team1.Length + team2.Length) >= playersRequired)
                {
                    gameState = state.START;
                    StartCoroutine(GameStart());
                }
                break;

            case state.START:
                //currently methods are triggered from code
                break;

            case state.ANNOUNCER:
                //currently methods are triggered from code
                break;

            case state.PLAY:
                break;
        }

        UpdateGameTimer();
    }

    private void UpdateTeamScreen()
    {
        waitingUIContainer.SetActive(true);
        waitingTeam1.text = team1.Length.ToString();
        waitingTeam2.text = team2.Length.ToString();
    }

    private IEnumerator GameStart()
    {
        yield return null;
        waitingUIContainer.SetActive(false);
        StartCoroutine(AnnounceNextTurn());
    }

    private void UpdatePlayerRoster()
    {
        team1 = GameObject.FindGameObjectsWithTag("playerTeam1");
        team2 = GameObject.FindGameObjectsWithTag("playerTeam2");

        playersRoster = new GameObject[team1.Length + team2.Length];
        team1.CopyTo(playersRoster, 0);
        team2.CopyTo(playersRoster, team1.Length);
    }

    private IEnumerator AnnounceNextTurn()
    {
        if (turnAnnouncerContainer.activeSelf == false)
        {
            yield return new WaitForSeconds(1);

            Color c = Color.white;
            String t = "";

            if (gameState == state.START)
            {
                c = Color.white;
                t = "Kick Off!";
                turnAnnouncerContainer.SetActive(true);
                turnAnnouncer.color = c;
                turnAnnouncer.text = t;
                soundManager.PlaySound("WhistleLong");
                yield return new WaitForSeconds(2);
                turnAnnouncerContainer.SetActive(false);
                timerActive = true;
            }
            else
            {
                soundManager.PlaySound("WhistleShort");
            }

            gameState = state.PLAY;
        }
    }

    private int GetNewPlayerIndex(GameObject[] team, int playerIndex)
    {
        if (playerIndex == team.Length - 1)
        {
            return 0;
        }
        else if (team.Length == 0)
        {
            Debug.Log("error, no players in team");
            return 0;
        }
        else
        {
            return playerIndex + 1;
        }
    }

    private void ActivateNewPlayer()
    {
        foreach (GameObject obj in playersRoster)
        {
            if (obj == activePlayer)
            {
                ChangeAlpha(1.0f, obj);
                obj.GetComponent<PlayersMovement>().canPlay = true;
                obj.GetComponent<PlayersMovement>().hasPlayed = false;
                obj.GetComponent<PlayersMovement>().ChangeTurn(true);
                ParticleSystem part = obj.GetComponentInChildren<ParticleSystem>();

                part.Play();
            }
            else
            {
                obj.GetComponent<PlayersMovement>().ChangeTurn(false);
                ChangeAlpha(0.25f, obj);
                ParticleSystem part = obj.GetComponentInChildren<ParticleSystem>();
                part.Stop();
            }
        }
    }

    private void ChangeAlpha(float f, GameObject obj)
    {
        Material m;
        for (var i = 0; i < obj.transform.FindChild("player").GetComponent<Renderer>().materials.Length; i++)
        {
            m = obj.transform.FindChild("player").GetComponent<Renderer>().materials[i];
            Color c = m.color;
            c.a = f;
            m.color = c;
        }
    }

    private void CheckEndOfTurn(GameObject[] team)
    {   //check if players is still there and if it has played
        if (team.Length > 0)
        {
            bool canPlay = activePlayer.GetComponent<PlayersMovement>().canPlay;
            bool hasPlayed = activePlayer.GetComponent<PlayersMovement>().hasPlayed;

            if (!canPlay && hasPlayed)
            {
                ParticleSystem part = activePlayer.GetComponentInChildren<ParticleSystem>();
                part.Stop();

                gameState = state.ANNOUNCER;
                activePlayer.GetComponent<PlayersMovement>().hasPlayed = false;
                StartCoroutine(AnnounceNextTurn());
            }
        }
        else
        {
            Debug.Log("No player in current team :(");
        }
    }

    private void UpdateGameTimer()
    {
        if (timerActive && gameTime > 0)
        {
            gameTime -= Time.deltaTime;

            TimeSpan timeSpan = TimeSpan.FromSeconds(gameTime);
            string timeText = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);

            timeCounter.text = timeText;
        }
        else if (gameTime <= 0)
        {
            gameTime = 0;
            timeCounter.text = gameTime.ToString();
            FinishGame();
        }
    }

    private void FinishGame()
    {
        GetComponent<HappySpawner>().Cleanup();

        gameState = state.TEAM_SELECTION;
    }

    public void ResetAllPlayersPositions()
    {
        foreach (GameObject obj in team1)
        {
            obj.transform.position = obj.GetComponent<PlayersMovement>().initialPosition;
        }

        foreach (GameObject obj in team2)
        {
            obj.transform.position = obj.GetComponent<PlayersMovement>().initialPosition;
        }
    }

    public void IncreaseGoalCount(string team)
    {
        StartCoroutine(GoalDisplay());

        if (team == "IncreaseRed")
        {
            goalCountRed++;
            redCounter.text = "Red: " + goalCountRed;
        }
        else if (team == "IncreaseBlue")
        {
            goalCountBlue++;
            blueCounter.text = "Blue: " + goalCountBlue;
        }
        else
        {
            //debug
        }
    }

    private void ActivateGoalFeedback(int team)
    {
        Color
            c;
        switch (team)
        {
            case 1:
                c = team1Color;
                c.a = 0.5f;
                goalLeft.GetComponent<Renderer>().material.color = c;
                goalArrowLeft.GetComponent<Renderer>().material.color = team1Color;
                goalArrowLeft.transform.GetComponent<Renderer>().enabled = true;
                goalArrowRight.GetComponent<Renderer>().enabled = false;
                //inactive goal
                c = Color.grey;
                c.a = 0.5f;
                goalRight.GetComponent<Renderer>().material.color = c;

                break;

            case 2:
                c = team2Color;
                c.a = 0.5f;
                goalRight.GetComponent<Renderer>().material.color = c;
                goalArrowRight.GetComponent<Renderer>().material.color = team2Color;
                goalArrowLeft.GetComponent<Renderer>().enabled = false;
                goalArrowRight.GetComponent<Renderer>().enabled = true;
                //inactive goal
                c = Color.grey;
                c.a = 0.5f;
                goalLeft.GetComponent<Renderer>().material.color = c;
                break;
        }
    }

    private IEnumerator GoalDisplay()
    {
        Vector3 initialPosition = goalDisplay.transform.position;
        Vector3 newPosition = initialPosition;

        while (newPosition.x < Screen.width / 2)
        {
            newPosition.x += 100;
            goalDisplay.transform.position = newPosition;
            yield return 0;
        }
        yield return new WaitForSeconds(1);
        goalDisplay.transform.position = initialPosition;
    }
}