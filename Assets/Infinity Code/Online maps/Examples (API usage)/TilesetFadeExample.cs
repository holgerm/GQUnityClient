/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TilesetFadeExample : MonoBehaviour
{
    // List of items that are animated.
    private List<TilesetFadeExampleItem> items = new List<TilesetFadeExampleItem>();

	void Start () 
    {
        // Subscribe to change material event.
	    (OnlineMapsTileSetControl.instance as OnlineMapsTileSetControl).OnChangeMaterialTexture += OnChangeMaterialTexture;
	}

    // This event called when tile texture changed.
    private void OnChangeMaterialTexture(OnlineMapsTile tile, Material material)
    {
        // Try get tile from list
        TilesetFadeExampleItem item = items.FirstOrDefault(i => i.tile == tile);

        if (item != null)
        {
            // If tile exist then update
            item.material = material;
            item.Update();
        }
        else if (tile.customData != null)
        {
            // If tile animator exist then restore animator and update
            item = (TilesetFadeExampleItem) tile.customData;
            if (!item.finished)
            {
                items.Add(item);
                item.Update();
            }
            else material.color = TilesetFadeExampleItem.toColor;
        }
        else if (tile.texture != null && tile.texture != OnlineMaps.instance.defaultTileTexture)
        {
            // If tile texture is not null then create new animator
            item = new TilesetFadeExampleItem(tile, material);
            item.Update();
            items.Add(item);
        }
        else
        {
            material.color = TilesetFadeExampleItem.fromColor;
        }
    }

    void Update ()
    {
        // Remove all disposed tiles
        items.RemoveAll(i => i.disposed);

        // Update alpha for tiles
        foreach (TilesetFadeExampleItem item in items) item.Update();

        // Remove all finished tiles
        items.RemoveAll(i => i.finished);
    }
}

// Class for instance of tile item
public class TilesetFadeExampleItem
{
    // Time of fade
    private const float totalTicks = 10000000;

    // Fade from color
    public static Color fromColor = new Color(1, 1, 1, 0);

    // Fade to color
    public static Color toColor = new Color(1, 1, 1, 1);

    // Animation finished
    public bool finished = false;

    // Tile material
    public Material material;

    // Reference to tile
    public OnlineMapsTile tile;

    // Tile alpha
    private float alpha = 0;

    // Time of start animation
    private long startTicks;

    // Check tile disposed
    public bool disposed
    {
        get { return tile.status == OnlineMapsTileStatus.disposed || tile.status == OnlineMapsTileStatus.error; }
    }

    public TilesetFadeExampleItem(OnlineMapsTile tile, Material material)
    {
        this.tile = tile;
        this.material = material;

        startTicks = DateTime.Now.Ticks;

        tile.customData = this;
    }

    // Update tile fade value
    public void Update()
    {
        alpha = (DateTime.Now.Ticks - startTicks) / totalTicks;

        if (alpha >= 1)
        {
            alpha = 1;
            finished = true;
        }

        material.color = Color.Lerp(fromColor, toColor, alpha);
    }
}