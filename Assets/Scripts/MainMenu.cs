using System;
using Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject mainPanel, settingsPanel, creditsPanel;

    [SerializeField]
    private int tutorialSceneIndex, mainMenuSceneIndex;

    [SerializeField]
    private TMP_Text scoreText;

    [SerializeField]
    private Slider volumeSlider;

    private void Start()
    {
        if (mainPanel != null && settingsPanel != null && creditsPanel != null)
        {
            mainPanel.SetActive(true);
            settingsPanel.SetActive(false);
            creditsPanel.SetActive(false);
        }

        if (volumeSlider != null)
        {
            volumeSlider.value = GameManager.Instance.Volume;
        }
    }

    private void Update()
    {
        if (scoreText != null)
        {
            scoreText.text = "Levels completed: " + GameManager.Instance.CurrentIndexOnSceneOrder;
        }
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

    public void OnVolumeSliderChange(Single value)
    {
        PlayerPrefs.SetFloat("Volume", value);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneIndex);
    }
}