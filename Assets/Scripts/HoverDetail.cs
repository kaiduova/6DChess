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
                pieceInfoDisplay.sprite = piece.pieceInfo;
            }
            if (hit.collider.gameObject.TryGetComponent<Card>(out var card))
            {
                pieceInfoDisplay.sprite = card.PiecePrefab.GetComponent<Piece>().pieceInfo;
            }
        }
    }
}