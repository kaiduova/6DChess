using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Actions
{
    public class AttackAction : Action
    {
        [SerializeField]
        private Vector2 relativeAttackCoordinate;

        [SerializeField]
        private Direction animationDirection;

        [SerializeField]
        private float attackTime;
        
        public override void PerformAction(ActionFinishCallback callback)
        {
            var targetTileLocation = Piece.Actor.Side == Side.Normal ? Piece.Tile.location + relativeAttackCoordinate : Piece.Tile.location + MoveAction.InvertY(relativeAttackCoordinate);
            var targetTile = Board.Instance.Tiles.FirstOrDefault(tile => tile.location == targetTileLocation);
            if (targetTile == null || targetTile.CurrentPiece == null)
            {
                callback();
                return;
            }

            var isVengeful = targetTile.CurrentPiece.isVengeful;
            targetTile.CurrentPiece.Destroy();
            if (isVengeful)
            {
                Piece.QueueDestroy = true;
            }
            callback();
        }

        /*
        private IEnumerator PerformAttack(float inAttackTime, Direction inAnimationDirection, )
        {
            Piece.StopRotationLock = true;
        }
        */
    }
}