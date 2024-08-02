using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileBase : MonoBehaviour, ITile
{
    public int x, y;
    [SerializeField] MapTile _maptile;
    public MapTile mapTile { get => _maptile; set => _maptile = value; }
    public int X { get => x; set => x = value; }
    public int Y { get => y; set => y = value; }

    public virtual Type_Tile Type { get; }


    [SerializeField, EnumToggleButtons] Type_Overlay type_Overlay = Type_Overlay.None;
    public Type_Overlay TypeOverlay => type_Overlay;

    [SerializeField] protected GameObject mainSprite = default;

    [SerializeField] protected GameObject hiddenSprite = default;
    protected bool isHidden = true;
    [SerializeField] protected GameObject freezeSprite = default;
    [SerializeField, ShowIf(nameof(TypeOverlay), Type_Overlay.Freeze)]
    int numberFreeze;
    [SerializeField, ShowIf(nameof(TypeOverlay), Type_Overlay.Freeze)]
    TMP_Text txtFreeze;
    protected int _currentNumFreeze;
    protected virtual void Awake()
    {
        if (type_Overlay == Type_Overlay.Freeze)
        {
            _currentNumFreeze = numberFreeze;
            freezeSprite.SetActive(true);
            SetupFreezeBlock();
        }
        else if (type_Overlay == Type_Overlay.Hidden)
            hiddenSprite.SetActive(true);
        else
        {
            hiddenSprite.SetActive(false);
            freezeSprite.SetActive(false);
        }
    }
    public virtual void SetVisual()
    {

    }
    void SetupFreezeBlock()
    {
        txtFreeze.gameObject.SetActive(_currentNumFreeze > 0);
        txtFreeze.text = _currentNumFreeze.ToString();
    }

    void OnDestroyFreeze()
    {
        // TODO: Animation break freeze
        freezeSprite.SetActive(false);
        type_Overlay = Type_Overlay.None;
    }
    public void UnHidden()
    {
        // TODO : animation break hidden
        hiddenSprite.SetActive(false);
        isHidden = false;
        type_Overlay = Type_Overlay.None;
    }
    public virtual void OnDestroyTile()
    {
        if (type_Overlay == Type_Overlay.Freeze)
        {
            if (_currentNumFreeze > 0)
                OnDestroyFreeze();
        }
        else if (type_Overlay == Type_Overlay.Hidden)
        {
            if (isHidden)
                UnHidden();
        }
        else
        {
            // TODO: animation break tile
            mapTile.RemoveTile(this);
            gameObject.SetActive(false);
            DestroyImmediate(gameObject);
        }
    }

    public virtual void OnMove1Tile(int _x, int _y)
    {
        if (type_Overlay == Type_Overlay.Freeze)
        {
            if (_currentNumFreeze > 0)
                _currentNumFreeze--;
            SetupFreezeBlock();
            if (_currentNumFreeze <= 0)
            {
                OnDestroyFreeze();
            }
        }
        else if (type_Overlay == Type_Overlay.Hidden)
        {
            if (Mathf.Abs(_x - x) + Mathf.Abs(_y - y) == 1) // check pos tile is next to of hidden?
            {
                UnHidden();
            }
        }
    }

    public virtual void OnTap()
    {
        if (type_Overlay == Type_Overlay.Hidden)
        {
            if (isHidden)
            {
                // TODO: animation on tap hidden
                return;
            }
        }
        else if (type_Overlay == Type_Overlay.Freeze)
        {
            if (_currentNumFreeze > 0)
            {
                // TODO: animation on tap freeze
                return;
            }
        }
    }

    public virtual void SetMapTile(MapTile mapTile)
    {
        _maptile = mapTile;
    }
}
