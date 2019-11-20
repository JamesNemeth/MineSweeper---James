using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public GameObject tilePrefab;
    public int width = 10, height = 10;
    public float spacing = .155f;

    private Tile[,] tiles;

    Tile SpawnTile(Vector3 pos)
    {
        //Finds the tile prefab and clones the same prefab
        GameObject clone = Instantiate(tilePrefab);
        clone.transform.position = pos;
        return clone.GetComponent<Tile>();
    }
    void GenerateTiles()
    {
        //Make the tiles within a certain area
        //Sets the height and width of each tile
        //Also sets the spacing and distace apart of each tile
        tiles = new Tile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 halfSize = new Vector2(width * 0.5f, height * 0.5f);
                Vector2 pos = new Vector2(x - halfSize.x, y - halfSize.y);
                Vector2 offset = new Vector2(0.5f, 0.5f);
                pos += offset;
                pos *= spacing;

                Tile tile = SpawnTile(pos);
                tile.transform.SetParent(transform);
                tile.x = x;
                tile.y = y;
                tiles[x, y] = tile;
            }
        }
    }
    void Start()
    {
        //At the start of the game, the tiles are generated
        GenerateTiles();
    }
    public int GetAdjacentMineCount(Tile tile)
    {
        int count = 0;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                int desiredX = tile.x + x;
                int desiredY = tile.y + y;

                if(desiredX < 0 || desiredX >= width || desiredY < 0 || desiredY >= height)
                {
                    continue;
                }
                Tile currentTile = tiles[desiredX, desiredY];
                if (currentTile.isMine)
                {
                    count++;
                }
            }
        }
        return count;
    }
    void SelectATile()
    {
        //Allows you to select a tile on the screen
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouseRay.origin, mouseRay.direction);
        //If tile has been selected
        if (hit.collider != null)
        {
            //reveal a hit tile prefab
            Tile hitTile = hit.collider.GetComponent<Tile>();
            //If tile has been selected
            if (hitTile != null)
            {
                //Reveal the number tile prefab depending on how many mines surround the tile
                int adjacentMines = GetAdjacentMineCount(hitTile);
                hitTile.Reveal(adjacentMines);
            }
        }
    }
    void Update()
    {
        //left mouse click is pressed
        if (Input.GetMouseButtonDown(0))
        {
            //Select tile is referenced
            SelectATile();
        }   
    }
    void FFuncover(int x, int y, bool[,] visited)
    {
        //This allows the spanning out of the tiles to stop if the tile has 
        //no adjacent mine
        //this allows you to stop the whole tile map from being revealed
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            if (visited[x, y])
                return;

            Tile tile = tiles[x, y];
            int ajacentMines = GetAdjacentMineCount(tile);
            tile.Reveal(ajacentMines);

            if (ajacentMines == 0)
            {
                visited[x, y] = true;

                FFuncover(x - 1, y, visited);
                FFuncover(x + 1, y, visited);
                FFuncover(x, y - 1, visited);
                FFuncover(x, y + 1, visited);
            }
        }
    }
    void UncoverMines(int mineState = 0)
    {
        // Uncovers all mines in the grid
        for (int x = 0; x < width; x++)
        {
            // Loop through 2D array
            for (int y = 0; y < height; y++)
            {
                Tile tile = tiles[x, y];
                // Check if tile is a mine
                if (tile.isMine)
                {
                    //Reveal the tile
                    int adjacentMines = GetAdjacentMineCount(tile);
                    tile.Reveal(adjacentMines, mineState);
                }
            }
        }
    }
    bool NoMoreEmptyTiles()
    {
        // Set empty tile count to zero
        int emptyTileCount = 0;
        // Loop through 2D array
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = tiles[x, y];
                // If tile is NOT revealed AND NOT a mine
                if (!tile.isRevealed && !tile.isMine)
                {
                    // We found an empty tile!
                    emptyTileCount += 1;
                }
            }
        }
        // If there are empty tiles - return true
        // If there are no empty tiles - return false
        return emptyTileCount == 0;
    }
    void SelectTile(Tile selected)
    {
        int adjacentMines = GetAdjacentMineCount(selected);
        selected.Reveal(adjacentMines);
        // Is the selected tile a mine?
        if (selected.isMine)
        {
            // Uncover all mines - with default loss state '0'
            UncoverMines();
            // Lose
        }
        // If there are  no mine tiles around
        else if (adjacentMines == 0)
        {
            int x = selected.x;
            int y = selected.y;
            // Then use flood fill to uncover all adjacent mines
            FFuncover(x, y, new bool[width, height]);
        }
        // No more empty game tiles
        if (NoMoreEmptyTiles())
        {
            // Uncover all mines - with the win state '1'
            UncoverMines(1);
            //win
        }
    }
}
