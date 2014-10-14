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
	private float kickStrenght;


	[CmdName("pad")]
	class MessagePad : HappyFunTimes.MessageCmdData {
		public int pad;
		public int dir;
	};

	[CmdName("kick")]
	class MessageKick : HappyFunTimes.MessageCmdData {
		public string platform;
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
		public int playerX;
		public int playerY;
		public int lineEndX;
		public int lineEndY;
	};


	public NetPlayer _player;
	DPadEmuJS _padEmu;
	
	void Start () {

		playerMovement = GetComponent<PlayersMovement> ();

		_player.OnDisconnect += Remove;
		_player.RegisterCmdHandler<MessagePad>(OnPad);
		_player.RegisterCmdHandler<MessageKick>(OnKick);
		_player.RegisterCmdHandler<MessageSetColor>(OnSetColor);
		_player.RegisterCmdHandler<MessageSetName>(OnSetName);
		_player.RegisterCmdHandler<MessageBusy>(OnBusy);
		_player.RegisterCmdHandler<MessageDrawLine>(OnDrawLine);

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

		Vector3 oldStart = new Vector3(data.playerX, 0, -data.playerY);
		Vector3 oldEnd = new Vector3(data.lineEndX,0, -data.lineEndY);

		kickStrenght = Mathf.Abs(Vector3.Distance(oldEnd,oldStart));
		kickDir = oldEnd - oldStart;
		kickStart = gameObject.transform.position;
		Debug.DrawRay (kickStart, kickDir, Color.black, 0.2f, false);
	}

	void OnKick(MessageKick data) {
		Debug.Log (data.platform);
		playerMovement.ApplyForce (kickDir, kickStrenght, data.platform);
	}


}

