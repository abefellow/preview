using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameScreen : Screen
{
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;
    
    public GameObject pausePanel;
    public GameObject howToPlayPanel;
    public GameObject winPanel;
    public GameObject losePanel;
    public Button pauseButton;
    public Button howToPlayButton;
    public Button musicButton;

    public Button exitButton;
    public Button howToPlayCloseButton;
    public Button playButton;
    public Button restartButton;
    public Button mainMenuButton;
    public TMP_Text howToPlayText;

    public Button restartButtonWin;
    public Button homeButtonWin;
    public Button restartButtonLose;
    public Button homeButtonLose;
    public TMP_Text moneyText;
    private bool isMusicOn;
    public enum PanelType
    {
        NoPanel,
        Pause,
        HowToPlay,
        Win,
        Lose
    }

    private void Start()
    {
        pauseButton.onClick.AddListener(() =>
        {
            SetPanel(PanelType.Pause);
        });
        howToPlayButton.onClick.AddListener(() =>
        {
            SetPanel(PanelType.HowToPlay);
        });
        exitButton.onClick.AddListener(() =>
        {
            SetPanel(PanelType.NoPanel);
        });
        howToPlayCloseButton.onClick.AddListener(() =>
        {
            SetPanel(PanelType.NoPanel);
        });
        playButton.onClick.AddListener(() =>
        {
            SetPanel(PanelType.NoPanel);
        });
        restartButton.onClick.AddListener(() =>
        {
            SetPanel(PanelType.NoPanel);
            GameManager.Instance.RestartGame();
        });
        restartButtonWin.onClick.AddListener(() =>
        {
            SetPanel(PanelType.NoPanel);
            GameManager.Instance.RestartGame();
        });
        restartButtonLose.onClick.AddListener(() =>
        {
            SetPanel(PanelType.NoPanel);
            GameManager.Instance.RestartGame();
        });
        mainMenuButton.onClick.AddListener(() =>
        {
            SetPanel(PanelType.NoPanel);
            UIManager.Instance.SetScreen(UIManager.ScreenType.Levels);
            GameManager.Instance.EndGame();
        });
        homeButtonWin.onClick.AddListener(() =>
        {
            SetPanel(PanelType.NoPanel);
            UIManager.Instance.SetScreen(UIManager.ScreenType.Levels);
            GameManager.Instance.EndGame();
        });
        homeButtonLose.onClick.AddListener(() =>
        {
            SetPanel(PanelType.NoPanel);
            UIManager.Instance.SetScreen(UIManager.ScreenType.Levels);
            GameManager.Instance.EndGame();
        });
        
        musicButton.onClick.AddListener(ToggleMusic);
        
        isMusicOn = PlayerPrefs.GetInt("music", 1) == 1;
        
        UpdateMusicButton();
    }

    private void ToggleMusic()
    {
        isMusicOn = !isMusicOn; // Toggle the music state
        PlayerPrefs.SetInt("music", isMusicOn ? 1 : 0); // Save the new state in PlayerPrefs
        PlayerPrefs.Save();
        AudioManager.Instance.UpdateMusicState();
        UpdateMusicButton();
    }

    private void UpdateMusicButton()
    {
        musicButton.GetComponent<Image>().sprite = (isMusicOn)? musicOnSprite : musicOffSprite;
    }
    public void SetPanel(PanelType panelType)
    {
        howToPlayText.text = GameManager.Instance.howToPlayString;
        pausePanel.SetActive(panelType == PanelType.Pause);
        howToPlayPanel.SetActive(panelType == PanelType.HowToPlay);
        winPanel.SetActive(panelType == PanelType.Win);
        losePanel.SetActive(panelType == PanelType.Lose);
        moneyText.text = $"+{GameManager.Instance.currentLevelData.moneyWin}";
    }
}
