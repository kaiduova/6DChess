using System.Linq;
using UnityEngine;

namespace Controllers
{
    public class AiController : Controller
    {
        [SerializeField]
        private int spawnsPerTurn, movesPerTurn;

        private int _spawnsLeft, _movesLeft;

        private bool _numbersRefreshed;

        protected override void Update()
        {
            base.Update();

            if (!Actor.CanAct) return;
            
            if (_spawnsLeft > 0 && !Actor.IsActing)
            {
                var spawnableTileArray = Board.Instance.Tiles.Where(tile => tile.SpawningActor == Actor).ToArray();
                Actor.SpawnPiece(spawnableTileArray[Random.Range(0, spawnableTileArray.Length)], Actor.Hand.Last());
                _spawnsLeft--;
            }
            
            if (_movesLeft > 0 && !Actor.IsActing)
            {
                Actor.PerformPieceActions();
                _movesLeft--;
            }

            if (_movesLeft <= 0 && _spawnsLeft <= 0 && !Actor.IsActing)
            {
                Actor.EndTurn();
                RefreshNumbers();
            }
        }

        private void RefreshNumbers()
        {
            _spawnsLeft = spawnsPerTurn;
            _movesLeft = movesPerTurn;
        }
    }
}