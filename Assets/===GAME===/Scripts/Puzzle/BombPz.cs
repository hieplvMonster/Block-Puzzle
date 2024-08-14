using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombPz : TileBase
{
    public override Type_Tile Type => Type_Tile.Bomb;

    public override void OnDestroyTile()
    {
        // TODO
        // ANimation exlosion bomb
        CheckTileAround();
        for (int i = 0; i < tilesCheck.Count; i++)
        {
            var x = tilesCheck[i];
            x.OnDestroyTile();
        }
        base.OnDestroyTile();
    }

     TileBase cacheTile = null;
    public List<TileBase> tilesCheck;
    public override void SetMapTile(MapTile mapTile)
    {
        base.SetMapTile(mapTile);
    }
    [BoxGroup("Explode Tile"), Button("Explode"), GUIColor(1, .67f, 0)]
    public override void OnTap(out bool canTap)
    {
        // Show 
        canTap = true;
        mapTile.onMoveTile?.Invoke(x, y);
        OnDestroyTile();
    }
    [BoxGroup("Explode Tile"), Button("CheckTileAround")]

    public void CheckTileAround()
    {
        tilesCheck = new List<TileBase>();
        for (int i = 0; i < mapTile.tiles.Count; i++)
        {
            cacheTile = mapTile.tiles[i];
            if (Mathf.Abs(cacheTile.X - x) <= 1 && Mathf.Abs(cacheTile.Y - y) <= 1 && cacheTile != this)
            {
                tilesCheck.Add(cacheTile);
            }
        }
    }
    [Button("Set Visual Tile")]
    public override void SetVisual()
    {
        mainSprite.GetComponent<SpriteRenderer>().sprite = TilePreset.Instance.GetAssetBomb();
    }

}
