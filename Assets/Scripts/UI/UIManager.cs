using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

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
    
    public enum ScreenType
    {
        Game,
        Levels,
        Profile,
        ProfileEdit
    }

    public GameScreen gameScreen;
    public Screen levelsScreen;
    public Screen profileScreen;
    public Screen profileEditScreen;
    public void SetScreen(ScreenType type)
    {
        gameScreen.gameObject.SetActive(type == ScreenType.Game);
        levelsScreen.gameObject.SetActive(type == ScreenType.Levels);
        profileScreen.gameObject.SetActive(type == ScreenType.Profile);
        profileEditScreen.gameObject.SetActive(type == ScreenType.ProfileEdit);
    }
}
