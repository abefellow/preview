using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Random = UnityEngine.Random;

public class DailyBonusSystem : MonoBehaviour
{
    public GameObject tilePrefab; // The prefab for a single chip tile
    public Transform content; // The content object inside the ScrollView
    public ScrollRect scrollView; // The ScrollRect component to manage scrolling
    public List<Chip> chips; // List of Chip objects containing sprite and reward

    public HorizontalLayoutGroup layoutGroup; // Layout group for handling padding and spacing
    public int totalChipCount = 20; // Total number of chips to display in the ScrollView

    private bool isSpinning = false;
    private List<GameObject> spawnedChips = new List<GameObject>();
    public Button spinButton;

    public TMP_Text bonusCooldownText; // The text to display time left for the next bonus
    public GameObject scrollViewObject; // The scroll view GameObject to hide/show
    public float bonusIntervalHours = 24f; // Interval in hours for the bonus
    private System.DateTime lastBonusTime; // To track the last time bonus was collected

    public TMP_Text youWonText;
    void Start()
    {
        
        PopulateScrollView(); // Initialize and populate the ScrollView with chips
        spinButton.onClick.AddListener(OnSpinButtonPressed);
    }

    private void OnEnable()
    {
        lastBonusTime = GetLastBonusTime(); // Load the last bonus time from PlayerPrefs or other storage
        CheckBonusAvailability();
        youWonText.gameObject.SetActive(false);
    }

    void CheckBonusAvailability()
    {
        if (IsBonusAvailable())
        {
            EnableBonus();
        }
        else
        {
            DisableBonus();
            StartCoroutine(UpdateBonusCooldownText());
        }
    }

    bool IsBonusAvailable()
    {
        System.TimeSpan timeSinceLastBonus = System.DateTime.Now - lastBonusTime;
        return timeSinceLastBonus.TotalHours >= bonusIntervalHours;
    }

    void EnableBonus()
    {
        scrollViewObject.SetActive(true);
        spinButton.gameObject.SetActive(true);
        bonusCooldownText.gameObject.SetActive(false); // Hide the countdown text
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator UpdateBonusCooldownText()
    {
        while (!IsBonusAvailable())
        {
            System.TimeSpan timeUntilNextBonus = System.DateTime.Now - lastBonusTime;
            timeUntilNextBonus = System.TimeSpan.FromHours(bonusIntervalHours) - timeUntilNextBonus;

            // Update the text to show the hours and minutes left
            bonusCooldownText.text = string.Format("{0:D2}:{1:D2}:{2:D2} until next bonus",
                timeUntilNextBonus.Hours,
                timeUntilNextBonus.Minutes,
                timeUntilNextBonus.Seconds);

            yield return new WaitForSeconds(1f); // Update the text every second
        }

        // When the bonus becomes available, re-enable the scroll view and spin button
        EnableBonus();
    }

    void DisableBonus()
    {
        scrollViewObject.SetActive(false);
        spinButton.gameObject.SetActive(false);
        bonusCooldownText.gameObject.SetActive(true); // Show the countdown text
    }

    // Populates the ScrollView content with a lot of chips by repeating the base chips
    void PopulateScrollView()
    {
        // Clear any existing children
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        spawnedChips.Clear();

        // Loop through and repeat the available chips until totalChipCount is reached
        for (int i = 0; i < totalChipCount; i++)
        {
            GameObject chipTile = Instantiate(tilePrefab, content); // Create a new chip tile

            // Assign the chip sprite and display the reward amount (looping through the base chip list)
            Chip chipData = chips[i % chips.Count]; // Loop through the chips list
            chipTile.transform.GetChild(0).GetComponent<Image>().sprite = chipData.sprite; // Assuming the sprite is the first child

            // Store the chip in the list for reference
            spawnedChips.Add(chipTile);
        }
    }

    // Function to trigger the spin when "Spin" is pressed
    void OnSpinButtonPressed()
    {
        if (IsBonusAvailable())
        {
            StartCoroutine(SpinScrollView());
        }
    }

    void SaveLastBonusTime()
    {
        // Store the time in PlayerPrefs or any persistent storage
        PlayerPrefs.SetString("LastBonusTime", lastBonusTime.ToString());
        PlayerPrefs.Save();
    }

    System.DateTime GetLastBonusTime()
    {
        // Retrieve the stored last bonus time
        string savedTime = PlayerPrefs.GetString("LastBonusTime", System.DateTime.MinValue.ToString());
        return System.DateTime.Parse(savedTime);
    }

    IEnumerator SpinScrollView()
    {
        isSpinning = true;

        // Spin the scroll view content with a limited range to avoid too large a spin
        float duration = 2.5f; // Moderate spin duration
        float randomEndPos = Random.Range(0.3f, 1f); // Limit the end position to avoid too large a spin

        scrollView.horizontalNormalizedPosition = 0; // Reset position
        scrollView.DOHorizontalNormalizedPos(randomEndPos, duration).SetEase(Ease.OutCubic); // Spin and slow down

        yield return new WaitForSeconds(duration); // Wait for spin to complete

        // Adjust the scroll position to make sure 3 chips are fully in view
        AdjustScrollToFullView();

        isSpinning = false;
    }

    void AdjustScrollToFullView()
    {
        float chipWidth = 163.4555f; // Actual width of each chip without padding or spacing
        float leftPadding = 10f;     // Padding from the left
        float spacing = 25f;         // Space between each chip

        // Calculate the total width per item (chip width + spacing)
        float totalItemWidth = chipWidth + spacing;

        // Get the current x position of the content
        RectTransform contentRect = content.GetComponent<RectTransform>();
        float currentXPos = contentRect.anchoredPosition.x;

        // Adjust the current position to account for the left padding
        float adjustedXPos = currentXPos - leftPadding;

        // Find the nearest multiple of the totalItemWidth (chip width + spacing)
        float nearestXPos = Mathf.Round(adjustedXPos / totalItemWidth) * totalItemWidth;

        // Add back the left padding to align correctly
        nearestXPos += leftPadding;

        // Animate the content to the nearest multiple of the totalItemWidth
        contentRect.DOAnchorPosX(nearestXPos, 0.5f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            
            // Get the selected chips and call GrantRewards()
            List<int> selectedChips = GetSelectedChips();
            GrantRewards(selectedChips);

            // Reset the last bonus time to now, so the user has to wait for the next bonus
            lastBonusTime = System.DateTime.Now;
            SaveLastBonusTime();

            // Disable the bonus immediately after the spin
            DisableBonus();
            StartCoroutine(UpdateBonusCooldownText()); // Start updating the cooldown timer

            youWonText.gameObject.SetActive(true);
           

        });
    }

    // Grants rewards to the player based on the selected chips
    void GrantRewards(List<int> selectedChips)
    {
        int totalGold = 0;

        foreach (int chipIndex in selectedChips)
        {
            totalGold += chips[chipIndex % chips.Count].rewardAmount; // Add up the rewards from each chip
        }

        youWonText.text = $"You won {totalGold} coins";
        PlayerPrefs.SetInt("money", PlayerPrefs.GetInt("money", 0) + totalGold);
        youWonText.gameObject.SetActive(true);
        youWonText.DOFade(0, 3f).OnComplete(() =>
        {
            youWonText.gameObject.SetActive(false);
        });

        // Here you would call a function to update the player's gold
        // PlayerManager.AddGold(totalGold);
    }

    List<int> GetSelectedChips()
    {
        List<int> selectedChips = new List<int>();

        // Get the content's anchored position (to know which chips are in view)
        RectTransform contentRect = content.GetComponent<RectTransform>();
        float contentXPos = contentRect.anchoredPosition.x;

        float chipWidth = 163.4555f; // Actual width of each chip without padding or spacing
        float leftPadding = 10f;
        float spacing = 25f;
        float totalItemWidth = chipWidth + spacing;

        // Calculate the index of the first visible chip in the spawnedChips list
        float firstVisibleChipIndex = -(contentXPos - leftPadding) / totalItemWidth;  // Negative due to movement in negative x

        // Round to nearest whole number to get the index of the chip fully in view
        int startIndex = Mathf.RoundToInt(firstVisibleChipIndex);

        // Ensure the startIndex is in bounds
        startIndex = Mathf.Clamp(startIndex, 0, spawnedChips.Count - 1);

        // Add the next 3 chips as the selected chips, and map them to the original chips list
        for (int i = 0; i < 3; i++)
        {
            int chipIndex = (startIndex + i) % spawnedChips.Count; // Wrap around for spawnedChips

            // Map spawnedChips index back to original chips index
            int originalChipIndex = chipIndex % chips.Count; // Original chips have fewer entries
            selectedChips.Add(originalChipIndex);
        }

        return selectedChips;
    }
}

[System.Serializable]
public class Chip
{
    public Sprite sprite;
    public int rewardAmount;
}
