using UnityEngine;
using System.Collections;
using HappyFunTimes;
using UnityEngine.UI;

public class HappyController : MonoBehaviour {

	private PlayersMovement playerMovement;
	private NetPlayer m_netPlayer;
	private string m_name;

	[CmdName("pad")]
	class MessagePad : HappyFunTimes.MessageCmdData {
		public int pad;
		public int dir;
	};

	[CmdName("swipe")]
	class MessageSwipe : HappyFunTimes.MessageCmdData {
		public string platform;
		public int startX;
		public int startY;
		public int endX;
		public int endY;
		public float duration;

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
		m_name = "TEST";
	}

	void Remove(object sender, System.EventArgs e) {
		Destroy(gameObject);
	}

	void OnSwipe(MessageSwipe data) {

		Vector2 startPos = new Vector2(data.startX,data.startY);
		Vector2 endPos = new Vector2(data.endX,data.endY);

		playerMovement.ApplyForce (startPos, endPos, data.duration, data.platform);
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
		if (m_netPlayer !=null) {
			if (data.name.Length == 0) {
				m_netPlayer.SendCmd(new MessageSetName(m_name));
			} else {
				m_name = data.name;
				Text t = gameObject.GetComponentInChildren<Text>();
				t.text = m_name;

			}
		}
	}
	
	void OnBusy(MessageBusy data) {
		// handle busy message if we care.
	}

}
