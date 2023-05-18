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

        private Tile _currentlyPlacingTile;

        [SerializeField]
        private TMP_Text currentlyActing, time;

        [SerializeField]
        private GameObject directionalIndicator;

        public static PlayerController Instance { get; set; }

        private void Start()
        {
            _actionTimer = actionTime;

            directionalIndicator.SetActive(false);
        }

        protected override void Update()
        {
            base.Update();

            if (_currentlyPlacingTile != null)
            {
                directionalIndicator.SetActive(true);
                directionalIndicator.transform.position = _currentlyPlacingTile.transform.position;
                var plane = new Plane(Vector3.up, -2f);
                if (Camera.main == null) throw new Exception("Camera missing");
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                plane.Raycast(ray, out var enter);
                //Right side is normal.
                if (ray.GetPoint(enter).x >= _currentlyPlacingTile.transform.position.x)
                {
                    if (Actor.Side == Side.Normal)
                    {
                        directionalIndicator.transform.GetChild(0).transform.localRotation =
                            Quaternion.Euler(90, 180, 90);
                    }
                    else
                    {
                        directionalIndicator.transform.GetChild(0).transform.localRotation =
                            Quaternion.Euler(90, 0, 90);
                    }
                }
                else
                {
                    if (Actor.Side == Side.Normal)
                    {
                        directionalIndicator.transform.GetChild(0).transform.localRotation =
                            Quaternion.Euler(90, 0, 90);
                    }
                    else
                    {
                        directionalIndicator.transform.GetChild(0).transform.localRotation =
                            Quaternion.Euler(90, 180, 90);
                    }
                }
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (ray.GetPoint(enter).x >= _currentlyPlacingTile.transform.position.x)
                    {
                        SpawnOnSide(Actor.Side != Side.Normal);
                    }
                    else
                    {
                        SpawnOnSide(Actor.Side == Side.Normal);
                    }
                }
            }
            
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
                directionalIndicator.SetActive(false);
                _currentlyPlacingTile = null;
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
            if (_currentlyPlacingTile) return;
            if (_selectedCard != null)
            {
                _selectedCard.Selected = false;
            }
            _selectedCard = card;
            card.Selected = true;
            card.Dragging = true;
        }

        public void ReleasedOnTile(Tile tile)
        {
            if (_selectedCard == null || _selectedCard.gameObject == null || !Actor.CanAct || Actor.IsActing) return;
            _currentlyPlacingTile = tile;
        }

        private void SpawnOnSide(bool isFlipped)
        {
            if (_currentlyPlacingTile == null) return;
            Actor.SpawnPiece(_currentlyPlacingTile, _selectedCard, isFlipped);
            _currentlyPlacingTile = null;
            _queuedEndTurn = true;
        }
    }
}