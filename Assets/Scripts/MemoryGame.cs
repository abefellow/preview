using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MemoryGame : Game
{
    public GameObject tilePrefab; // Prefab of each tile
    public Transform gridParent; // The parent object of the grid
    public Sprite unrevealedSprite;
    private List<GameObject> tiles; // List of the actual tile objects
    private List<Sprite> tileAssignments; // Store shuffled image assignments
    private List<int> revealedTiles = new List<int>(); // Indices of revealed tiles
    public GridLayoutGroup gridParentComponent;
    private bool isChecking = false; // Prevents further clicks during checking
    private bool isGameOver = false;
    private bool[] isRevealed; // Keeps track of revealed state for each tile


    public override void Begin(Visual visual)
    {
        base.Begin(visual);
        CreateMemoryGrid(); // Create the grid for the memory game
        // gridParentComponent.enabled = false;
    }

    public override void End()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            Destroy(tiles[i]);
        }
    }

    // Creates a 4x4 grid of tiles for the memory game
    void CreateMemoryGrid()
    {
        tiles = new List<GameObject>();
        tileAssignments = new List<Sprite>();
        isRevealed = new bool[gridSize * gridSize]; // Initialize all tiles as not revealed

        // Set GridLayoutGroup's padding and cell size based on grid size and parent dimensions
        float parentWidth = gridParentComponent.GetComponent<RectTransform>().rect.width;
        float parentHeight = gridParentComponent.GetComponent<RectTransform>().rect.height;

        // Calculate the total spacing between tiles in both dimensions
        float totalSpacingWidth = spacing * (gridSize - 1); // Total horizontal spacing
        float totalSpacingHeight = spacing * (gridSize - 1); // Total vertical spacing

        // Calculate the available width and height by subtracting padding and spacing
        float availableWidth = parentWidth - (padding * 2) - totalSpacingWidth;
        float availableHeight = parentHeight - (padding * 2) - totalSpacingHeight;

        // Calculate cell size based on available space and grid size
        float cellSize = Mathf.Min(availableWidth / gridSize, availableHeight / gridSize);

        // Set the padding for the GridLayoutGroup
        gridParentComponent.padding.left = (int)padding;
        gridParentComponent.padding.right = (int)padding;
        gridParentComponent.padding.top = (int)padding;
        gridParentComponent.padding.bottom = (int)padding;

        // Set the spacing for the GridLayoutGroup
        gridParentComponent.spacing = new Vector2(spacing, spacing);

        // Set the cell size
        gridParentComponent.cellSize = new Vector2(cellSize, cellSize);

        // Determine how many pairs are needed for the grid
        int totalTiles = gridSize * gridSize;
        int pairsNeeded = totalTiles / 2; // Half the tiles are pairs
        int spriteIndex = 0;

        // Fill `tileAssignments` with repeated sprites
        for (int i = 0; i < pairsNeeded; i++)
        {
            Sprite sprite = tileSprites[spriteIndex];
            tileAssignments.Add(sprite); // First of the pair
            tileAssignments.Add(sprite); // Second of the pair

            spriteIndex = (spriteIndex + 1) % tileSprites.Count; // Cycle through available sprites
        }

        // Shuffle the paired sprites
        ShuffleSprites();

        // Create the grid and assign the shuffled sprites
        for (int i = 0; i < totalTiles; i++)
        {
            GameObject tile = Instantiate(tilePrefab, gridParent);
            tile.GetComponent<Button>().onClick.AddListener(() => OnTileClick(tile));
            tile.transform.GetChild(0).GetComponent<Image>().sprite = unrevealedSprite;
            tile.name = i.ToString(); // Store index of the tile as its name
            tiles.Add(tile);
        }
    }

    // Shuffle the sprite assignments
    void ShuffleSprites()
    {
        for (int i = 0; i < tileAssignments.Count; i++)
        {
            Sprite temp = tileAssignments[i];
            int randomIndex = Random.Range(0, tileAssignments.Count);
            tileAssignments[i] = tileAssignments[randomIndex];
            tileAssignments[randomIndex] = temp;
        }
    }

    // Handle tile click
    void OnTileClick(GameObject clickedTile)
    {
        if (isChecking || isGameOver) return; // Prevent clicks if checking or game is over

        int clickedIndex = int.Parse(clickedTile.name); // Get the index from the tile's name

        if (isRevealed[clickedIndex]) return; // Prevent re-clicking an already revealed tile

        // Reveal the tile by flipping it
        RevealTile(clickedTile, clickedIndex);

        revealedTiles.Add(clickedIndex);

        // Check if we have two tiles revealed
        if (revealedTiles.Count == 2)
        {
            isChecking = true; // Prevent further clicks while checking
            StartCoroutine(CheckForMatch());
        }
    }

    // Flips a tile to reveal the hidden image
    void RevealTile(GameObject tile, int index)
    {
        tile.transform.DOScaleX(0, 0.3f).OnComplete(() =>
        {
            // Once the tile is "flipped", reveal the hidden sprite
            tile.transform.GetChild(0).GetComponent<Image>().sprite = tileAssignments[index];
            tile.transform.DOScaleX(1, 0.3f);
            isRevealed[index] = true;
        });
    }

    // Flips a tile back to hide the image
    void HideTile(GameObject tile, int index)
    {
        tile.transform.DOScaleX(0, 0.3f).OnComplete(() =>
        {
            tile.transform.GetChild(0).GetComponent<Image>().sprite = unrevealedSprite; // Hide the sprite
            tile.transform.DOScaleX(1, 0.3f);
            isRevealed[index] = false;
        });
    }

    // Checks if the two revealed tiles are a match
    IEnumerator CheckForMatch()
    {
        yield return new WaitForSeconds(1f); // Wait for 1 second before checking

        int indexA = revealedTiles[0];
        int indexB = revealedTiles[1];

        // Check if the revealed tiles match
        if (tileAssignments[indexA] == tileAssignments[indexB])
        {
            // Tiles match, disable them (remove from game)
            RemoveTile(tiles[indexA]);
            RemoveTile(tiles[indexB]);
        }
        else
        {
            // Tiles do not match, flip them back over
            HideTile(tiles[indexA], indexA);
            HideTile(tiles[indexB], indexB);
        }

        revealedTiles.Clear(); // Reset revealed tiles list
        isChecking = false; // Allow new clicks

        // Check if the game is over (all tiles are matched)
        
    }

    // Removes the tile from play after matching
    void RemoveTile(GameObject tile)
    {
        tile.transform.DOScale(0, 0.5f).OnComplete(() =>
        {
            tile.GetComponent<Image>().enabled = false;
            if (CheckForWin())
            {
                GameManager.Instance.Win();
                isGameOver = true;
            }
        });
    }

    // Check if all tiles have been matched (win condition)
    bool CheckForWin()
    {
        // Iterate over all tiles
        foreach (var tile in tiles)
        {
            // Check if the tile's image component is still enabled (meaning it's not yet matched/removed)
            if (tile.GetComponent<Image>().enabled)
            {
                return false; // If any tile is still visible, the game is not yet won
            }
        }

        return true; // All tiles have been matched and removed
    }
}