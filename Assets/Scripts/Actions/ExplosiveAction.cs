using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Actions
{
    public class ExplosiveAction : Action
    {
        [SerializeField]
        private float turnsUntilExplode;

        private int _turnCounter;
        
        public override void PerformAction(ActionFinishCallback callback)
        {
            _turnCounter++;

            if (_turnCounter >= turnsUntilExplode)
            {
                foreach (var direction in Enum.GetValues(typeof(Direction)))
                {
                    var coordinate = MoveAction.TranslateToRelativeCoordinate((Direction)direction);
                    var targetTileLocation = Piece.Tile.location + coordinate;
                    var targetTile = Board.Instance.Tiles.FirstOrDefault(tile => tile.location == targetTileLocation);
                    if (targetTile == null || targetTile.CurrentPiece == null)
                    {
                        continue;
                    }
                    targetTile.CurrentPiece.Destroy();
                }
                
                Piece.Destroy();
            }

            callback();
        }
    }
}