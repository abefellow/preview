using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleGame : Game
{
    public GameObject tilePrefab;  // Prefab of each tile
    public Transform gridParent;   // The parent object of the player's grid
    public Transform goalGridParent; // The parent object of the goal grid
    public Transform animatedSpriteParent; 

    private List<GameObject> tiles;   // List of the actual tile objects for the player grid
    private List<GameObject> goalTiles; // List of the actual tile objects for the goal grid
    private int emptyIndex;      // Index of the empty spot
    private bool isMoving = false;
    private GridLayoutGroup gridLayout; // To manage grid layout
    private GridLayoutGroup goalGridLayout; // To manage the goal grid layout

    // Dynamic grid size and layout settings
    

    public override void Begin(Visual visual)
    {
        base.Begin(visual);
        gridLayout = gridParent.GetComponent<GridLayoutGroup>();
        goalGridLayout = goalGridParent.GetComponent<GridLayoutGroup>();

        SetGridLayout(gridLayout); // Set the player's grid layout
        SetGridLayout(goalGridLayout); // Set the goal's grid layout

        CreatePuzzleGrid();
        CreateGoalGrid(); // Create the goal grid to show the correct arrangement
        ShuffleGrid();
    }

    public override void End()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            Destroy(tiles[i]);
        }
        for (int i = 0; i < goalTiles.Count; i++)
        {
            Destroy(goalTiles[i]);
        }
    }

    // Set grid layout dynamically based on gridSize, padding, and spacing
    void SetGridLayout(GridLayoutGroup layoutGroup)
    {
        float parentWidth = layoutGroup.GetComponent<RectTransform>().rect.width;
        float parentHeight = layoutGroup.GetComponent<RectTransform>().rect.height;

        // Calculate the total spacing between tiles
        float totalSpacingWidth = spacing * (gridSize - 1);
        float totalSpacingHeight = spacing * (gridSize - 1);

        // Calculate available space by subtracting padding and spacing
        float availableWidth = parentWidth - (padding * 2) - totalSpacingWidth;
        float availableHeight = parentHeight - (padding * 2) - totalSpacingHeight;

        // Calculate cell size based on available space and grid size
        float cellSize = Mathf.Min(availableWidth / gridSize, availableHeight / gridSize);

        // Set GridLayoutGroup's padding and spacing
        layoutGroup.padding.left = (int)padding;
        layoutGroup.padding.right = (int)padding;
        layoutGroup.padding.top = (int)padding;
        layoutGroup.padding.bottom = (int)padding;

        layoutGroup.spacing = new Vector2(spacing, spacing);
        layoutGroup.cellSize = new Vector2(cellSize, cellSize);
    }

    // Creates a grid of tiles for the player to interact with
    void CreatePuzzleGrid()
    {
        tiles = new List<GameObject>();
        int spriteIndex = 0; // To cycle through available sprites
        emptyIndex = gridSize * gridSize - 1; // Default empty tile is the last one

        for (int i = 0; i < gridSize * gridSize; i++)
        {
            GameObject tile = Instantiate(tilePrefab, gridParent);
            tile.GetComponent<Button>().onClick.AddListener(() => OnTileClick(tile));

            // Assign sprite and track the tile
            if (i < emptyIndex) // Leave the last one empty
            {
                tile.transform.GetChild(0).GetComponent<Image>().sprite = tileSprites[spriteIndex];
                tile.name = spriteIndex.ToString();  // Assign the sprite index for win checking

                // Cycle through available sprites
                spriteIndex = (spriteIndex + 1) % tileSprites.Count;
            }
            else
            {
                // Last tile is the empty tile (the one without a sprite)
                tile.name = "Empty";  // Mark the tile as "empty"
                tile.transform.GetChild(0).GetComponent<Image>().enabled = false;  // Disable the image to make it empty
            }

            tiles.Add(tile);
        }
    }

    // Creates the goal grid that shows the correct arrangement
    void CreateGoalGrid()
    {
        goalTiles = new List<GameObject>();
        int spriteIndex = 0; // To cycle through available sprites

        for (int i = 0; i < gridSize * gridSize; i++)
        {
            GameObject tile = Instantiate(tilePrefab, goalGridParent);
            
            tile.GetComponent<Image>().pixelsPerUnitMultiplier = 1.31f;
            tile.transform.GetComponent<Button>().interactable = false;
            // Assign sprite and track the goal tile
            if (i < emptyIndex) // Leave the last one empty
            {
                tile.transform.GetChild(0).GetComponent<Image>().sprite = tileSprites[spriteIndex];

                tile.name = spriteIndex.ToString();  // Assign the sprite index for win checking

                // Cycle through available sprites
                spriteIndex = (spriteIndex + 1) % tileSprites.Count;
            }
            else
            {
                // Last tile is the empty tile (the one without a sprite)
                tile.name = "Empty";  // Mark the tile as "empty"
                tile.transform.GetChild(0).GetComponent<Image>().enabled = false;  // Disable the image to make it empty
            }

            goalTiles.Add(tile); // Add tiles to goal grid list
        }
    }

    // Shuffle the tiles to randomize the player's grid
    void ShuffleGrid()
    {
        for (int i = 0; i < 100; i++) // Perform 100 random moves to shuffle
        {
            int randomTileIndex = Random.Range(0, gridSize * gridSize);
            SwapTilesWithoutAnimation(emptyIndex, randomTileIndex);
            emptyIndex = randomTileIndex;
        }
    }

    // Handle tile click
    void OnTileClick(GameObject clickedTile)
    {
        if (isMoving) return;

        int clickedIndex = tiles.IndexOf(clickedTile);

        // Swap the clicked tile with the empty spot
        SwapTilesWithAnimation(clickedIndex, emptyIndex);
        emptyIndex = clickedIndex; // Update the empty spot's index after the swap
    }

    // Swap any clicked tile with the empty tile without animation
    void SwapTilesWithoutAnimation(int indexA, int indexB)
    {
        var tempSprite = tiles[indexA].transform.GetChild(0).GetComponent<Image>().sprite;
        var tempEnabled = tiles[indexA].transform.GetChild(0).GetComponent<Image>().enabled;

        // Swap sprite and enabled state
        tiles[indexA].transform.GetChild(0).GetComponent<Image>().sprite = tiles[indexB].transform.GetChild(0).GetComponent<Image>().sprite;
        tiles[indexA].transform.GetChild(0).GetComponent<Image>().enabled = tiles[indexB].transform.GetChild(0).GetComponent<Image>().enabled;

        tiles[indexB].transform.GetChild(0).GetComponent<Image>().sprite = tempSprite;
        tiles[indexB].transform.GetChild(0).GetComponent<Image>().enabled = tempEnabled;
    }

    // Swap tiles with animation
    private void SwapTilesWithAnimation(int indexA, int indexB)
    {
        isMoving = true; // Prevents further moves during animation

        Transform animatedTileImage = tiles[indexA].transform.GetChild(0);
        animatedTileImage.SetParent(animatedSpriteParent);

        // Animate the clicked tile moving to the empty spot
        animatedTileImage.transform.DOMove(tiles[indexB].transform.position, 0.5f).OnComplete(() =>
        {
            animatedTileImage.SetParent(tiles[indexA].transform);
            animatedTileImage.transform.position = tiles[indexA].transform.position;

            // Swap the sprites after the movement is complete
            SwapTilesWithoutAnimation(indexA, indexB);
            isMoving = false; // Allow movement again after the animation completes
            
            if (CheckForWin())
            {
                GameManager.Instance.Win();
            }
        });
    }

    // Check if all tiles in the player's grid match the goal grid (except the empty one)
    bool CheckForWin()
    {
        for (int i = 0; i < gridSize * gridSize - 1; i++) // Ignore the last empty tile
        {
            if (tiles[i].transform.GetChild(0).GetComponent<Image>().sprite != goalTiles[i].transform.GetChild(0).GetComponent<Image>().sprite)
            {
                return false; // If any tile doesn't match, return false
            }
        }
        return true; // If all tiles match, return true
    }
}
