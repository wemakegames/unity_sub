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
        goalCountRed = 0,
        goalCountBlue = 0;

    private GameObject
        turnAnnouncerContainer,
        waitingUIContainer;

    private GameObject[]
        playersRoster;

    public float
        gameTimeInMinutes;

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
        gameTime = gameTimeInMinutes * 60;
        timeCounter = GameObject.Find("CounterTurnText").GetComponent<Text>();
        TimeSpan timeSpan = TimeSpan.FromSeconds(gameTime);
        string timeText = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        timeCounter.text = timeText;
        //END TIMER DISPLAY

        InitializeGameObjects();

        soundManager = GetComponent<SoundManager>();

        turnAnnouncerContainer.SetActive(false);
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
                UpdateGameTimer();
                break;
        }
    }

    private void UpdateTeamScreen()
    {
        waitingUIContainer.SetActive(true);
        waitingTeam1.text = team1.Length.ToString();
        waitingTeam2.text = team2.Length.ToString();
    }

    private IEnumerator GameStart()
    {
        yield return new WaitForSeconds(2);
        waitingUIContainer.SetActive(false);
        StartCoroutine(AnnounceNextTurn());
    }

    private void UpdatePlayerRoster()
    {
        team1 = GameObject.FindGameObjectsWithTag("playerTeam1");
        team2 = GameObject.FindGameObjectsWithTag("playerTeam2");
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

            ActivatePlayers();
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

    private void ActivatePlayers()
    {
        foreach (GameObject obj in team1)
        {
            ChangeAlpha(1.0f, obj);
            obj.GetComponent<PlayersMovement>().canPlay = true;
        }

        foreach (GameObject obj in team2)
        {
            ChangeAlpha(1.0f, obj);
            obj.GetComponent<PlayersMovement>().canPlay = true;
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
        soundManager.PlaySound("WhistleLong");
        ResetAllPlayersPositions();
        ResetGoalCount();
        ResetTimer();
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
    }

    private void ResetGoalCount()
    {
        goalCountRed = 0;
        redCounter.text = "Red: " + goalCountRed;

        goalCountBlue = 0;
        blueCounter.text = "Blue: " + goalCountBlue;
    }

    private void ResetTimer()
    {
        gameTime = gameTimeInMinutes * 60;
    }

    private IEnumerator GoalDisplay()
    {
        Vector3
            initialPosition = goalDisplay.transform.position;
        Vector3
            newPosition = initialPosition;

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