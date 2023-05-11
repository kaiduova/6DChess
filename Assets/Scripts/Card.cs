using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Card : MonoBehaviour
{
    [SerializeField]
    private GameObject piecePrefab;

    public GameObject PiecePrefab => piecePrefab;
    
    public bool Selected { get; set; }

    public bool Dragging { get; set; }

    private Vector3 _startingScale;

    public Vector3 IntendedPosition { get; set; }
    
    public Actor Actor { get; set; }

    private void Awake()
    {
        _startingScale = transform.localScale;
    }

    private void OnMouseDown()
    {
        if (!PlayerController.Instance.Actor.Hand.Contains(this)) return;
        PlayerController.Instance.ClickedCard(this);
    }

    private void Update()
    {
        if (!PlayerController.Instance.Actor.CanAct || PlayerController.Instance.Actor.IsActing)
        {
            Selected = false;
            if (Dragging)
            {
                Dragging = false;
                transform.position = IntendedPosition;
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Dragging = false;
            transform.position = IntendedPosition;
        }

        if (!Dragging)
        {
            IntendedPosition = transform.position;
        }

        if (Dragging)
        {
            var plane = new Plane(Vector3.up, -2f);
            if (Camera.main != null)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                plane.Raycast(ray, out var enter);
                ray.GetPoint(enter);
                transform.position = ray.GetPoint(enter);
            }
        }

        if (Selected && transform.localScale == _startingScale)
        {
            transform.localScale = _startingScale * 1.2f;
        }
        
        if (!Selected && transform.localScale != _startingScale)
        {
            transform.localScale = _startingScale;
        }
    }
}
