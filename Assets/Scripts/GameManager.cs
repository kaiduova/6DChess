using System;
using System.Collections;
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

    private void Awake()
    {
        if (Instance != null) Destroy(this);
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
    }

    public override void OnConnectedToMaster()
    {
        print("Connected to master.");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void EndGame(Actor winner)
    {
        StartCoroutine(FadeAndChangeScene(winner.Side == ClientSide ? winSceneIndex : loseSceneIndex));
    }
    
    private IEnumerator FadeAndChangeScene(int index)
    {
        var color = Color.black;
        for (float alpha = 0f; alpha <= 1; alpha += 0.05f)
        {
            color.a = alpha;
            fadeScreen.color = color;
            yield return new WaitForSeconds(.1f);
        }
        SceneManager.LoadScene(index);
        fadeScreen.color = new Color(0, 0, 0, 0);
    }

    public void StartMultiplayerLobby()
    {
        
    }

    public void JoinMultiplayerLobby()
    {
        
    }
}