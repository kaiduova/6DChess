using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum GameType
{
    Singleplayer,
    Multiplayer
}

public enum GameDifficulty
{
    Easy,
    Medium,
    Hard,
    Boss
}

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private int winSceneIndex, loseSceneIndex, selectionSceneIndex, numberOfSelectableCardsToOffer;

    [SerializeField]
    private Image fadeScreen;

    [SerializeField]
    private int[] easyScenes, mediumScenes, hardScenes, bossScenes;

    [SerializeField]
    private int numEasyScenesToSelect, numMediumScenesToSelect, numHardScenesToSelect, numBossScenesToSelect;

    [SerializeField]
    private GameObject[] easySelectableCards, mediumSelectableCards, hardSelectableCards, bossSelectableCards;

    public List<GameObject> AddedCardPrefabs { get; } = new();

    private GameObject[] _selectableCardListToUse;

    public Side ClientSide { get; set; }

    public GameType GameType { get; set; }

    public GameDifficulty GameDifficulty { get; private set; }

    public static GameManager Instance { get; set; }

    public bool Connected { get; private set; }

    public bool HostingGame { get; set; }

    private bool _lastGameWon;
    private bool _queuedLoadFinishScene;

    private readonly List<int> _sceneOrder = new();

    private int _currentIndexOnSceneOrder;

    public int CurrentIndexOnSceneOrder => _currentIndexOnSceneOrder;

    private bool _inSelectionScene;

    public float Volume { get; set; }

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
        DontDestroyOnLoad(this);
        PhotonNetwork.AutomaticallySyncScene = true;
        if (!PlayerPrefs.HasKey("Volume"))
        {
            PlayerPrefs.SetFloat("Volume", 0.5f);
            Volume = 0.5f;
        }
        else
        {
            Volume = PlayerPrefs.GetFloat("Volume");
        }
    }

    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Update()
    {
        if (PlayerPrefs.HasKey("Volume"))
        {
            Volume = PlayerPrefs.GetFloat("Volume");
        }

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
        if (GameType == GameType.Multiplayer || _lastGameWon == false)
        {
            EndSession(winner);
            return;
        }

        if (_currentIndexOnSceneOrder >= _sceneOrder.Count - 1)
        {
            EndSession(winner);
            return;
        }
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
        _sceneOrder.Clear();
        RandomlyAddToSceneOrder(easyScenes.ToList(), numEasyScenesToSelect);
        RandomlyAddToSceneOrder(mediumScenes.ToList(), numMediumScenesToSelect);
        RandomlyAddToSceneOrder(hardScenes.ToList(), numHardScenesToSelect);
        RandomlyAddToSceneOrder(bossScenes.ToList(), numBossScenesToSelect);
        _currentIndexOnSceneOrder = -1;
    }

    private void RandomlyAddToSceneOrder(List<int> scenes, int maxScenes)
    {
        while (true)
        {
            if (scenes.Count == 0 || maxScenes == 0) break;
            var selectedScene = scenes[Random.Range(0, scenes.Count)];
            _sceneOrder.Add(selectedScene);
            scenes.Remove(selectedScene);
            maxScenes--;
        }
    }

    private List<T> RandomSelect<T>(List<T> items, int numToSelect)
    {
        List<T> returnList = new();
        while (true)
        {
            if (items.Count == 0 || numToSelect == 0) break;
            var selected = items[Random.Range(0, items.Count)];
            returnList.Add(selected);
            items.Remove(selected);
            numToSelect--;
        }

        return returnList;
    }

    public void LoadNextGameScene()
    {
        _currentIndexOnSceneOrder++;
        if (easyScenes.Contains(_sceneOrder[_currentIndexOnSceneOrder]))
        {
            GameDifficulty = GameDifficulty.Easy;
            _selectableCardListToUse = easySelectableCards;
        }
        else if (mediumScenes.Contains(_sceneOrder[_currentIndexOnSceneOrder]))
        {
            GameDifficulty = GameDifficulty.Medium;
            _selectableCardListToUse = mediumSelectableCards;
        }
        else if (hardScenes.Contains(_sceneOrder[_currentIndexOnSceneOrder]))
        {
            GameDifficulty = GameDifficulty.Hard;
            _selectableCardListToUse = hardSelectableCards;
        }
        else if (bossScenes.Contains(_sceneOrder[_currentIndexOnSceneOrder]))
        {
            GameDifficulty = GameDifficulty.Boss;
            _selectableCardListToUse = bossSelectableCards;
        }
        SceneManager.LoadScene(_sceneOrder[_currentIndexOnSceneOrder]);
    }

    public void ResetAddedCards()
    {
        AddedCardPrefabs.Clear();
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == selectionSceneIndex)
        {
            SelectionScene.Instance.Initialize(RandomSelect(_selectableCardListToUse.ToList(), numberOfSelectableCardsToOffer));
        }
    }
}