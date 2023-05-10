using System;
using System.Globalization;
using System.Linq;
using TMPro;
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

        private bool _showingIcons;

        [SerializeField]
        private TMP_Text currentlyActing, time;

        public static PlayerController Instance { get; set; }

        private void Start()
        {
            _actionTimer = actionTime;
        }

        protected override void Update()
        {
            base.Update();
            //Locking mechanism where Actor.IsActing is set to true by time-based functions that are responsible
            //for setting it back to false when they're done. Calls therefore go from top to bottom.
            Instance = this;
            if (!Actor.CanAct || Actor.IsActing) _actionTimer = actionTime;
            else
            {
                _actionTimer -= Time.deltaTime;
            }

            if (!Actor.IsActing && !_moved && Actor.CanAct)
            {
                Actor.PerformPieceActions();
                _moved = true;
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
            
            //Non-locking.
            if (!Actor.IsActing && Actor.CanAct)
            {
                foreach (var piece in Actor.Pieces)
                {
                    _showingIcons = true;
                    piece.ShowIcons();
                }
            }
            else if (_showingIcons)
            {
                foreach (var piece in Actor.Pieces)
                {
                    _showingIcons = false;
                    piece.HideIcons();
                }
            }

            if (Actor.CanAct && !Actor.IsActing)
            {
                currentlyActing.text = "You may pick a piece to spawn.";
            }
            else
            {
                currentlyActing.text = "Please wait.";
            }

            time.text = "Time remaining: " + Mathf.Clamp((int)_actionTimer, 0, 999999);
        }

        public void ClickedCard(Card card)
        {
            if (_selectedCard != null) _selectedCard.Selected = false;
            _selectedCard = card;
            card.Selected = true;
        }

        public void ClickedTile(Tile tile)
        {
            if (_selectedCard == null || _selectedCard.gameObject == null || !Actor.CanAct || Actor.IsActing) return;
            Actor.SpawnPiece(tile, _selectedCard);
            _queuedEndTurn = true;
        }
    }
}