using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TilePz : MonoBehaviour
{
    [SerializeField] MapTile mapTile;
    [EnumToggleButtons] public Type_Tile type = Type_Tile.Arrow;
    [EnableIf(nameof(type), Type_Tile.Arrow), EnumToggleButtons, SerializeField, HideLabel]
    Direction direction = Direction.TOP;

    public void SetMapTile(MapTile mapTile)
    {
        this.mapTile = mapTile;
    }

    private void Awake()
    {
        _brokeVal = brokenValue;
        SetupFreezeBlock();
        SetupHiddenBlock(true);
        //this.mapTile.onRemove1Tile += OnMove1Tile;
        mapTile.onMoveTile += OnMove1Tile;
    }
    private void OnDisable()
    {
        mapTile.onMoveTile -= OnMove1Tile;
        //this.mapTile.onRemove1Tile -= OnMove1Tile;

    }

    [BoxGroup] public int x, y;

    Node targetFind = null;

    #region Button Inspector
    [Button("Set Visual Tile")]
    public void SetVisual()
    {
        GetComponentInChildren<SpriteRenderer>().sprite = TilePreset.Instance.GetAssetArrow(direction);
    }
    [BoxGroup("Move Tile"), Button("Find target node", ButtonSizes.Medium)]
    public void FindNode()
    {
        resultX = x; resultY = y;
        if (!FindtargetNode(out targetFind))
        {
            if (targetFind == null)
            {
                Debug.LogError("STAY!");
            }
            else
                Debug.LogError("Find " + targetFind.name);
        }
        else
        {
            Debug.LogError("Out");
        }
    }
    #endregion

    int resultX, resultY, nextX, nextY;
    bool FindtargetNode(out Node target)
    {
        nextX = resultX;
        nextY = resultY;

        switch (direction)
        {
            case Direction.LEFT:
                nextX--;
                break;
            case Direction.RIGHT:
                nextX++;
                break;
            case Direction.TOP:
                nextY++;
                break;
            case Direction.DOWN:
                nextY--;
                break;
        }
        if (nextX < 0 || nextX >= mapTile.totalX || nextY < 0 || nextY >= mapTile.totalY)
        {
            switch (direction)
            {
                case Direction.LEFT:
                    nextX++;
                    break;
                case Direction.RIGHT:
                    nextX--;
                    break;
                case Direction.TOP:
                    nextY--;
                    break;
                case Direction.DOWN:
                    nextY++;
                    break;
            }
            target = mapTile.nodes[nextX, nextY];
            return true;
        }
        if (mapTile.nodes[nextX, nextY].HaveTile)
        {
            switch (direction)
            {
                case Direction.LEFT:
                    nextX++;
                    break;
                case Direction.RIGHT:
                    nextX--;
                    break;
                case Direction.TOP:
                    nextY--;
                    break;
                case Direction.DOWN:
                    nextY++;
                    break;
            }
            if (nextX == x && nextY == y)
                target = null;
            else
                target = mapTile.nodes[nextX, nextY];
            return false;
        }
        else
        {
            resultX = nextX;
            resultY = nextY;
            target = mapTile.nodes[nextX, nextY];
            return FindtargetNode(out targetFind);
        }
    }
    bool canTap = true;
    Action OnCompleteTap = null;
    public void TapPuzzle(Action onComplete = null)
    {
        canTap = false;
        OnCompleteTap = onComplete;
        // Do something with type
        switch (type)
        {
            case Type_Tile.Arrow:
                MoveTile();
                break;
        }
    }
    TilePz cacheTile = null;
    #region STONE BLOCK
    void SetupStoneBlock()
    {
        if (type != Type_Tile.Stone) return;

    }
    #endregion
    #region ARROW ACTION
    [BoxGroup("Move Tile"), Button("Move"), GUIColor(0, 1, .03f)]
    public void MoveTile()
    {
        mapTile.onMoveTile?.Invoke(x, y);
        if (type != Type_Tile.Arrow) return;
        resultX = x; resultY = y;
        if (!FindtargetNode(out targetFind))
        {
            if (targetFind == null)
            {
                Debug.LogError("STAY!");
            }
            else
            {
                Debug.LogError("Find " + targetFind.name);
                transform.DOMove(targetFind.transform.position, .2f)
                    .OnComplete(() =>
                    {
                        canTap = true;
                        OnCompleteTap?.Invoke();
                        mapTile.nodes[x, y].SetTile(tile: null);
                        targetFind.SetTile(this);
                        transform.SetParent(targetFind.transform);
                    });
            }
        }
        else
        {
            Vector3 target = targetFind.transform.position;
            switch (direction)
            {
                case Direction.LEFT:
                    target += Vector3.left * 10;
                    break;
                case Direction.RIGHT:
                    target += Vector3.right * 10;
                    break;
                case Direction.TOP:
                    target += Vector3.up * 10;
                    break;
                case Direction.DOWN:
                    target += Vector3.down * 10;
                    break;
            }
            transform.parent = null;
            transform.DOMove(target, .2f)
                .OnComplete(() =>
                {
                    canTap = true;
                    OnCompleteTap?.Invoke();
                    //Debug.Log(gameObject.name);
                    gameObject.SetActive(false);
                    mapTile.RemoveTile(this);
                });
            Debug.LogError("Out");
        }
        //// Check target node
        //if (!FindtargetNode(out Node tar))
        //{

        //}
    }
    #endregion

    #region BOMB ACTION
    [BoxGroup("Explode Tile"), Button("Explode"), GUIColor(1, .67f, 0)]
    public void Explode()
    {
        if (type != Type_Tile.Bomb) return;
        CheckTileAround();
        for (int i = 0; i < tilesCheck.Count; i++)
        {
            var x = tilesCheck[i];
            Debug.Log($"Node {x.x}-{x.y} = {x}====={x.type}");
            if (x.type == Type_Tile.Freeze)
            {
                x.OnDestroyFreeze();
            }
            else
            {
                x.gameObject.SetActive(false);
                x.transform.parent = null;
                mapTile.RemoveTile(x);
            }
        }
        OnCompleteTap?.Invoke();
    }
    List<TilePz> tilesCheck;
    [BoxGroup("Explode Tile"), Button("CheckTileAround")]
    public void CheckTileAround()
    {
        tilesCheck = new List<TilePz>();
        for (int i = 0; i < mapTile.tiles.Count; i++)
        {
            cacheTile = mapTile.tiles[i];
            if (Mathf.Abs(cacheTile.x - x) <= 1 && Mathf.Abs(cacheTile.y - y) <= 1)
            {
                tilesCheck.Add(cacheTile);
            }
        }
    }
    #endregion

    #region FREEZE BLOCK

    [SerializeField, ShowIf(nameof(type), Type_Tile.Freeze)] int brokenValue = 1;
    [SerializeField, ShowIf(nameof(type), Type_Tile.Freeze)] GameObject overlayObjFreeze;
    [SerializeField, ShowIf(nameof(type), Type_Tile.Freeze)] TMP_Text txtTurn;
    [SerializeField, ShowIf(nameof(type), Type_Tile.Freeze)] Type_Tile typeAfterBreakFreeze;
    private int _brokeVal;
    public void SetupFreezeBlock()
    {
        if (type != Type_Tile.Freeze) return;
        overlayObjFreeze.SetActive(_brokeVal > 0);
        txtTurn.text = _brokeVal.ToString();
    }

    public void OnDestroyFreeze()
    {
        // TODO: Vfx
        type = typeAfterBreakFreeze;
        OnCompleteTap?.Invoke();
        SetVisual();
    }
    public int GetCurrentTurnBroke() => _brokeVal;
    #endregion

    #region HIDDEN BLOCK
    [SerializeField, ShowIf(nameof(type), Type_Tile.Hidden)] GameObject overlayObjHidden;
    [SerializeField, ShowIf(nameof(type), Type_Tile.Hidden)] Type_Tile typeAfterBreakHidden;
    public void SetupHiddenBlock(bool isShow)
    {
        if (type != Type_Tile.Hidden) return;
        overlayObjHidden.SetActive(isShow);
    }
    #endregion

    private void OnMove1Tile(int _x, int _y)
    {
        canTap = true;
        if (type == Type_Tile.Freeze)
        {
            if (_brokeVal > 0)
                _brokeVal--;
            SetupFreezeBlock();
            if (_brokeVal <= 0)
            {
                OnDestroyFreeze();
            }
        }

        if (type == Type_Tile.Hidden)
        {

            if (Mathf.Abs(_x - x) + Mathf.Abs(_y - y) == 1) // check pos tile is next to of hidden?
            {
                SetupHiddenBlock(false);
                OnCompleteTap?.Invoke();
                type = typeAfterBreakHidden;
            }
        }
    }
}
