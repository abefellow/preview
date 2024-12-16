using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Check if there's already an instance of UIManager
        if (Instance != null && Instance != this)
        {
            // If there is another instance, destroy this one
            Destroy(gameObject);
        }
        else
        {
            // Set the instance to this UIManager and mark it as persistent
            Instance = this;
        }
    }
    

    public enum GameType
    {
        Puzzle,
        Memory,
        Match3
    }

    public PuzzleGame puzzleGame;
    public MemoryGame memoryGame;
    public Match3Game match3Game;

    public string puzzleGameHowToPlayText;
    public string memoryGameHowToPlayText;
    public string match3GameHowToPlayText;

    private Game currentGame;

    public string howToPlayString;

    public LevelData currentLevelData;
    public int currentLevelIndex;


    public void EndGame()
    {
        if (currentGame != null)
        {
            currentGame.End();
        }
    }

    public void RestartGame()
    {
        EndGame();
        InitGame(currentLevelIndex, currentLevelData);
    }

    public void Win()
    {
        int passedLevel = PlayerPrefs.GetInt("level", 0);
        if (currentLevelIndex == passedLevel)
        {
            if (currentLevelIndex + 1 < FindObjectOfType<LevelsScreen>().levelsData.levelDatas.Length)
            {
                PlayerPrefs.SetInt("level", passedLevel+1);
            }
            
        }

        PlayerPrefs.SetInt("money", PlayerPrefs.GetInt("money") + currentLevelData.moneyWin);
        UIManager.Instance.gameScreen.SetPanel(GameScreen.PanelType.Win);
    }

    public void InitGame(int levelIndex, LevelData levelData)
    {
        currentLevelIndex = levelIndex;
        currentLevelData = levelData;
        GameType type = levelData.gameType;
        switch (type)
        {
            case GameType.Puzzle:
                currentGame = puzzleGame;
                howToPlayString = puzzleGameHowToPlayText;
                break;
            case GameType.Memory:
                currentGame = memoryGame;
                howToPlayString = memoryGameHowToPlayText;
                break;
            case GameType.Match3:
                currentGame = match3Game;
                howToPlayString = match3GameHowToPlayText;
                break;
            default:
                currentGame = null;
                break;
        }

        if (currentGame == null)
        {
            Debug.LogError("No such game type");
            return;
        }

        currentGame.gridSize = levelData.gridSize;
        currentGame.Begin(levelData.visuals);
        
        puzzleGame.gameObject.SetActive(type == GameType.Puzzle);
        memoryGame.gameObject.SetActive(type == GameType.Memory);
        match3Game.gameObject.SetActive(type == GameType.Match3);
    }
}
