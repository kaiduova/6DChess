using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Side
{
    Normal,
    Inverted
}

public class Actor : MonoBehaviourPunCallbacks
{
    public IReadOnlyList<Piece> Pieces => _pieces.AsReadOnly();
    public IReadOnlyList<Card> Hand => _hand.AsReadOnly();
    
    public bool CanAct => _canAct;
    public bool IsActing => _isActing;
    
    public Side Side => side;

    public int HandCapacity => handCapacity;

    public Actor Opponent => opponent;
    
    #region Private Fields
    
    private readonly List<Piece> _pieces = new();
    private readonly List<Card> _hand = new();
    
    [SerializeField]
    private GameObject[] deck;
    
    [SerializeField]
    private GameObject cameraGameObject;

    [SerializeField]
    private int handCapacity;

    [SerializeField]
    private Actor opponent;

    [SerializeField]
    private Side side;

    private int _currentlyActingPieceIndex;
    private Piece[] _orderedPieces;

    private bool _canAct;
    private bool _isActing;

    [SerializeField]
    private int health;

    public int Health
    {
        get => health;
        set => health = value;
    }

    private bool _queuedEndTurn;
    
    #endregion

    private void Start()
    {
        if (GameManager.Instance.ClientSide == Side)
        {
            cameraGameObject.SetActive(true);
            if (TryGetComponent<AiController>(out var aiController)) aiController.enabled = false;
            if (TryGetComponent<PlayerController>(out var playerController)) playerController.enabled = true;
        }
        else
        {
            cameraGameObject.SetActive(false);
            if (TryGetComponent<AiController>(out var aiController) && GameManager.Instance.GameType == GameType.Singleplayer) aiController.enabled = true;
            if (TryGetComponent<PlayerController>(out var playerController) && GameManager.Instance.GameType == GameType.Multiplayer) playerController.enabled = false;
        }
        if (Side == Side.Normal) StartGame();
    }

    public void Update()
    {
        if (Health <= 0f)
        {
            GameManager.Instance.EndGame(opponent);
        }
    }

    private void StartGame()
    {
        _canAct = true;
        _isActing = false;
    }
    
    public void Draw()
    {
        if (!_canAct || _isActing) return;
        _isActing = true;
        photonView.RPC(nameof(DrawCommon), RpcTarget.All, Random.Range(0, deck.Length));
    }

    public void SpawnPiece(Tile tile, Card card)
    {
        //Spawns piece on the tile.
        if (!_canAct || _isActing) return;
        _isActing = true;
        photonView.RPC(nameof(SpawnPieceCommon), RpcTarget.All, tile.location, _hand.IndexOf(card));
    }

    public void PerformPieceActions()
    {
        if (!_canAct || _isActing) return;
        _isActing = true;
        photonView.RPC(nameof(PerformPieceActionsCommon), RpcTarget.All);
    }
    
    public void EndTurn()
    {
        if (_isActing) return;
        photonView.RPC(nameof(EndTurnCommon), RpcTarget.All);
    }

    public void DestroyPiece(Piece piece)
    {
        _pieces.Remove(piece);
        Destroy(piece.gameObject);
        //Destroy animation.
    }

    [PunRPC]
    private void DrawCommon(int deckIndex)
    {
        if (_hand.Count > handCapacity) return;
        _isActing = true;
        var card = Instantiate(deck[deckIndex], transform.GetChild(_hand.Count));
        _hand.Add(card.GetComponent<Card>());
        //Animation here.
        //Move stop acting to after animation.
        _isActing = false;
    }

    [PunRPC]
    private void SpawnPieceCommon(Vector2 location, int cardIndex)
    {
        var tile = Board.Instance.Tiles.First(tile => tile.location == location);
        var piece = Instantiate(_hand[cardIndex].PiecePrefab, tile.transform, false);
        var pieceComp = piece.GetComponent<Piece>();
        tile.SetOrReplacePieceOnTile(pieceComp);
        _pieces.Add(pieceComp);
        pieceComp.Actor = this;
        //Spawn animation here.
        Destroy(_hand[cardIndex]);
        //Card destroy animation here.
        _hand.RemoveAt(cardIndex);
        //Move stop acting to after animation.
        _isActing = false;
    }
    
    [PunRPC]
    private void PerformPieceActionsCommon()
    {
        _orderedPieces = _pieces.OrderBy(piece => piece.transform.position.x)
            .ThenBy(piece => piece.transform.position.z).ToArray();
        _currentlyActingPieceIndex = 0;
        NextPieceAct();
    }
    
    private void NextPieceAct()
    {
        if (_currentlyActingPieceIndex >= _orderedPieces.Length)
        {
            _currentlyActingPieceIndex = 0;
            _isActing = false;
            return;
        }
        _currentlyActingPieceIndex++;
        _orderedPieces[_currentlyActingPieceIndex - 1].Act(NextPieceAct);
    }

    [PunRPC]
    private void EndTurnCommon()
    {
        _canAct = false;
        _isActing = false;
        opponent._canAct = true;
        opponent._isActing = false;
    }

    //Put RPCs down here that get called based on the application instance.
}
