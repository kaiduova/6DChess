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

        public override void PerformAction(ActionFinishCallback callback)
        {
            var processedRelativeAttackCoordinate = relativeAttackCoordinate;
            if (Piece.Actor.Side != Side.Normal)
            {
                processedRelativeAttackCoordinate = MoveAction.InvertY(processedRelativeAttackCoordinate);
            }

            if (Piece.IsFlipped)
            {
                processedRelativeAttackCoordinate = MoveAction.InvertX(processedRelativeAttackCoordinate);
            }
            var targetTileLocation = Piece.Tile.location + processedRelativeAttackCoordinate;
            var targetTile = Board.Instance.Tiles.FirstOrDefault(tile => tile.location == targetTileLocation);
            if (targetTile == null || targetTile.CurrentPiece == null || targetTile.CurrentPiece.Actor == Piece.Actor)
            {
                callback();
                return;
            }

            StartCoroutine(PerformAttack(1, targetTile, callback));
        }
        
        private IEnumerator PerformAttack(float inAttackTime, Tile inTargetTile, ActionFinishCallback callback)
        {
            Piece.StopRotationLock = true;
            var isVengeful = inTargetTile.CurrentPiece.isVengeful;
            var originalRotation = Piece.transform.GetChild(0).rotation;
            //Rotate and play animation.
            var lookRotation =
                Quaternion.LookRotation(inTargetTile.transform.position - transform.position, Vector3.up);
            Transform pieceTransform;
            (pieceTransform = Piece.transform).GetChild(0).rotation = Quaternion.Euler(originalRotation.eulerAngles.x, lookRotation.eulerAngles.y, originalRotation.eulerAngles.z);
            Instantiate(GlobalAssetCache.Instance.attackFxPrefab, pieceTransform.position + Vector3.up * 1f,
                Quaternion.Euler(0, lookRotation.eulerAngles.y, 0));
            
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