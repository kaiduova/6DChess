using System;
using UnityEngine;

public class GlobalAssetCache : MonoBehaviour
{
    public static GlobalAssetCache Instance { get; private set; }

    [SerializeField]
    public GameObject explosionPrefab;

    private void Awake()
    {
        Instance = this;
    }
}