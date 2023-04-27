using System;
using System.Linq;
using UnityEngine;

namespace Controllers
{
    public class PlayerController : Controller
    {
        [SerializeField]
        private float actionTime;

        private float _actionTimer;

        private Card _selectedCard;

        private bool _queuedExecute;
        
        private bool _queuedEndTurn;

        private bool _moved;

        public static PlayerController Instance { get; set; }

        private void Awake()
        {
            Instance = this;
        }
        
        protected override void Update()
        {
            base.Update();
            if (!Actor.CanAct) _actionTimer = actionTime;
            else
            {
                _actionTimer -= Time.deltaTime;
            }
            
            if (!Actor.IsActing && !_moved)
            {
                Actor.PerformPieceActions();
            }
            
            if (_actionTimer <= 0f)
            {
                _queuedEndTurn = true;
            }

            if (_queuedEndTurn && !Actor.IsActing)
            {
                Actor.EndTurn();
                _queuedEndTurn = false;
                _moved = false;
            }
        }

        public void ClickedCard(Card card)
        {
            _selectedCard.Selected = false;
            _selectedCard = card;
            card.Selected = true;
        }

        public void ClickedTile(Tile tile)
        {
            if (_selectedCard == null || _selectedCard.gameObject == null || !Actor.CanAct) return;
            Actor.SpawnPiece(tile, _selectedCard);
            _queuedEndTurn = true;
        }
    }
}