using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBladePz : TileBase
{
    [ShowInInspector]
    public override Type_Tile Type { get => Type_Tile.Saw_Blade; }
    protected override void Awake()
    {
        // TODO: animation rotate
    }
    public override void OnTap(out bool canTap)
    {
        canTap = false;
    }
    public override void SetMapTile(MapTile mapTile)
    {
        base.SetMapTile(mapTile);
    }
    [Button("Set Visual Tile")]
    public override void SetVisual()
    {
        mainSprite.GetComponent<SpriteRenderer>().sprite = TilePreset.Instance.GetAssetSawBlade();
    }
    public override void OnDestroyTile()
    {
        base.OnDestroyTile();
    }
}
