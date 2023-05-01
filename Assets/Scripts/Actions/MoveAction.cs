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
        
        private Tile _destinationTile;

        private bool _moving;

        public override void PerformAction(ActionFinishCallback callback)
        {
            var destination = Piece.Actor.Side == Side.Normal ? Piece.Tile.location + relativeMoveCoordinate : Piece.Tile.location + InvertY(relativeMoveCoordinate);
            _destinationTile = Board.Instance.Tiles.FirstOrDefault(tile => tile.location == destination);
            if (_destinationTile == null) return;
            _moveTimer = moveDuration;
            _callback = callback;
            _moving = true;
        }

        private void Update()
        {
            if (_moveTimer > 0f)
            {
                _moveTimer -= Time.deltaTime;
                transform.position = Vector3.Lerp(Piece.Tile.transform.position, _destinationTile.transform.position, 1 - (_moveTimer / moveDuration));
            }
            else if (_moving)
            {
                _moving = false;
                Piece.Tile.DisconnectPieceFromTile();
                _destinationTile.SetOrReplacePieceOnTile(Piece);
                _callback();
            }
        }

        public static Vector2 InvertY(Vector2 input)
        {
            Vector2 output;
            output.x = input.x;
            output.y = -input.y;
            return output;
        }
    }
}