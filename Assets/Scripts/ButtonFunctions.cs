using System;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WebSocketSharp;

public class ButtonFunctions : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private TMP_InputField inputFieldMultiplayer, inputFieldScene;

    [SerializeField]
    private TMP_Text connectionStatus;
    
    [SerializeField]
    private TMP_Text roomStatus;

    private bool _queuedCreateRoomRequest;
    
    private bool _queuedJoinRoomRequest;
    
    public static void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    private void Update()
    {
        connectionStatus.text = GameManager.Instance.Connected ? "Connected." : "Not Connected.";
        
        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient && !GameManager.Instance.HostingGame)
        {
            GameManager.Instance.HostingGame = true;
            PhotonNetwork.LoadLevel(int.Parse(inputFieldScene.text));
        }
    }

    public void StartSingleplayerGame()
    {
        GameManager.Instance.GenerateSceneOrder();
        GameManager.Instance.ResetAddedCards();
        if (inputFieldScene.text.IsNullOrEmpty())
        {
            GameManager.Instance.LoadNextGameScene();
        }
        else
        {
            LoadScene(int.Parse(inputFieldScene.text));
        }
    }
    
    public void HostMultiplayerGame()
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            print("Client not yet network ready.");
            return;
        }

        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            _queuedCreateRoomRequest = true;
        }
        else
        {
            DefaultCreateRoom();
        }
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        if (_queuedCreateRoomRequest)
        {
            DefaultCreateRoom();
            _queuedCreateRoomRequest = false;
        }
        if (_queuedJoinRoomRequest)
        {
            PhotonNetwork.JoinRoom(inputFieldMultiplayer.text);
            _queuedJoinRoomRequest = false;
        }
    }

    private void DefaultCreateRoom()
    {
        var roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(inputFieldMultiplayer.text, roomOptions);
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        print("Room created.");
        roomStatus.text = "Room created.";
    }

    public void JoinMultiplayerGame()
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            print("Client not yet network ready.");
            return;
        }
        
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            _queuedJoinRoomRequest = true;
        }
        else
        {
            PhotonNetwork.JoinRoom(inputFieldMultiplayer.text);
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        print("Room joined.");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        print("Room couldn't be joined.");
        roomStatus.text = "Room couldn't be joined.";
    }
}