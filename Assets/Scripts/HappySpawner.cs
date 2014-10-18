using UnityEngine;
using System.Collections;
using HappyFunTimes;

public class HappySpawner : MonoBehaviour {

	public GameObject PrefabToSpawnForPlayer;
	private GameManager gameManager;
	private GameObject[] team1SpawnPos;
	private GameObject[] team2SpawnPos;
	private int currentTeam1SpawnPoint;
	private int currentTeam2SpawnPoint;

	GameServer server;
	public int playerCount;

	public NetPlayer _player;

	[CmdName ("changeBG")]
	private class MessageChangeBG : MessageCmdData {
		public int playerTeam;
		public MessageChangeBG(int _team) {
			playerTeam = _team;
			Debug.Log ("team  " + playerTeam);
		}		

	};



	// Use this for initialization
	void Start () {
		var options = new GameServer.Options();

		server = new GameServer(options, gameObject);
		server.OnPlayerConnect += StartNewPlayer;
		server.OnConnect += Connected;
		server.OnDisconnect += Disconnected;
		server.Init();
		gameManager = GameObject.Find ("GameManager").GetComponent<GameManager>();
		team1SpawnPos = GameObject.FindGameObjectsWithTag("SpawnPointTeam1");
		team2SpawnPos = GameObject.FindGameObjectsWithTag("SpawnPointTeam2");
		currentTeam1SpawnPoint = 0;
		currentTeam2SpawnPoint = 0;

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void StartNewPlayer(object sender, PlayerConnectMessageArgs e){

		Debug.Log("HappySpawner:StartNewPlayer");

		int t = GetPlayerTeam ();
		Vector3 newPos = getNewSpawnPos (t);

		// Spawn a new player then add a script to it.
		var go = (GameObject)Instantiate(PrefabToSpawnForPlayer, newPos, transform.rotation);
		go.GetComponent<PlayersMovement> ().initialPosition = newPos;

		if (t == 1) {
			go.GetComponentInChildren<Renderer> ().materials[1].color = gameManager.team1Color;
			go.tag = "playerTeam1";
		} else if ( t == 2) {
			go.GetComponentInChildren<Renderer> ().materials[1].color = gameManager.team2Color;
			go.tag = "playerTeam2";
		}

		// Get the Example3rdPersonController script to this object.
		var player = go.GetComponent<HappyController>();
		player.Init(e.netPlayer, "Player" + (++playerCount));
		_player = go.GetComponent<HappyController>()._player;

		//change phone bg
		_player.SendCmd (new MessageChangeBG (GetPlayerTeam()));	
	}

	int GetPlayerTeam (){
		if (playerCount % 2 == 0) {
			return 1;
		} else {
			return 2;
		}
	}

	Vector3 getNewSpawnPos(int team){
		Vector3 newPos = Vector3.zero;
		if (team == 1) {
			newPos = team1SpawnPos[currentTeam1SpawnPoint].transform.position;
			++currentTeam1SpawnPoint;
		} else if (team == 2){
			newPos = team2SpawnPos[currentTeam2SpawnPoint].transform.position;
			++currentTeam2SpawnPoint;
		}
		return newPos;
	}
	
	void Connected(object sender, System.EventArgs e) {
		Debug.Log("HappySpawner:Connected");
	}
	
	void Disconnected(object sender, System.EventArgs e) {
		Debug.Log("HappySpawner:Disconnected");
		Application.Quit();
	}
	
	void Cleanup() {
		if (server != null) {
			server.Close();
			server = null;
		}
	}
	
	void OnDestroy() {
		Cleanup();
	}
	
	void OnApplicationExit() {
		Cleanup();
	}
}
