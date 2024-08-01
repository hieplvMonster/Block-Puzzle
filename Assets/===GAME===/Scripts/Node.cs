using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
[System.Serializable]
public class Node : MonoBehaviour
{
    [SerializeField] int _x, _y;
    public int X => _x;
    public int Y => _y;

    public MapTile map;
    //[SerializeField] TMP_Text txtTitle;
    public MapTile GetMapTile() => map;
    public void Setup(int x, int y, MapTile map)
    {
        _x = x;
        _y = y;
        this.map = map;
        //txtTitle.text = $"{_x},{_y}";
    }
    [SerializeField] TilePz tile;
    public void SetTile(TilePz tile)
    {
        this.tile = tile;
        if (tile == null) return;
        tile.x = X;
        tile.y = _y;
    }
    public bool HaveTile => transform.childCount > 0;
    public TilePz GetTile() => tile;

    ITile iTile;
    public void SetTile(ITile itile)
    {
        iTile = itile;
        itile.X = X;
        itile.Y = Y;
    }
    private void OnGUI()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

        // Draw the text on the screen
        GUI.Label(new Rect(screenPos.x - 20, Screen.height - screenPos.y - 20, 50, 20), $"({_x},{_y})");
    }
    //}
}
