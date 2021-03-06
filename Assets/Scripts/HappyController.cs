﻿using HappyFunTimes;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HappyController : MonoBehaviour
{
    private PlayersMovement playerMovement;
    private NetPlayer m_netPlayer;
    private string m_name;

    private Vector3 kickStart;
    private Vector3 kickDir;
    private float kickStrength;

    private HappySpawner happySpawner;

    private PlayerLineRenderer playerLineRenderer;

    [CmdName("pad")]
    private class MessagePad : HappyFunTimes.MessageCmdData
    {
        public int pad;
        public int dir;
    };

    [CmdName("kick")]
    private class MessageKick : HappyFunTimes.MessageCmdData
    {
    };

    [CmdName("setColor")]
    private class MessageSetColor : HappyFunTimes.MessageCmdData
    {
        public string color;
    };

    [CmdName("setName")]
    private class MessageSetName : MessageCmdData
    {
        public MessageSetName()
        {  // needed for deserialization
        }

        public MessageSetName(string _name)
        {
            name = _name;
        }

        public string name = "";
    };

    [CmdName("busy")]
    private class MessageBusy : HappyFunTimes.MessageCmdData
    {
        public bool busy;
    };

    [CmdName("drawLine")]
    private class MessageDrawLine : HappyFunTimes.MessageCmdData
    {
        public string platform;
        public int playerX = 0;
        public int playerY = 0;
        public int lineEndX = 0;
        public int lineEndY = 0;
        public float strength = 0;
    };

    [CmdName("setPlayerColor")]
    private class setPlayerColor : MessageCmdData
    {
        public setPlayerColor(int _team)
        {
            playerTeam = _team;
        }

        public int playerTeam;
    };

    public NetPlayer _player;
    private DPadEmuJS _padEmu;

    private void Start()
    {
        playerMovement = GetComponent<PlayersMovement>();

        _player.OnDisconnect += Remove;
        _player.RegisterCmdHandler<MessagePad>(OnPad);
        _player.RegisterCmdHandler<MessageKick>(OnKick);
        _player.RegisterCmdHandler<MessageSetColor>(OnSetColor);
        _player.RegisterCmdHandler<MessageSetName>(OnSetName);
        _player.RegisterCmdHandler<MessageBusy>(OnBusy);
        _player.RegisterCmdHandler<MessageDrawLine>(OnDrawLine);

        happySpawner = GameObject.Find("GameManager").GetComponent<HappySpawner>();

        playerLineRenderer = gameObject.GetComponent<PlayerLineRenderer>();

        //change phone bg
        int t = 0;

        if (happySpawner.GetPlayerTeam() == 1)
        {
            t = 1;
        }
        else if (happySpawner.GetPlayerTeam() == 2)
        {
            t = 2;
        }
        _player.SendCmd(new setPlayerColor(t));
    }

    private void Update()
    {
    }

    public void Init(NetPlayer player, string name)
    {
        _player = player;
        _padEmu = new DPadEmuJS();
        m_name = "TEST";
    }

    private void Remove(object sender, System.EventArgs e)
    {
        Destroy(gameObject);
    }

    private void OnPad(MessagePad data)
    {
        _padEmu.Update(data.pad, data.dir);
    }

    public float GetAxis(int index, string name)
    {
        return _padEmu.GetAxisRaw(index, name);
    }

    private void OnSetColor(MessageSetColor data)
    {
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

    private void OnSetName(MessageSetName data)
    {
        if (data.name.Length == 0)
        {
            _player.SendCmd(new MessageSetName(m_name));
        }
        else
        {
            m_name = data.name;
            Text t = gameObject.GetComponentInChildren<Text>();
            t.text = m_name;
        }
    }

    private void OnBusy(MessageBusy data)
    {
        // handle busy message if we care.
    }

    private void OnDrawLine(MessageDrawLine data)
    {
        Vector3 rp1, rp2;

        if (playerMovement.canPlay)
        {
            Vector3 oldStart = new Vector3(data.playerX, 0, -data.playerY);
            Vector3 oldEnd = new Vector3(data.lineEndX, 0, -data.lineEndY);

            kickStrength = data.strength;
            kickDir = oldEnd - oldStart;
            kickStart = gameObject.transform.position;

            Ray r = new Ray(kickStart, kickDir.normalized);
            rp1 = r.GetPoint(data.strength / 8);
            rp2 = r.GetPoint(data.strength / 6);
        }
        else
        {
            rp1 = gameObject.transform.position;
            rp2 = gameObject.transform.position;
        }
        playerLineRenderer.DrawPlayerLine(rp1, rp2);
        //Debug.DrawRay (kickStart, kickDir, Color.black, 0.2f, false);
    }

    private void OnKick(MessageKick data)
    {
        playerMovement.ApplyForce(kickDir.normalized, kickStrength);
    }
}