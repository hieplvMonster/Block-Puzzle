using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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
    Trap,
    Rotate,
    Stone,
    Bomb,
    Change_Direction,
    Saw_Blade
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
    public void OnTap();
    public void OnMove1Tile(int _x, int _y);
    public void OnDestroyTile();
    public void SetMapTile(MapTile mapTile);
}
