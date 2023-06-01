using System;
using Controllers;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private bool _active;

    [SerializeField]
    private GameObject pauseMenu, mainContainer, tileContainer;

    private void Start()
    {
        pauseMenu.SetActive(false);
        mainContainer.SetActive(true);
        tileContainer.SetActive(false);
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        _active = !_active;
        pauseMenu.SetActive(_active);
        Time.timeScale = _active ? 0 : 1;
    }

    public void Surrender()
    {
        Time.timeScale = 1;
        GameManager.Instance.EndSession(PlayerController.Instance.Actor.Opponent);
    }

    public void ToTileContainer()
    {
        mainContainer.SetActive(false);
        tileContainer.SetActive(true);
    }
    
    public void ToMainContainer()
    {
        mainContainer.SetActive(true);
        tileContainer.SetActive(false);
    }
}