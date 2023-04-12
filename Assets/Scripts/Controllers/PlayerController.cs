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

            if (_actionTimer <= 0f)
            {
                _queuedExecute = true;
            }

            if (_queuedExecute && !Actor.IsActing)
            {
                Actor.PerformPieceActions();
                _queuedEndTurn = true;
                _queuedExecute = false;
            }

            if (_queuedEndTurn && !Actor.IsActing)
            {
                Actor.EndTurn();
                _queuedEndTurn = false;
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
            _queuedExecute = true;
        }
    }
}