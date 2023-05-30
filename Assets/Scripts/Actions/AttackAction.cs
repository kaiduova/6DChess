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

            StartCoroutine(PerformAttack(attackTime, targetTile, callback));
        }
        
        private IEnumerator PerformAttack(float inAttackTime, Tile inTargetTile, ActionFinishCallback callback)
        {
            Piece.StopRotationLock = true;
            var isVengeful = inTargetTile.CurrentPiece.isVengeful;
            var originalRotation = Piece.transform.GetChild(0).rotation;
            //Rotate and play animation.
            var lookRotation =
                Quaternion.LookRotation(inTargetTile.transform.position - transform.position, Vector3.up);
            Piece.transform.GetChild(0).rotation = Quaternion.Euler(originalRotation.eulerAngles.x, lookRotation.y, originalRotation.eulerAngles.z);

            yield return new WaitForSeconds(inAttackTime);
            
            inTargetTile.CurrentPiece.Destroy();
            if (isVengeful)
            {
                Piece.QueueDestroy = true;
            }
            callback();
            Piece.transform.GetChild(0).rotation = originalRotation;
            Piece.StopRotationLock = false;
        }
    }
}