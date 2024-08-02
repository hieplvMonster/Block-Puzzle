using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonePz : TileBase
{
    public override Type_Tile Type { get => Type_Tile.Stone; }

    protected override void Awake()
    {
    }
    public override void OnTap()
    {

    }
    public override void SetMapTile(MapTile mapTile)
    {
        base.SetMapTile(mapTile);
    }
    [Button("Set Visual Tile")]
    public override void SetVisual()
    {
        mainSprite.GetComponent<SpriteRenderer>().sprite = TilePreset.Instance.GetAssetStone();
    }
    public override void OnDestroyTile()
    {
        // TODO: 
        // Animation break stone
        base.OnDestroyTile();
    }
}
