using System;
using UnityEngine;

public class GlobalAssetCache : MonoBehaviour
{
    public static GlobalAssetCache Instance { get; private set; }

    [SerializeField]
    public GameObject explosionPrefab;

    [SerializeField]
    public GameObject spawnFxPrefab, attackFxPrefab;

    private void Awake()
    {
        Instance = this;
    }
}