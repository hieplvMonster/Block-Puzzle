using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TurningPz : TileBase
{
    public override Type_Tile Type => Type_Tile.Rotate;
    protected override void Awake()
    {
    }
    public override void OnTap(out bool canTap)
    {
        canTap = true;
        mapTile.onMoveTile?.Invoke(x, y);
        Rotate();
    }
    public override void SetMapTile(MapTile mapTile)
    {
        base.SetMapTile(mapTile);
    }
    [Button("Set Visual Tile")]
    public override void SetVisual()
    {
    }
    public override void OnDestroyTile()
    {
        base.OnDestroyTile();
    }
    [/*BoxGroup("Rotate Around"), */SerializeField] List<TileBase> linkTiles = new List<TileBase>();
    [/*BoxGroup("Rotate Around"), */SerializeField] public float duration;
    //[BoxGroup("Rotate Around"), Button("Link Tile")]
    public void LinkTile(TileBase tile)
    {
        linkTiles.Add(tile);
    }
    public void UnlinkChild(TileBase tile)
    {
        linkTiles.Remove(tile);
    }
    [Button("Turning")]
    public void Rotate(bool isClockWise = true)
    {
        float val = isClockWise ? -1 : 1;
        if (linkTiles.Count == 0) return;
        float angle = 0;
        for (int i = 1; i < 4; i++)
        {
            int _count = 0;
            foreach (var x in linkTiles)
            {
                if (x.CheckAvailablePos(val * i * 90))
                    _count++;
            }
            if (_count == linkTiles.Count)
            {
                angle = val * i * 90;
                break;
            }
        }
        if (angle != 0)
        {
            foreach (var x in linkTiles)
                x.RotateTile(angle);
        }
        else
        {
            // TODO: animate if not rotate
        }
        //for (int i=0;i<linkTiles.Count;i++)
        //{
        //    var tile = linkTiles[i];
        //    linkTiles[i].CheckTargetNodeRotate(isClockWise, out float angle);
        //    if(angle!=_angle)
        //        Debug.Log("Not")
        //}
        // Animation node after duration->normal
    }
    List<float> angles = new List<float>();
    void GetRotateAngles(bool isClockWise)
    {
        angles.Clear();
        foreach (var x in linkTiles)
        {
            x.CheckTargetNodeRotate(isClockWise, out float a);
            angles.Add(a);
        }
    }
}

