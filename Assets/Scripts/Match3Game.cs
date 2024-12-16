using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Match3Game : Game
{
    public GameObject tilePrefab;
    public Transform gridParent;

    private List<GameObject> tiles;
    private GameObject firstSelectedTile = null;
    private bool isChecking = false;
    private GridLayoutGroup gridLayout;

    public TMP_Text moneyText;
    public int amountToWin;
    private int money;
    

    public override void Begin(Visual visual)
    {
        base.Begin(visual);
        money = 0;
        gridLayout = gridParent.GetComponent<GridLayoutGroup>();
        SetGridLayout(); // Set up the grid layout
        CreateGrid();
        CheckForMatches();
        amountToWin = GameManager.Instance.currentLevelData.moneyWin;   
        moneyText.text = $"0/{amountToWin}";
    }

    public override void End()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            Destroy(tiles[i]);
        }
    }

    // Set the grid layout to adjust the cell size, padding, and spacing dynamically
    void SetGridLayout()
    {
        float parentWidth = gridParent.GetComponent<RectTransform>().rect.width;
        float parentHeight = gridParent.GetComponent<RectTransform>().rect.height;

        // Calculate the total spacing between tiles
        float totalSpacingWidth = spacing * (gridSize - 1); // Total horizontal spacing
        float totalSpacingHeight = spacing * (gridSize - 1); // Total vertical spacing

        // Calculate the available width and height by subtracting padding and spacing
        float availableWidth = parentWidth - (padding * 2) - totalSpacingWidth;
        float availableHeight = parentHeight - (padding * 2) - totalSpacingHeight;

        // Calculate cell size based on available space and grid size
        float cellSize = Mathf.Min(availableWidth / gridSize, availableHeight / gridSize);

        // Set GridLayoutGroup's padding and spacing
        gridLayout.padding.left = (int)padding;
        gridLayout.padding.right = (int)padding;
        gridLayout.padding.top = (int)padding;
        gridLayout.padding.bottom = (int)padding;

        gridLayout.spacing = new Vector2(spacing, spacing);
        gridLayout.cellSize = new Vector2(cellSize, cellSize);
    }

    void CreateGrid()
    {
        tiles = new List<GameObject>();

        int totalTiles = gridSize * gridSize;
        for (int i = 0; i < totalTiles; i++)
        {
            GameObject tile = Instantiate(tilePrefab, gridParent);
            tile.GetComponent<Button>().onClick.AddListener(() => OnTileClick(tile));
            tile.name = i.ToString();
            tile.transform.GetChild(0).GetComponent<Image>().sprite = tileSprites[Random.Range(0, tileSprites.Count)];
            tiles.Add(tile);
        }
    }

    void OnTileClick(GameObject clickedTile)
    {
        if (isChecking) return;

        if (firstSelectedTile == null)
        {
            firstSelectedTile = clickedTile;
        }
        else
        {
            if (firstSelectedTile == clickedTile)
            {
                firstSelectedTile = null;
                return;
            }
            SwapTiles(firstSelectedTile, clickedTile, true);
            firstSelectedTile = null;
        }
    }

    void SwapTiles(GameObject tileA, GameObject tileB, bool animate)
    {
        if (animate)
        {
            isChecking = true;
            gridLayout.enabled = false;

            RectTransform rectA = tileA.GetComponent<RectTransform>();
            RectTransform rectB = tileB.GetComponent<RectTransform>();

            Vector3 posA = rectA.position;
            Vector3 posB = rectB.position;

            rectA.DOMove(posB, 0.5f);
            rectB.DOMove(posA, 0.5f).OnComplete(() =>
            {
                int indexA = tileA.transform.GetSiblingIndex();
                int indexB = tileB.transform.GetSiblingIndex();

                tileA.transform.SetSiblingIndex(indexB);
                tileB.transform.SetSiblingIndex(indexA);

                tiles[indexA] = tileB;
                tiles[indexB] = tileA;

                gridLayout.enabled = true;

                CheckForMatches();
                isChecking = false;
            });
        }
        else
        {
            int indexA = tiles.IndexOf(tileA);
            int indexB = tiles.IndexOf(tileB);
        }
    }

    void ShuffleGrid()
    {
        int totalTiles = gridSize * gridSize;
        for (int i = 0; i < 100; i++)
        {
            int randomTileIndex = Random.Range(0, totalTiles);
            int emptyTileIndex = tiles.FindIndex(t => !t.GetComponent<Button>().interactable);
            if (emptyTileIndex == -1) emptyTileIndex = Random.Range(0, totalTiles);
            SwapTiles(tiles[emptyTileIndex], tiles[randomTileIndex], false);
        }
    }

    void CheckForMatches()
    {
        HashSet<int> tilesToRemove = new HashSet<int>();

        // Horizontal match check
        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col <= gridSize - 3; col++)
            {
                int index = row * gridSize + col;
                if (AreSpritesEqual(tiles[index], tiles[index + 1], tiles[index + 2]))
                {
                    tilesToRemove.Add(index);
                    tilesToRemove.Add(index + 1);
                    tilesToRemove.Add(index + 2);
                }
            }
        }

        // Vertical match check
        for (int col = 0; col < gridSize; col++)
        {
            for (int row = 0; row <= gridSize - 3; row++)
            {
                int index = row * gridSize + col;
                if (AreSpritesEqual(tiles[index], tiles[index + gridSize], tiles[index + 2 * gridSize]))
                {
                    tilesToRemove.Add(index);
                    tilesToRemove.Add(index + gridSize);
                    tilesToRemove.Add(index + 2 * gridSize);
                }
            }
        }

        if (tilesToRemove.Count > 0)
        {
            foreach (int index in tilesToRemove)
            {
                RemoveTile(tiles[index]);
            }
            Invoke("RefillGrid", 0.6f);
        }
        else
        {
            isChecking = false;
        }
    }

    bool AreSpritesEqual(GameObject tile1, GameObject tile2, GameObject tile3)
    {
        Sprite sprite1 = tile1.transform.GetChild(0).GetComponent<Image>().sprite;
        Sprite sprite2 = tile2.transform.GetChild(0).GetComponent<Image>().sprite;
        Sprite sprite3 = tile3.transform.GetChild(0).GetComponent<Image>().sprite;

        return sprite1 == sprite2 && sprite2 == sprite3;
    }

    void RemoveTile(GameObject tile)
    {
        tile.transform.DOScale(0, 0.5f).OnComplete(() => {
            tile.GetComponent<Button>().interactable = false;
            tile.transform.GetChild(0).GetComponent<Image>().enabled = false;
            money += 25;
            moneyText.text = $"{money}/{amountToWin}";
            if (money >= amountToWin)
            {
                GameManager.Instance.Win();
            }
        });
    }

    void RefillGrid()
    {
        int totalTiles = gridSize * gridSize;
        for (int i = 0; i < totalTiles; i++)
        {
            if (!tiles[i].GetComponent<Button>().interactable)
            {
                tiles[i].transform.localScale = Vector3.one;
                tiles[i].GetComponent<Button>().interactable = true;
                tiles[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                tiles[i].transform.GetChild(0).GetComponent<Image>().sprite = tileSprites[Random.Range(0, tileSprites.Count)];
            }
        }
        isChecking = false;
        CheckForMatches();
    }
}
