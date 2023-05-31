using System;
using Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject mainPanel, settingsPanel, creditsPanel;

    [SerializeField]
    private int tutorialSceneIndex;

    private void Start()
    {
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    public void Play()
    {
        GameManager.Instance.GenerateSceneOrder();
        GameManager.Instance.ResetAddedCards();
        GameManager.Instance.LoadNextGameScene();
    }

    public void Tutorial()
    {
        SceneManager.LoadScene(tutorialSceneIndex);
    }
    
    public void ToMainPanel()
    {
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    public void ToSettingsPanel()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
        creditsPanel.SetActive(false);
    }

    public void ToCreditsPanel()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void OnVolumeSliderChange(float value)
    {
        PlayerPrefs.SetFloat("Volume", value);
    }

    public void Quit()
    {
        Application.Quit();
    }
}