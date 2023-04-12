using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Instance { get; set; }

    public TileLocationList locationList = new()
    {
        locations = new List<Vector2>()
    };
    
    public readonly List<Tile> Tiles = new();

    private void Awake()
    {
        Instance = this;
    }
}
