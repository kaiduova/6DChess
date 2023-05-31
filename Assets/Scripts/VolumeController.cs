using System;
using UnityEngine;

public class VolumeController: MonoBehaviour
{
    private AudioSource _audioSource;
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        _audioSource.volume = GameManager.Instance.Volume * 0.08f;
    }
}