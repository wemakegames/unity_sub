using HappyFunTimes;
using System.Collections;
using UnityEngine;

public class PlayersMovement : MonoBehaviour
{
    public float maxStrength;
    public bool canPlay;
    public bool hasPlayed;
    public Vector3 initialPosition;

    private SoundManager soundManager;
    private CameraShake cameraShake;

    private NetPlayer _player;

    [CmdName("myTurn")]
    private class MessageMyTurn : MessageCmdData
    {
        public MessageMyTurn(bool _turnBool)
        {
            myTurn = _turnBool;
        }

        public bool myTurn;
    };

    private void Start()
    {
        canPlay = false;

        _player = GetComponent<HappyController>()._player;

        soundManager = GameObject.Find("GameManager").GetComponent<SoundManager>();
        cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();

        soundManager.PlaySound("PlayerConnect");
    }

    public void ApplyForce(Vector3 kickDir, float strength)
    {
        if (canPlay)
        {
            soundManager.PlaySound("FingerPlay");
            GetComponent<Rigidbody>().AddForce(kickDir * ((strength / 100) * maxStrength));
            cameraShake.shake = 0.03f;
        }
    }
}