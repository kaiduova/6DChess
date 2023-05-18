using System;
using UnityEngine;

public class HoverDetail : MonoBehaviour
{
    private GameObject _currentlyActivePieceInfo;

    [SerializeField]
    private GameObject pieceInfoLocationMarker;
    
    private void Update()
    {
        /*
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out var hit, 100f);
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.TryGetComponent<Piece>(out var piece))
            {
                if (_currentlyActivePieceInfo != null)
                {
                    Destroy(_currentlyActivePieceInfo);
                }

                if (_currentlyActivePieceInfo == null)
                {
                    _currentlyActivePieceInfo = Instantiate(piece.pieceInfo, pieceInfoLocationMarker.transform);
                    _currentlyActivePieceInfo.transform.localPosition = Vector3.zero;
                }
            }
            if (hit.collider.gameObject.TryGetComponent<Card>(out var card))
            {
                if (_currentlyActivePieceInfo != null)
                {
                    Destroy(_currentlyActivePieceInfo);
                }

                if (_currentlyActivePieceInfo == null)
                {
                    _currentlyActivePieceInfo = Instantiate(card.PiecePrefab.GetComponent<Piece>().pieceInfo, pieceInfoLocationMarker.transform);
                    _currentlyActivePieceInfo.transform.localPosition = Vector3.zero;
                }
            }
        }
        */
    }
}