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
    [SerializeField] SpriteRenderer render;
    [SerializeField, EnumToggleButtons] public Type_Node typeNode = Type_Node.None;
    [ShowIf(nameof(typeNode),Type_Node.Change_Direction), SerializeField, EnumToggleButtons, HideLabel]
    Direction direction;
    //[SerializeField] TMP_Text txtTitle;
    [Button("Set Visual")]
    public void SetVisual()
    {

    }
    //[SerializeField] TMP_Text txtTitle;
    public void UnVisual()
    {
        render.enabled = false;
        allowGUI = false;
    }
    private void OnEnable()
    {
        isLock = false;
        IsSelect = false;
        if (IsTrafficPole)
            map.onMoveTile += OnMoveTile;
    }
    private void OnDisable()
    {
        if (IsTrafficPole)
            map.onMoveTile -= OnMoveTile;
    }
    private void OnMoveTile(int x, int y)
    {
        isLock = !isLock;
    }
    #region PUBLIC METHOD
    bool isSelect = false;
    public bool IsSelect
    {
        get => isSelect;
        set
        {
            if (value)
                render.color = Color.magenta;
            else
                render.color = Color.cyan;
            isSelect = value;
        }
    }
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
        return typeNode==Type_Node.Change_Direction;
    }
    public Direction GetDirection() => direction;
    #endregion
    [ShowIf(nameof(typeNode), Type_Node.Traffic_Pole), SerializeField] bool isLock = false;
    public bool IsLock { get => isLock; set => isLock = value; }

    public bool IsTrafficPole => typeNode == Type_Node.Traffic_Pole;

    bool allowGUI = true;
    private void OnGUI()
    {
        if (!allowGUI) return;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

        // Draw the text on the screen
        GUI.Label(new Rect(screenPos.x - 20, Screen.height - screenPos.y - 20, 50, 20), $"({_x},{_y})");
    }
    //}
}
