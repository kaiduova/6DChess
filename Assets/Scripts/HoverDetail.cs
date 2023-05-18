using System;
using UnityEngine;
using UnityEngine.UI;

public class HoverDetail : MonoBehaviour
{
    [SerializeField]
    private Image pieceInfoDisplay;
    
    private void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out var hit, 100f);
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.TryGetComponent<Piece>(out var piece))
            {
                if (piece.pieceInfo == null) return;
                pieceInfoDisplay.sprite = piece.pieceInfo;
            }
            if (hit.collider.gameObject.TryGetComponent<Card>(out var card))
            {
                if (card.PiecePrefab == null) return;
                if (!card.PiecePrefab.TryGetComponent<Piece>(out var cardPiece)) return;
                if (cardPiece.pieceInfo == null) return;
                pieceInfoDisplay.sprite = cardPiece.pieceInfo;
            }
        }
    }
}