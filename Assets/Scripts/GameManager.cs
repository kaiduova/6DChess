using System;
using System.Collections;
using System.Collections.Generic;
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
    private int winSceneIndex, loseSceneIndex, selectionSceneIndex;

    [SerializeField]
    private Image fadeScreen;

    [SerializeField]
    private int[] easyScenes, mediumScenes, hardScenes, bossScenes;
    
    public Side ClientSide { get; set; }

    public GameType GameType { get; set; }

    public static GameManager Instance { get; set; }

    public bool Connected { get; private set; }

    public bool HostingGame { get; set; }

    private bool _lastGameWon;
    private bool _queuedLoadFinishScene;

    private List<int> _sceneOrder;

    private int _currentIndexOnSceneOrder;

    private bool _inSelectionScene;

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

        if (_inSelectionScene)
        {
            
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
        if (GameType == GameType.Multiplayer || _lastGameWon == false)
        {
            EndSession(winner);
            return;
        }
        //Go to selection scene, then to next in scene order.
        _inSelectionScene = true;
        SceneManager.LoadScene(selectionSceneIndex);
    }

    public void EndSession(Actor winner)
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

    public void GenerateSceneOrder()
    {
        
    }
}