using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using UnityEditor;
using UnityEngine;

[Serializable]
public struct TileLocationList
{
    public List<Vector2> locations;
}

#if UNITY_EDITOR

[CustomEditor(typeof(Tile))]
public class TileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var tile = (Tile)target;

        if (GUILayout.Button("Generate Surrounding Tiles"))
        {
            Board.Instance = tile.GetComponentInParent<Board>();
            tile.GenerateTiles();
        }
        
        if (GUILayout.Button("Delete Tile"))
        {
            Board.Instance = tile.GetComponentInParent<Board>();
            Board.Instance.locationList.locations.RemoveAll(loc =>
                (int)loc.x == (int)tile.location.x && (int)loc.y == (int)tile.location.y);
            DestroyImmediate(tile.gameObject);
        }
        
        if (GUILayout.Button("Reset Registry"))
        {
            Board.Instance = tile.GetComponentInParent<Board>();
            Board.Instance.locationList.locations.Clear();
        }
    }
}

#endif

public class Tile : MonoBehaviour
{
    [SerializeField]
    private GameObject tilePrefab;

    [SerializeField]
    private float maxWidth;

    [SerializeField]
    private Actor spawningActor;

    [SerializeField]
    private Actor owningActor;

    private bool _hasOwningActor;

    private Piece _currentPiece;

    /// <summary>
    /// The coordinate system used here sets the initial tile to 0, 0.
    /// The tile above that is 0, 2.
    /// The tile right of that is 2, 0.
    /// The tile top right of that is 1, 1.
    /// Essentially it's more understandable but half the coordinates are invalid.
    /// More specifically any coordinate with an odd and an even number cannot exist.
    /// </summary>
    public Vector2 location;

    public Actor SpawningActor => spawningActor;

    public Actor OwningActor => owningActor;

    public Piece CurrentPiece => _currentPiece;

    public float MaxWidth => maxWidth;

    private void Start()
    {
        Board.Instance.Tiles.Add(this);
        _hasOwningActor = owningActor != null;
    }

    private void Update()
    {
        if (_hasOwningActor && _currentPiece != null && _currentPiece.Actor == owningActor.Opponent)
        {
            owningActor.Health -= _currentPiece.Damage;
            _currentPiece.Destroy();
        }
    }

    /// <summary>
    /// Disconnects a piece from its tile without destroying it.
    /// </summary>
    public void DisconnectPieceFromTile()
    {
        _currentPiece.Tile = null;
        _currentPiece.transform.parent = null;
        _currentPiece = null;
    }

    public void TempPlacePieceOnTile(Piece piece)
    {
        piece.Tile = this;
        var pieceTransform = piece.transform;
        pieceTransform.parent = gameObject.transform;
        pieceTransform.localPosition = Vector3.zero;
    }
    
    public void SetOrReplacePieceOnTile(Piece piece)
    {
        if (_currentPiece != null) _currentPiece.Destroy();
        _currentPiece = piece;
        piece.Tile = this;
        var pieceTransform = piece.transform;
        pieceTransform.parent = gameObject.transform;
        pieceTransform.localPosition = Vector3.zero;
    }

    public void GenerateTiles()
    {
        if (!Board.Instance.locationList.locations.Any(loc => (int)loc.x == (int)location.x && (int)loc.y == (int)location.y))
            Board.Instance.locationList.locations.Add(location);
        TrySpawnTile(new Vector2(0f, 2f));
        TrySpawnTile(new Vector2(1f, 1f));
        TrySpawnTile(new Vector2(1f, -1f));
        TrySpawnTile(new Vector2(0f, -2f));
        TrySpawnTile(new Vector2(-1f, -1f));
        TrySpawnTile(new Vector2(-1f, 1f));
    }

    private void TrySpawnTile(Vector2 relativeTargetCoordinate)
    {
        if (Board.Instance.locationList.locations.Any(loc => (int)loc.x == (int)(location + relativeTargetCoordinate).x && (int)loc.y == (int)(location + relativeTargetCoordinate).y)) return;
        var go = Instantiate(tilePrefab, transform.position + RelativeCoordinateToRelativePosition(relativeTargetCoordinate, maxWidth), Quaternion.identity);
        go.transform.parent = Board.Instance.gameObject.transform;
        var tile = go.GetComponent<Tile>();
        tile.tilePrefab = tilePrefab;
        tile.maxWidth = maxWidth;
        tile.location = location + relativeTargetCoordinate;
        Board.Instance.locationList.locations.Add(tile.location);
    }

    public static Vector3 RelativeCoordinateToRelativePosition(Vector2 relativeCoordinate, float maxWidth)
    {
        if (Mathf.Abs((int)relativeCoordinate.x % 2) == 1 && Mathf.Abs((int)relativeCoordinate.y % 2) == 0 ||
            Mathf.Abs((int)relativeCoordinate.x % 2) == 0 && Mathf.Abs((int)relativeCoordinate.y % 2) == 1)
            throw new Exception("Coordinate is invalid.");
        var sideLength = maxWidth / 2f;
        var minRadius = Mathf.Cos(Mathf.PI / 6f) * sideLength;
        var z = relativeCoordinate.y * minRadius;
        var x = relativeCoordinate.x * 1.5f * sideLength;
        return new Vector3(x, 0f, z);
    }

    private void OnMouseDown()
    {
        if (spawningActor == null || spawningActor != PlayerController.Instance.Actor) return;
        PlayerController.Instance.ClickedTile(this);
    }
}
