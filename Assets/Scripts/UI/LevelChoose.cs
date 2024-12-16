using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelChoose : MonoBehaviour
{
    public TMP_Text levelNumberText;

    public TMP_Text levelCoinText;

    public GameObject availableObj;
    public GameObject unavailableObj;

    public void Init(int levelNumber, int levelCoin, bool available)
    {
        levelNumberText.text = levelNumber.ToString();
        levelCoinText.text = levelCoin.ToString();
        availableObj.SetActive(available);
        unavailableObj.SetActive(!available);
        GetComponent<Button>().interactable = available;
    }
}
