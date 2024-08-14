using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
public class GameConfig
{
}
public enum Direction
{
    TOP,
    DOWN,
    RIGHT = 3,
    LEFT = 2,
    FORWARD = 4,
    BACK
}
public enum Type_Tile
{
    Arrow,
    Rotate,
    Stone,
    Bomb,
    Saw_Blade,
}
public enum Type_Node
{
    None,
    Change_Direction,
    Traffic_Pole
}
public enum Type_Overlay
{
    None,
    Hidden,
    Freeze
}
public interface ITile
{
    public MapTile mapTile { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public Type_Tile Type { get; }
    public void OnTap(out bool canTap);
    public void OnMove1Tile(int _x, int _y);
    public void OnDestroyTile();
    public void SetMapTile(MapTile mapTile);
}


public static class InputAction
{
    public static Vector2 GetMousePosition()
    {
        return Input.mousePosition;
    }
    public static Vector2 ScreenToWorldVector2(Camera camera)
    {
        Vector3 p = camera.ScreenToWorldPoint(GetMousePosition());
        p.z = 0;
        return p;
    }
    static Collider2D[] cols;
    public static bool HaveNodeInPosition(this Vector2 position, LayerMask layerMaskNode, float radiusCheck, out Node node)
    {
        bool result = false;
        result = Physics2D.OverlapCircleNonAlloc(position, radiusCheck, cols, layerMaskNode) > 0;
        cols[0].TryGetComponent<Node>(out node);
        return result;
    }
}