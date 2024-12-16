using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Levels", fileName = "Level")]
public class LevelsData : ScriptableObject
{
    public LevelData[] levelDatas;
}

[System.Serializable]
public class LevelData
{
    public GameManager.GameType gameType;
    public Visual visuals;
    public int gridSize;
    public int moneyWin;
}
