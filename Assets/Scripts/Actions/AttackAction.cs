using System;
using System.Linq;
using UnityEngine;

namespace Actions
{
    public class AttackAction : Action
    {
        [SerializeField]
        private Vector2 relativeAttackCoordinate;
        
        public override void PerformAction(ActionFinishCallback callback)
        {
            var targetTileLocation = Piece.Actor.Side == Side.Normal ? Piece.Tile.location + relativeAttackCoordinate : Piece.Tile.location + MoveAction.InvertY(relativeAttackCoordinate);
            var targetTile = Board.Instance.Tiles.FirstOrDefault(tile => tile.location == targetTileLocation);
            if (targetTile == null) return;
            if (targetTile.CurrentPiece == null) return;
            targetTile.CurrentPiece.Destroy();
            callback();
        }
    }
}