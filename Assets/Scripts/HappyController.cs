using UnityEngine;
using System.Collections;
using HappyFunTimes;
using UnityEngine.UI;

public class HappyController : MonoBehaviour {

	private PlayersMovement playerMovement;
	private NetPlayer m_netPlayer;
	private string m_name;

	private Vector3 kickStart;
	private Vector3 kickDir;
	private float kickStrength;

	private HappySpawner happySpawner;

	private PlayerLineRenderer playerLineRenderer;

	[CmdName("pad")]
	class MessagePad : HappyFunTimes.MessageCmdData {
		public int pad;
		public int dir;
	};

	[CmdName("kick")]
	class MessageKick : HappyFunTimes.MessageCmdData {

	};
	
	[CmdName("setColor")]
	class MessageSetColor : HappyFunTimes.MessageCmdData {
		public string color;
	};
	
	[CmdName("setName")]
	private class MessageSetName : MessageCmdData {
		public MessageSetName() {  // needed for deserialization
		}
		public MessageSetName(string _name) {
			name = _name;
		}
		public string name = "";
	};
	
	[CmdName("busy")]
	class MessageBusy : HappyFunTimes.MessageCmdData {
		public bool busy;
	};

	[CmdName("drawLine")]
	class MessageDrawLine : HappyFunTimes.MessageCmdData {
		public string platform;
		public int playerX = 0;
		public int playerY = 0;
		public int lineEndX = 0;
		public int lineEndY = 0;
		public float strength = 0;
	};

	[CmdName ("setPlayerColor")]
	private class setPlayerColor : MessageCmdData {

		public setPlayerColor(int _team) {
			playerTeam = _team;
		}		
		public int playerTeam;
	};


	public NetPlayer _player;
	DPadEmuJS _padEmu;
	
	void Start() {

		playerMovement = GetComponent<PlayersMovement> ();

		_player.OnDisconnect += Remove;
		_player.RegisterCmdHandler<MessagePad>(OnPad);
		_player.RegisterCmdHandler<MessageKick>(OnKick);
		_player.RegisterCmdHandler<MessageSetColor>(OnSetColor);
		_player.RegisterCmdHandler<MessageSetName>(OnSetName);
		_player.RegisterCmdHandler<MessageBusy>(OnBusy);
		_player.RegisterCmdHandler<MessageDrawLine>(OnDrawLine);

		happySpawner = GameObject.Find("GameManager").GetComponent<HappySpawner>();

		playerLineRenderer = gameObject.GetComponent<PlayerLineRenderer> ();

		//change phone bg
		int t = 0;

		if (happySpawner.GetPlayerTeam() == 1) {
			t = 1;
		} else if (happySpawner.GetPlayerTeam() == 2) {
			t = 2;
		}
		_player.SendCmd (new setPlayerColor (t));	
	}

	void Update () {
			
	}

	public void Init(NetPlayer player, string name)
	{
		_player = player;
		_padEmu = new DPadEmuJS();
		m_name = "TEST";
	}

	void Remove(object sender, System.EventArgs e) {
		Destroy(gameObject);
	}

	void OnPad(MessagePad data) {
		_padEmu.Update(data.pad, data.dir);
	}

	public float GetAxis(int index, string name)
	{
		return _padEmu.GetAxisRaw(index, name);
	}
	
	void OnSetColor(MessageSetColor data) {
		/*
		var color : Color = CSSParse.Style.ParseCSSColor(data.color);
		var pix : Color[] =  new Color[1];
		pix[0] = color;
		var tex : Texture2D = new Texture2D(1, 1);
		tex.SetPixels(pix);
		tex.Apply();
		_guiStyle.normal.background = tex;
		*/
	}

	
	private void OnSetName(MessageSetName data) {

		if (data.name.Length == 0) {
			_player.SendCmd(new MessageSetName(m_name));
		} else {
			m_name = data.name;
			Text t = gameObject.GetComponentInChildren<Text>();
			t.text = m_name;
		}
	}
	
	void OnBusy(MessageBusy data) {
		// handle busy message if we care.
	}

	void OnDrawLine (MessageDrawLine data){

		Vector3 rp;

		if (playerMovement.canPlay) {
			Vector3 oldStart = new Vector3 (data.playerX, 0, -data.playerY);
			Vector3 oldEnd = new Vector3 (data.lineEndX, 0, -data.lineEndY);

			kickStrength = data.strength;
			kickDir = oldEnd - oldStart;
			kickStart = gameObject.transform.position;

			Ray r = new Ray (kickStart, kickDir.normalized);
			rp = r.GetPoint (data.strength / 6);
		} else {
			rp = gameObject.transform.position;
		}
			playerLineRenderer.DrawPlayerLine (rp);
			//Debug.DrawRay (kickStart, kickDir, Color.black, 0.2f, false);
		
	}

	void OnKick(MessageKick data) {
		playerMovement.ApplyForce (kickDir.normalized, kickStrength);
	}


}

