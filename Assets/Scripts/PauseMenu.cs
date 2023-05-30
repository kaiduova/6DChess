using System;
using Controllers;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private bool _active;

    [SerializeField]
    private GameObject pauseMenu;
    
    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        _active = !_active;
        pauseMenu.SetActive(_active);
    }

    public void Surrender()
    {
        GameManager.Instance.EndSession(PlayerController.Instance.Actor.Opponent);
    }
}