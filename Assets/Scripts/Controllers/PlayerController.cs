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

        [SerializeField]
        private TMP_Text currentlyActing, time;

        public static PlayerController Instance { get; set; }

        protected override void Update()
        {
            base.Update();
            Instance = this;
            if (!Actor.CanAct || _moved && Actor.IsActing) _actionTimer = actionTime;
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