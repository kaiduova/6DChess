using System;
using System.Collections;
using Controllers;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameType
{
    Singleplayer,
    Multiplayer
}

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private int winSceneIndex, loseSceneIndex;

    [SerializeField]
    private Image fadeScreen;
    
    public Side ClientSide { get; set; }

    public GameType GameType { get; set; }

    public static GameManager Instance { get; set; }

    public bool Connected { get; private set; }

    public bool HostingGame { get; set; }

    private bool _lastGameWon;
    private bool _queuedLoadFinishScene;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
        DontDestroyOnLoad(this);
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    private void Update()
    {
        Connected = PhotonNetwork.IsConnected;
        
        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
        {
            GameType = GameType.Multiplayer;
            if (PhotonNetwork.IsMasterClient)
            {
                ClientSide = Side.Normal;
            }
            else
            {
                ClientSide = Side.Inverted;
            }
        }
        else if (!PhotonNetwork.InRoom)
        {
            GameType = GameType.Singleplayer;
            ClientSide = Side.Normal;
        }
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        print("Connected to master.");
        if (_queuedLoadFinishScene)
        {
            SceneManager.LoadScene(_lastGameWon ? winSceneIndex : loseSceneIndex);
            _queuedLoadFinishScene = false;
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void EndGame(Actor winner)
    {
        _lastGameWon = winner.TryGetComponent<PlayerController>(out var playerController) && playerController.isActiveAndEnabled;
        if (PhotonNetwork.InRoom)
        {
            _queuedLoadFinishScene = true;
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            SceneManager.LoadScene(_lastGameWon ? winSceneIndex : loseSceneIndex);
        }
        HostingGame = false;
    }
}