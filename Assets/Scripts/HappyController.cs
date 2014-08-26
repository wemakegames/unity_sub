using UnityEngine;
using System.Collections;
using HappyFunTimes;

public class HappyController : MonoBehaviour {

	public PlayersMovement playerMovement;

	[CmdName("pad")]
	class MessagePad : HappyFunTimes.MessageCmdData {
		public int pad;
		public int dir;
	};

	[CmdName("swipe")]
	class MessageSwipe : HappyFunTimes.MessageCmdData {
		public string swipe;
		public int startX;
		public int startY;
		public int endX;
		public int endY;

	};
	
	[CmdName("setColor")]
	class MessageSetColor : HappyFunTimes.MessageCmdData {
		public string color;
	};
	
	[CmdName("setName")]
	class MessageSetName : HappyFunTimes.MessageCmdData {
		public string name;
	};
	
	[CmdName("busy")]
	class MessageBusy : HappyFunTimes.MessageCmdData {
		public bool busy;
	};

	NetPlayer _player;
	DPadEmuJS _padEmu;
	
	void Start () {

		playerMovement = GetComponent<PlayersMovement> ();

		_player.OnDisconnect += Remove;
		_player.RegisterCmdHandler<MessagePad>(OnPad);
		_player.RegisterCmdHandler<MessageSwipe>(OnSwipe);
		_player.RegisterCmdHandler<MessageSetColor>(OnSetColor);
		_player.RegisterCmdHandler<MessageSetName>(OnSetName);
		_player.RegisterCmdHandler<MessageBusy>(OnBusy);
	}

	void Update () {
			
	}

	public void Init(NetPlayer player, string name)
	{
		_player = player;
		_padEmu = new DPadEmuJS();
		SetName(name);
	}

	void Remove(object sender, System.EventArgs e) {
		Destroy(gameObject);
	}

	void OnSwipe(MessageSwipe data) {

		Vector2 startPos = new Vector2(data.startX,data.startY);
		Vector2 endPos = new Vector2(data.endX,data.endY);


		Debug.Log ("start   " + data.startX + " , " + data.startY + "     " + "end   " + data.endX + " , " + data.endY);
		Vector2 dir = endPos - startPos; 
		playerMovement.ApplyForce (dir);
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
	
	void SetName(string name) {
		/*
		_playerName = name;
		gameObject.name = "Player-" + _playerName;
		var size : Vector2 = _guiStyle.CalcSize(GUIContent(_playerName));
		_nameRect.width = size.x + 10;
		_nameRect.height = size.y + 5;
		*/
	}
	
	void OnGUI() {
		/*
		var size : Vector2 = _guiStyle.CalcSize(GUIContent(_playerName));
		var coords : Vector3 = Camera.main.WorldToScreenPoint(transform.position + _nameOffset);
		_nameRect.x = coords.x - size.x * 0.5 - 5;
		_nameRect.y = Screen.height - coords.y;
		_guiStyle.normal.textColor = Color.black;
		_guiStyle.contentOffset.x = 4;
		_guiStyle.contentOffset.y = 2;
		GUI.Box(_nameRect, _playerName, _guiStyle);
		*/
	}
	
	void OnSetName(MessageSetName data) {
		/*
		if (data.name.length > 0) {
			SetName(data.name);
		} else {
			var msg = new MessageSetName();
			msg.name = _playerName;
			_netPlayer.SendCmd(msg);
		}
		*/
	}
	
	void OnBusy(MessageBusy data) {
		// handle busy message if we care.
	}

}
