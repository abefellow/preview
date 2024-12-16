using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileScreen : Screen
{
    public Button deleteProgressButton;
    public Button homeButton;
    public Button editButton;
    public TMP_Text userName;

    private void Start()
    {
        deleteProgressButton.onClick.AddListener(() =>
        {
            PlayerPrefs.DeleteAll();
        });
        homeButton.onClick.AddListener(() =>
        {
            UIManager.Instance.SetScreen(UIManager.ScreenType.Levels);
        });
        editButton.onClick.AddListener(() =>
        {
            UIManager.Instance.SetScreen(UIManager.ScreenType.ProfileEdit);
        });
    }

    private void OnEnable()
    {
        userName.text = PlayerPrefs.GetString("user", "USER");
    }
}
