using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileEditScreen : Screen
{
    public TMP_InputField userInput;

    public Button saveChangesButton;

    public void Start()
    {
        userInput.text = PlayerPrefs.GetString("user", "USER");
        saveChangesButton.onClick.AddListener(() =>
        {
            PlayerPrefs.SetString("user", userInput.text);
            UIManager.Instance.SetScreen(UIManager.ScreenType.Profile);
        });
    }
}
