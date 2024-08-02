using Sirenix.OdinInspector;
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

    [SerializeField] bool isChangeDirection = false;

    [ShowIf(nameof(isChangeDirection)), SerializeField, EnumToggleButtons, HideLabel]
    Direction direction;
    //[SerializeField] TMP_Text txtTitle;

    #region PUBLIC METHOD
    public MapTile GetMapTile() => map;
    public void Setup(int x, int y, MapTile map)
    {
        _x = x;
        _y = y;
        this.map = map;
        //txtTitle.text = $"{_x},{_y}";
    }
    [SerializeField] TileBase tile;
    public void SetTile(TileBase tile)
    {
        this.tile = tile;
        //if (tile == null) return;
        //tile.X = X;
        //tile.X = Y;
    }
    public bool HaveTile => tile != null;
    public TileBase GetTile() => tile;
    public bool IsNodeChangeDirection()
    {
        return isChangeDirection;
    }
    public Direction GetDirection() => direction;
    #endregion

    private void OnGUI()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

        // Draw the text on the screen
        GUI.Label(new Rect(screenPos.x - 20, Screen.height - screenPos.y - 20, 50, 20), $"({_x},{_y})");
    }
    //}
}
