using System;
using UnityEngine;

public enum GameType
{
    Singleplayer,
    Multiplayer
}

public class GameManager : MonoBehaviour
{
    public Side ClientSide { get; set; }

    public GameType GameType { get; set; }

    public static GameManager Instance { get; set; }
    
    private void Awake()
    {
        if (Instance != null) Destroy(this);
        else Instance = this;
        DontDestroyOnLoad(this);
    }

    public void EndGame(Actor winner)
    {
        //Play fade animation on coroutine and show win or lose page.
    }
}