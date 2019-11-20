using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour
{
    public int x, y;
    public bool isMine = false;
    public bool isRevealed = false;
    [Header("References")]
    public Sprite[] emptySprites;
    public Sprite[] mineSprites;
    private SpriteRenderer rend;

    public void Awake()
    {
        // Grab reference to sprite renderer
        rend = GetComponent<SpriteRenderer>();
    }
    public void Start()
    {
        isMine = Random.value < .05f;
    }
    public void Reveal (int adjacentMines, int mineState = 0)
    {
        //Flags the tile as being revealed
        isRevealed = true;
        //Checks if tile is a mine
        if(isMine)
        {
            // Sets sprite to appropriate texture based on adjacent mines
            rend.sprite = mineSprites[mineState];
        }
        else
        {
            rend.sprite = emptySprites[adjacentMines];
        }
    }
}

