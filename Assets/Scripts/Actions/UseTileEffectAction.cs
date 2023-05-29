using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Actions
{
    public class UseTileEffectAction : Action
    {
        private ActionFinishCallback _callback;
        
        [SerializeField]
        private float arrowMoveDuration;

        private float _arrowMoveTimer;
        private bool _arrowMoving;
        private Tile _arrowDestinationTile;

        public override void PerformAction(ActionFinishCallback callback)
        {
            while (true)
            {
                _callback = callback;
                switch (Piece.Tile.tileEffect)
                {
                    case TileEffect.Random:
                        IList enumValues = Enum.GetValues(typeof(TileEffect));
                        Piece.Tile.tileEffect = (TileEffect)enumValues[Random.Range(2, enumValues.Count - 1)];
                        if (Piece.Tile.randomOriginalVisual != null)
                        {
                            Piece.Tile.randomOriginalVisual.SetActive(false);
                        }

                        if (Piece.Tile.randomInstakillVisual != null && Piece.Tile.tileEffect == TileEffect.Instakill)
                        {
                            Piece.Tile.randomInstakillVisual.SetActive(true);
                        }
                        
                        if (Piece.Tile.randomSludgeVisual != null && Piece.Tile.tileEffect == TileEffect.Sludge)
                        {
                            Piece.Tile.randomSludgeVisual.SetActive(true);
                        }
                        
                        if (Piece.Tile.randomTeleportVisual != null && Piece.Tile.tileEffect == TileEffect.Teleport)
                        {
                            Piece.Tile.randomTeleportVisual.SetActive(true);
                        }

                        continue;
                    case TileEffect.Teleport:
                        var oldTile = Piece.Tile;
                        Piece.Tile.DisconnectPieceFromTile();
                        oldTile.teleportTarget.SetOrReplacePieceOnTile(Piece);
                        callback();
                        return;
                    case TileEffect.Instakill:
                        Piece.QueueDestroy = true;
                        callback();
                        return;
                    case TileEffect.Sludge:
                        Piece.InSludge = true;
                        callback();
                        return;
                    case TileEffect.Arrow:
                        if (Piece.Tile.arrowSide != Piece.Actor.Side)
                        {
                            callback();
                            return;
                        }
                        var coordinate = MoveAction.TranslateToRelativeCoordinate(Piece.Tile.arrowTarget);
                        var destination = Piece.Tile.location + coordinate;
                        _arrowDestinationTile = Board.Instance.Tiles.FirstOrDefault(tile => tile.location == destination);
                        if (_arrowDestinationTile == null) throw new Exception("Arrow tile points to missing tile.");
                        _arrowMoveTimer = arrowMoveDuration;
                        _arrowMoving = true;
                        return;
                    default:
                        callback();
                        return;
                }
            }
        }

        private void Update()
        {
            if (_arrowMoveTimer > 0f)
            {
                _arrowMoveTimer -= Time.deltaTime;
                transform.position = Vector3.Lerp(Piece.Tile.transform.position, _arrowDestinationTile.transform.position, 1 - _arrowMoveTimer / arrowMoveDuration);
            }
            else if (_arrowMoving)
            {
                _arrowMoving = false;
                Piece.Tile.DisconnectPieceFromTile();
                _arrowDestinationTile.SetOrReplacePieceOnTile(Piece);

                if (Piece.Tile.tileEffect != TileEffect.None)
                {
                    PerformAction(_callback);
                }
                else
                {
                    _callback();
                }
            }
        }
    }
}