using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public List<Sprite> tileSprites;
    
    public int gridSize = 4; // Default is 4x4
    public float padding = 15f; // Padding for left, right, top, bottom
    public float spacing = 10f; // Spacing between tiles

    public virtual void Begin(Visual visual)
    {
        tileSprites = visual.visuals;
    }

    public virtual void End()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
