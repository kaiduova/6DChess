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

    private Vector3 _startingScale;

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
        if (!PlayerController.Instance.Actor.CanAct || PlayerController.Instance.Actor.IsActing /*|| Input.GetKeyUp(KeyCode.Mouse0)*/)
        {
            Selected = false;
        }

        /*
        if (Selected)
        {
            var plane = new Plane(Vector3.up, 2f);
            if (Camera.main != null)
            {
                plane.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), );
            }
            if ()
            transform.position =
        }
        */
        
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
