using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LevelsScreen : Screen
{
    public LevelsData levelsData;

    public GameObject levelChoosePrefab;
    public Transform levelChooseParent;

    private List<GameObject> levelChooses = new List<GameObject>();
    public Button profileButton;
    public TMP_Text moneyText;

    public GameObject dailyBonusPanel;

    public Button closeDailyBonusButton;
    public Button dailyBonusButton;
    
    public enum PanelType
    {
        NoPanel,
        DailyBonus
    }


    private void Start()
    {
        profileButton.onClick.AddListener(() =>
        {
            UIManager.Instance.SetScreen(UIManager.ScreenType.Profile);
        });
        dailyBonusButton.onClick.AddListener(() =>
        {
            SetPanel(PanelType.DailyBonus);
        });
        closeDailyBonusButton.onClick.AddListener(() =>
        {
            SetPanel(PanelType.NoPanel);
        });
    }

    private void OnEnable()
    {
        moneyText.text = PlayerPrefs.GetInt("money", 0).ToString();
        for (int i = 0; i < levelChooses.Count; i++)
        {
            Destroy(levelChooses[i].gameObject);
            
        }
        levelChooses.Clear();
        for (int i = 0; i < levelsData.levelDatas.Length; i++)
        {
            LevelChoose levelChoose = Instantiate(levelChoosePrefab, levelChooseParent).GetComponent<LevelChoose>();
            int passedLevel = PlayerPrefs.GetInt("level", 0);
            levelChoose.Init(i + 1, levelsData.levelDatas[i].moneyWin, i <= passedLevel);
            int currentLevel = i;
            levelChoose.GetComponent<Button>().onClick.AddListener(() =>
            {
                UIManager.Instance.SetScreen(UIManager.ScreenType.Game);
                GameManager.Instance.InitGame(currentLevel, levelsData.levelDatas[currentLevel]);
            });
            levelChooses.Add(levelChoose.gameObject);
        }
    }
    
    public void SetPanel(PanelType panelType)
    {
        moneyText.text = PlayerPrefs.GetInt("money", 0).ToString();
        dailyBonusPanel.SetActive(panelType == PanelType.DailyBonus);
        dailyBonusButton.gameObject.SetActive(panelType != PanelType.DailyBonus);
    }
}
