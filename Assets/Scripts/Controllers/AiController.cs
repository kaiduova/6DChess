using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Controllers
{
    public class AiController : Controller
    {
        [SerializeField]
        private int spawnsPerTurn, movesPerTurn;

        private int _spawnsLeft, _movesLeft;

        private readonly List<Tile> _occupiedTiles = new();

        private void Start()
        {
            RefreshNumbers();
        }

        protected override void Update()
        {
            base.Update();

            if (!Actor.CanAct) return;

            if (_movesLeft > 0 && !Actor.IsActing)
            {
                Actor.PerformPieceActions();
                _movesLeft--;
            }
            
            if (_spawnsLeft > 0 && !Actor.IsActing)
            {
                var spawnableTileList = Board.Instance.Tiles.Where(tile => tile.SpawningActor == Actor).ToList();
                spawnableTileList.RemoveAll(tile => _occupiedTiles.Contains(tile));
                var tileToSpawnOn = spawnableTileList[Random.Range(0, spawnableTileList.Count)];
                Actor.SpawnPiece(tileToSpawnOn, Actor.Hand.Last(), RandomBool());
                _occupiedTiles.Add(tileToSpawnOn);
                _spawnsLeft--;
            }

            if (_movesLeft <= 0 && _spawnsLeft <= 0 && !Actor.IsActing)
            {
                _occupiedTiles.Clear();
                Actor.EndTurn();
                RefreshNumbers();
            }
        }

        private void RefreshNumbers()
        {
            _spawnsLeft = spawnsPerTurn;
            _movesLeft = movesPerTurn;
        }
        
        public static bool RandomBool()
        {
            return Random.Range(0, 2) == 0;
        }
    }
}