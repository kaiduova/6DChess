using System;
using System.Linq;
using UnityEngine;

namespace Actions
{
    public enum Direction
    {
        North,
        NorthEast,
        SouthEast,
        South,
        SouthWest,
        NorthWest
    }
    
    public class MoveAction : Action
    {
        [SerializeField]
        private Direction movePattern;

        [SerializeField, Header("MAKE SURE THE LAST MOVE IS NON-JUMP!")]
        private bool isJump;

        [SerializeField]
        private float moveDuration;

        private float _moveTimer;

        private ActionFinishCallback _callback;
        
        private Tile _destinationTile;

        private bool _moving;

        private int _pauseCounter;

        [SerializeField]
        private int pauseTurnsAfterMove;

        public static Vector2 TranslateToRelativeCoordinate(Direction direction)
        {
            return direction switch
            {
                Direction.North => new Vector2(0, 2),
                Direction.NorthEast => new Vector2(1, 1),
                Direction.SouthEast => new Vector2(1, -1),
                Direction.South => new Vector2(0, -2),
                Direction.SouthWest => new Vector2(-1, -1),
                Direction.NorthWest => new Vector2(-1, 1),
                _ => Vector2.zero
            };
        }
        
        public override void PerformAction(ActionFinishCallback callback)
        {
            if (Piece.InSludge)
            {
                callback();
                return;
            }

            if (_pauseCounter > 0)
            {
                _pauseCounter--;
                callback();
                return;
            }

            Tile destinationTile;
            if (!Piece.IsFlipped)
            {
                if (!TryNormalMove(out destinationTile))
                {
                    if (TryFlippedMove(out destinationTile))
                    {
                        Piece.IsFlipped = true;
                    }
                    else
                    {
                        throw new Exception("Piece predicted to walk off the board.");
                    }
                }
            }
            else
            {
                if (!TryFlippedMove(out destinationTile))
                {
                    if (TryNormalMove(out destinationTile))
                    {
                        Piece.IsFlipped = false;
                    }
                    else
                    {
                        throw new Exception("Piece predicted to walk off the board.");
                    }
                }
            }

            _destinationTile = destinationTile;
            _moveTimer = moveDuration;
            _callback = callback;
            _moving = true;
            _pauseCounter = pauseTurnsAfterMove;
        }

        private void Update()
        {
            if (_moveTimer > 0f)
            {
                _moveTimer -= Time.deltaTime;
                transform.position = Vector3.Lerp(Piece.Tile.transform.position, _destinationTile.transform.position, 1 - _moveTimer / moveDuration);
            }
            else if (_moving)
            {
                _moving = false;
                if (Piece.Tile.CurrentPiece == Piece)
                {
                    Piece.Tile.DisconnectPieceFromTile();
                }
                if (isJump)
                {
                    _destinationTile.TempPlacePieceOnTile(Piece);
                }
                else
                {
                    _destinationTile.SetOrReplacePieceOnTile(Piece);
                }

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
        
        public static Vector2 InvertX(Vector2 input)
        {
            Vector2 output;
            output.x = -input.x;
            output.y = input.y;
            return output;
        }

        private bool TryNormalMove(out Tile destinationTile)
        {
            var coordinate = TranslateToRelativeCoordinate(movePattern);
            var destination = Piece.Actor.Side == Side.Normal
                ? Piece.Tile.location + coordinate
                : Piece.Tile.location + InvertY(coordinate);
            destinationTile = Board.Instance.Tiles.FirstOrDefault(tile => tile.location == destination);
            return destinationTile != null;
        }

        private bool TryFlippedMove(out Tile destinationTile)
        {
            var coordinate = InvertX(TranslateToRelativeCoordinate(movePattern));
            var destination = Piece.Actor.Side == Side.Normal
                ? Piece.Tile.location + coordinate
                : Piece.Tile.location + InvertY(coordinate);
            destinationTile = Board.Instance.Tiles.FirstOrDefault(tile => tile.location == destination);
            return destinationTile != null;
        }
    }
}