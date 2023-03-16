using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Actor : MonoBehaviourPunCallbacks
{
    private readonly List<Piece> _pieces = new();
    private readonly List<Card> _hand = new();

    public IReadOnlyList<Piece> Pieces => _pieces.AsReadOnly();
    public IReadOnlyList<Card> Hand => _hand.AsReadOnly();
    
    [SerializeField]
    private GameObject[] deck;

    [SerializeField]
    private int handCapacity;

    [SerializeField]
    private Actor opponent;

    private int _currentlyActingPieceIndex;
    private Piece[] _orderedPieces;

    private bool _canAct;
    private bool _isActing;
    
    private int _health;

    public void Draw()
    {
        if (!_canAct || _isActing) return;
        photonView.RPC(nameof(DrawCommon), RpcTarget.All, Random.Range(0, deck.Length));
    }

    public void SpawnPiece(Tile tile, Card card)
    {
        //Spawns piece on the tile.
        //Tiles are assigned GUIDs when created which are used to serialize over network.
        if (!_canAct || _isActing) return;
        photonView.RPC(nameof(SpawnPieceCommon), RpcTarget.All, tile.location, _hand.IndexOf(card));
    }

    public void PerformPieceActions()
    {
        if (!_canAct || _isActing) return;
        photonView.RPC(nameof(PerformPieceActionsCommon), RpcTarget.All);
    }
    
    public void EndTurn()
    {
        if (_isActing) return;
        photonView.RPC(nameof(EndTurnCommon), RpcTarget.All);
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
        _isActing = true;
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
        _isActing = true;
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
        opponent._canAct = true;
    }

    //Put RPCs down here that get called based on the application instance.
}
