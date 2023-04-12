using System;
using System.Linq;
using UnityEngine;

namespace Actions
{
    public class MoveAction : Action
    {
        [SerializeField]
        private Vector2 relativeMoveCoordinate;

        [SerializeField]
        private float moveDuration;

        private float _moveTimer;

        private ActionFinishCallback _callback;

        private Vector3 _origin;

        public override void PerformAction(ActionFinishCallback callback)
        {
            var destination = Piece.Actor.Side == Side.Normal ? Piece.Tile.location + relativeMoveCoordinate : Piece.Tile.location + InvertY(relativeMoveCoordinate);
            Piece.Tile.DisconnectPieceFromTile();
            var destinationTile = Board.Instance.Tiles.FirstOrDefault(tile => tile.location == destination);
            if (destinationTile == null) return;
            destinationTile.SetOrReplacePieceOnTile(Piece);
            _moveTimer = moveDuration;
            _callback = callback;
            _origin = transform.localPosition;
        }

        private void Update()
        {
            switch (_moveTimer)
            {
                case <= 0 and > -100f:
                    _moveTimer = -200f;
                    transform.localPosition = Vector3.zero;
                    _callback();
                    return;
                case < 0f:
                    return;
                default:
                    _moveTimer -= Time.deltaTime;
                    transform.localPosition = Vector3.Lerp(Vector3.zero, _origin, _moveTimer / moveDuration);
                    break;
            }
        }

        private static Vector2 InvertY(Vector2 input)
        {
            Vector2 output;
            output.x = input.x;
            output.y = -input.y;
            return output;
        }
    }
}