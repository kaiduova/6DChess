using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using Photon.Pun;
using TMPro;
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

    [SerializeField]
    private GameObject cardSpawnLocationMarker;

    [SerializeField]
    private GameObject[] handLocationMarkers;

    [SerializeField]
    private TMP_Text healthText;
        
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
        if (handLocationMarkers.Length < handCapacity)
            throw new Exception("There must be at least an equal amount of hand location markers as the capacity.");
        if (GameManager.Instance.ClientSide == Side)
        {
            cameraGameObject.SetActive(true);
            if (TryGetComponent<AiController>(out var aiController))
            {
                aiController.enabled = false;
            }

            if (TryGetComponent<PlayerController>(out var playerController))
            {
                playerController.enabled = true;
            }
        }
        else
        {
            cameraGameObject.SetActive(false);
            if (TryGetComponent<AiController>(out var aiController))
            {
                aiController.enabled = GameManager.Instance.GameType == GameType.Singleplayer;
            }
            if (TryGetComponent<PlayerController>(out var playerController)) playerController.enabled = false;
        }
        
        if (Side == Side.Normal) StartGame();
    }

    public void Update()
    {
        if (Health <= 0f)
        {
            GameManager.Instance.EndGame(opponent);
            Health = 1;
        }

        healthText.text = Health.ToString();
    }

    private void StartGame()
    {
        _canAct = true;
        _isActing = false;
    }
    
    public void Draw()
    {
        if (!_canAct || _isActing) return;
        if (_hand.Count >= handCapacity) return;
        _isActing = true;
        if (GameManager.Instance.GameType == GameType.Multiplayer)
        {
            photonView.RPC(nameof(DrawCommon), RpcTarget.All, Random.Range(0, deck.Length));
        }
        else
        {
            DrawCommon(Random.Range(0, deck.Length));
        }
    }

    public void SpawnPiece(Tile tile, Card card)
    {
        //Spawns piece on the tile.
        if (!_canAct || _isActing) return;
        _isActing = true;
        if (GameManager.Instance.GameType == GameType.Multiplayer)
        {
            photonView.RPC(nameof(SpawnPieceCommon), RpcTarget.All, tile.location, _hand.IndexOf(card));
        }
        else
        {
            SpawnPieceCommon(tile.location, _hand.IndexOf(card));
        }
    }

    public void PerformPieceActions()
    {
        if (!_canAct || _isActing) return;
        _isActing = true;
        if (GameManager.Instance.GameType == GameType.Multiplayer)
        {
            photonView.RPC(nameof(PerformPieceActionsCommon), RpcTarget.All);
        }
        else
        {
            PerformPieceActionsCommon();
        }
    }
    
    public void EndTurn()
    {
        if (_isActing) return;
        if (GameManager.Instance.GameType == GameType.Multiplayer)
        {
            photonView.RPC(nameof(EndTurnCommon), RpcTarget.All);
        }
        else
        {
            EndTurnCommon();
        }
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
        if (_hand.Count >= handCapacity) return;
        _isActing = true;
        var card = Instantiate(deck[deckIndex], cardSpawnLocationMarker.transform.position, Quaternion.identity);
        var cardComp = card.GetComponent<Card>();
        _hand.Add(cardComp);
        //Animation here.
        StartCoroutine(DrawAnimation(card, _hand.IndexOf(cardComp)));
    }

    private IEnumerator DrawAnimation(GameObject card, int index)
    {
        for (var angle = 0f; angle <= 180f; angle += 18f)
        {
            card.transform.rotation = Quaternion.Euler(0, 0, angle);
            yield return new WaitForSeconds(0.05f);
        }

        const float duration = 0.5f;
        for (var t = 0f; t <= duration; t += Time.deltaTime)
        {
            card.transform.position = Vector3.Lerp(cardSpawnLocationMarker.transform.position, handLocationMarkers[index].transform.position, t/duration);
            yield return null;
        }

        card.transform.position = handLocationMarkers[index].transform.position;
        _isActing = false;
    }

    private IEnumerator RelocateCard(GameObject card, Vector3 origin, Vector3 newLocation)
    {
        const float duration = 0.2f;
        for (var t = 0f; t <= duration; t += Time.deltaTime)
        {
            card.transform.position = Vector3.Lerp(origin, newLocation, t/duration);
            yield return null;
        }
        card.transform.position = newLocation;
    }

    [PunRPC]
    private void SpawnPieceCommon(Vector2 location, int cardIndex)
    {
        _isActing = true;
        var tile = Board.Instance.Tiles.First(tile => tile.location == location);
        var piece = Instantiate(_hand[cardIndex].PiecePrefab, tile.transform.position, Quaternion.identity);
        piece.transform.parent = tile.transform;
        var pieceComp = piece.GetComponent<Piece>();
        tile.SetOrReplacePieceOnTile(pieceComp);
        _pieces.Add(pieceComp);
        pieceComp.Actor = this;
        //Spawn animation here.
        Destroy(_hand[cardIndex].gameObject);
        //Card destroy animation here.
        _hand.RemoveAt(cardIndex);
        for (var i = cardIndex; i < _hand.Count; i++)
        {
            StartCoroutine(RelocateCard(_hand[i].gameObject, _hand[i].gameObject.transform.position,
                handLocationMarkers[i].transform.position));
        }
        //Move stop acting to after animation.
        _isActing = false;
    }
    
    [PunRPC]
    private void PerformPieceActionsCommon()
    {
        _isActing = true;
        if (side == Side.Normal)
        {
            _orderedPieces = _pieces.OrderByDescending(piece => piece.transform.position.z)
                .ThenBy(piece => piece.transform.position.x).ToArray();
        }
        else
        {
            _orderedPieces = _pieces.OrderBy(piece => piece.transform.position.z)
                .ThenByDescending(piece => piece.transform.position.x).ToArray();
        }
        
        _currentlyActingPieceIndex = 0;
        NextPieceAct();
        foreach (var piece in _pieces)
        {
            piece.InSludge = false;
            if (piece.Tile.CurrentPiece != piece)
                throw new Exception("The piece " + piece + " needs to end on a non-jump move action.");
        }
    }

    private void NextPieceAct()
    {
        while (true)
        {
            if (_currentlyActingPieceIndex >= _orderedPieces.Length)
            {
                _currentlyActingPieceIndex = 0;
                _isActing = false;
                return;
            }

            _currentlyActingPieceIndex++;
            
            //Skip piece if destroyed.
            if (_orderedPieces[_currentlyActingPieceIndex - 1] != null)
                _orderedPieces[_currentlyActingPieceIndex - 1].Act(NextPieceAct);
            else
            {
                continue;
            }

            break;
        }
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
