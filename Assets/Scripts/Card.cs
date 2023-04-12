using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField]
    private GameObject piecePrefab;

    public GameObject PiecePrefab => piecePrefab;
    
    public bool Selected { get; set; }

    private void OnMouseDown()
    {
        if (!PlayerController.Instance.Actor.Hand.Contains(this)) return;
        PlayerController.Instance.ClickedCard(this);
    }

    private void Update()
    {
        if (Selected)
        {
            //Implement selected effect.
        }
    }
}
