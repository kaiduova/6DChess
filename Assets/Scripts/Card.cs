using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField]
    private GameObject piecePrefab;

    public GameObject PiecePrefab => piecePrefab;
}
