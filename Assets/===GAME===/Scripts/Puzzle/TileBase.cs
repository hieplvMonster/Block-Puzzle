using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
            UnLink();
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

    public virtual void OnTap(out bool canTap)
    {
        canTap = false;
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
    [SerializeField] bool canRotate = false;
    [ShowIf(nameof(canRotate)), BoxGroup("Rotate Around"), SerializeField] float _duration;
    [ShowIf(nameof(canRotate)), BoxGroup("Rotate Around"), SerializeField] AnimationCurve curve;
    [ShowIf(nameof(canRotate)), BoxGroup("Rotate Around"), SerializeField] TurningPz rootRotate = default;
    [ShowIf(nameof(canRotate)), BoxGroup("Rotate Around"), SerializeField] LineRenderer lineRender = default;

    public virtual void RotateAroundNode(bool isClockwise, Action onComplete = null)
    {
        if (rootRotate == null) return;
        RotateAroundRoot(isClockwise);
    }
    [ShowIf(nameof(canRotate)), BoxGroup("Rotate Around"), Button("Link Tile")]
    private void LinkRoot(TurningPz root)
    {
        rootRotate = root;
        rootRotate.LinkTile(this);
        _duration = root.duration;
        ang = rootAng;
        lineRender.gameObject.SetActive(true);
        RenderLine();
    }
    void RenderLine()
    {
        lineRender.positionCount = 2;
        lineRender.SetPosition(0, transform.position);
        lineRender.SetPosition(1, rootRotate.transform.position);
    }
    public void UnLink()
    {
        if (!canRotate) return;
        lineRender.gameObject.SetActive(false);
        rootRotate.UnlinkChild(this);
    }
    Coroutine coRot = null;
    [ShowIf(nameof(canRotate)), BoxGroup("Rotate Around"), Button("Rotation")]
    public void RotateAroundRoot(bool isClockwise)
    {
        // Check target tile
        target = CheckTargetNodeRotate(isClockwise, out float angleRot);
        if (angleRot != 0)
        {
            Debug.LogError($"OUT: {angleRot}");
            if (coRot == null)
                coRot = StartCoroutine(IRotateAround(rootRotate.transform, angleRot, _duration, () =>
                {
                    mapTile.nodes[X, Y].SetTile(null);
                    X = (int)target.x;
                    Y = (int)target.y;
                    if (mapTile.nodes[X, Y].GetTile())
                    {
                        if (mapTile.nodes[X, Y].GetTile().Type == Type_Tile.Saw_Blade)
                        {
                            OnDestroyTile();
                            return;
                        }
                        if (Type == Type_Tile.Saw_Blade)
                        {
                            mapTile.nodes[X, Y].GetTile().OnDestroyTile();
                        }
                    }
                    mapTile.nodes[X, Y].SetTile(this);
                    transform.parent = mapTile.nodes[X, Y].transform;
                }));
        }
    }

    public void RotateTile(float angle)
    {
        if (angle != 0)
        {
            if (coRot == null)
                coRot = StartCoroutine(IRotateAround(rootRotate.transform, angle, _duration, () =>
                {
                    mapTile.nodes[X, Y].SetTile(null);
                    X = (int)target.x;
                    Y = (int)target.y;
                    if (mapTile.nodes[X, Y].GetTile())
                    {
                        if (mapTile.nodes[X, Y].GetTile().Type == Type_Tile.Saw_Blade)
                        {
                            OnDestroyTile();
                            return;
                        }
                        if (Type == Type_Tile.Saw_Blade)
                        {
                            mapTile.nodes[X, Y].GetTile().OnDestroyTile();
                        }
                    }
                    mapTile.nodes[X, Y].SetTile(this);
                    transform.parent = mapTile.nodes[X, Y].transform;
                }));
        }
    }
    Vector2 target;
    float time = 0;
    float _angle;
    IEnumerator IRotateAround(Transform root, float angle, float duration, Action onComplete = null)
    {
        time = 0;
        while (time <= duration)
        {
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
            _angle = angle * curve.Evaluate(time / duration);
            SetPosByRadAngle((_angle/*rootAng*/ + ang) * Mathf.Deg2Rad); ;
        }
        SetPosByRadAngle((angle  /*rootAng*/ + ang) * Mathf.Deg2Rad);
        ang = ang + angle /*+ rootAng*/;
        onComplete?.Invoke();
        coRot = null;
    }
    float rootAng
    {
        get
        {
            int _x = X - rootRotate.X;
            int _y = Y - rootRotate.Y;
            if (_x > 0) return 0;
            else if (_x < 0) return 180;
            else if (_y > 0) return 90;
            else return 270;
        }
    }
    [SerializeField] float ang;
    float distance
    {
        get
        {
            float dis = Vector2.Distance(new Vector2(X, Y), new Vector2(rootRotate.X, rootRotate.Y));
            return 1.5f * dis;
        }
    }
    //[ShowInInspector]
    //public float rootAng
    //{
    //    get
    //    {
    //        return Mathf.Atan2(Y - rootRotate.Y, X - rootRotate.X) * Mathf.Rad2Deg;
    //    }
    //}
    void SetPosByRadAngle(float radAngle)
    {
        Vector3 offset = rootRotate.transform.position;
        float x = offset.x + distance * Mathf.Cos(radAngle);
        float y = offset.y + distance * Mathf.Sin(radAngle);
        float z = offset.z;
        transform.position = new Vector3(x, y, z);
        RenderLine();
    }
    [Button("Check")]
    public bool CheckAvailablePos(float angle)
    {
        if (this.IsCorrectCoordinates(rootRotate.X + (int)v1.x, rootRotate.Y + (int)v1.y))
        {
            if ((angle == -270 && mapTile.nodes[rootRotate.X + (int)v1.x, rootRotate.Y + (int)v1.y].IsNodeAvailableToMove(this))
                || (angle == 90 && mapTile.nodes[rootRotate.X + (int)v1.x, rootRotate.Y + (int)v1.y].IsNodeAvailableToMove(this)))
            {
                target = new Vector2(rootRotate.X + (int)v1.x, rootRotate.Y + (int)v1.y);
                return true;
            }
        }
        if (this.IsCorrectCoordinates(rootRotate.X + (int)v2.x, rootRotate.Y + (int)v2.y))
        {
            if ((angle == -180 && mapTile.nodes[rootRotate.X + (int)v2.x, rootRotate.Y + (int)v2.y].IsNodeAvailableToMove(this))
                || (angle == 180 && mapTile.nodes[rootRotate.X + (int)v2.x, rootRotate.Y + (int)v2.y].IsNodeAvailableToMove(this)))
            {
                target = new Vector2(rootRotate.X + (int)v2.x, rootRotate.Y + (int)v2.y);
                return true;
            }
        }
        if (this.IsCorrectCoordinates(rootRotate.X + (int)v3.x, rootRotate.Y + (int)v3.y))
        {
            if ((angle == -90 && mapTile.nodes[rootRotate.X + (int)v3.x, rootRotate.Y + (int)v3.y].IsNodeAvailableToMove(this))
                || (angle == 270 && mapTile.nodes[rootRotate.X + (int)v3.x, rootRotate.Y + (int)v3.y].IsNodeAvailableToMove(this)))
            {
                target = new Vector2(rootRotate.X + (int)v3.x, rootRotate.Y + (int)v3.y);
                return true;
            }
        }
        return false;
    }

    Vector2 vTrans => new Vector2(X - rootRotate.X, Y - rootRotate.Y);
    /// <summary>
    /// vecto 90
    /// </summary>
    Vector2 v1 => new Vector2(-Y + rootRotate.Y, X - rootRotate.X);
    /// <summary>
    /// vecto 180
    /// </summary>
    Vector2 v2 => new Vector2(-X + rootRotate.X, -Y + rootRotate.Y);
    /// <summary>
    /// vecto 270
    /// </summary>
    Vector2 v3 => new Vector2(Y - rootRotate.Y, -X + rootRotate.X);
    public Vector2 CheckTargetNodeRotate(bool isClockwise, out float angle)
    {
        if (isClockwise)
        {
            // Check 90 dec
            Debug.Log($"<color=green>Check 1 node: {rootRotate.X + (int)v3.x}-{rootRotate.Y + (int)v3.y}</color>");
            if (this.IsCorrectCoordinates(rootRotate.X + (int)v3.x, rootRotate.Y + (int)v3.y))
                if (mapTile.nodes[rootRotate.X + (int)v3.x, rootRotate.Y + (int)v3.y].IsNodeAvailableToMove(this))
                {
                    angle = -90;
                    return new Vector2(rootRotate.X + (int)v3.x, rootRotate.Y + (int)v3.y);
                }
            // Check 180 dec
            Debug.Log($"<color=green>Check 2 node: {rootRotate.X + (int)v2.x}-{rootRotate.Y + (int)v2.y}</color>");
            if (this.IsCorrectCoordinates(rootRotate.X + (int)v2.x, rootRotate.Y + (int)v2.y))
                if (mapTile.nodes[rootRotate.X + (int)v2.x, rootRotate.Y + (int)v2.y].IsNodeAvailableToMove(this))
                {
                    angle = -180;
                    return new Vector2(rootRotate.X + (int)v2.x, rootRotate.Y + (int)v2.y);
                }
            // Check 270 dec
            Debug.Log($"<color=green>Check 3 node: {rootRotate.X + (int)v1.x}-{rootRotate.Y + (int)v1.y}</color>");
            if (this.IsCorrectCoordinates(rootRotate.X + (int)v1.x, rootRotate.Y + (int)v1.y))
                if (mapTile.nodes[rootRotate.X + (int)v1.x, rootRotate.Y + (int)v1.y].IsNodeAvailableToMove(this))
                {
                    angle = -270;
                    return new Vector2(rootRotate.X + (int)v1.x, rootRotate.Y + (int)v1.y);
                }
            angle = 0;
            return Vector2.one * -1;
        }
        else
        {
            // Check 90 dec
            Debug.Log($"<color=green>Check 4 node: {rootRotate.X + (int)v1.x}-{rootRotate.Y + (int)v1.y}</color>");
            if (this.IsCorrectCoordinates(rootRotate.X + (int)v1.x, rootRotate.Y + (int)v1.y))
                if (mapTile.nodes[rootRotate.X + (int)v1.x, rootRotate.Y + (int)v1.y].IsNodeAvailableToMove(this))
                {
                    angle = 90;
                    return new Vector2(rootRotate.X + (int)v1.x, rootRotate.Y + (int)v1.y);
                }

            // Check 180 dec
            Debug.Log($"<color=green>Check 5 node: {rootRotate.X + (int)v2.x}-{rootRotate.Y + (int)v2.y}</color>");
            if (this.IsCorrectCoordinates(rootRotate.X + (int)v2.x, rootRotate.Y + (int)v2.y))
                if (mapTile.nodes[rootRotate.X + (int)v2.x, rootRotate.Y + (int)v2.y].IsNodeAvailableToMove(this))
                {
                    angle = 180;
                    return new Vector2(rootRotate.X + (int)v2.x, rootRotate.Y + (int)v2.y);
                }
            // Check 90 dec
            Debug.Log($"<color=green>Check 6 node: {rootRotate.X + (int)v3.x}-{rootRotate.Y + (int)v3.y}</color>");
            if (this.IsCorrectCoordinates(rootRotate.X + (int)v3.x, rootRotate.Y + (int)v3.y))
                if (mapTile.nodes[rootRotate.X + (int)v3.x, rootRotate.Y + (int)v3.y].IsNodeAvailableToMove(this))
                {
                    angle = 270;
                    return new Vector2(rootRotate.X + (int)v3.x, rootRotate.Y + (int)v3.y);
                }
            angle = 0;
            return Vector2.one * -1;
        }
    }

}
public static class ExtensionMethod
{
    public static bool IsCorrectCoordinates(this TileBase tile, int x, int y)
    {
        return x >= 0 && y >= 0 && y < tile.mapTile.totalY && x < tile.mapTile.totalX;
    }
    public static bool IsNodeAvailableToMove(this Node node, TileBase tile)
    {

        if (node.IsLock) return false;
        if (!node.HaveTile)
            return true;
        else
        {
            if (node.GetTile().Type == Type_Tile.Saw_Blade && (tile.Type == Type_Tile.Arrow || tile.Type == Type_Tile.Bomb || tile.Type == Type_Tile.Stone))
                return true;
            else if ((node.GetTile().Type == Type_Tile.Arrow || node.GetTile().Type == Type_Tile.Bomb || node.GetTile().Type == Type_Tile.Stone)
                && tile.Type == Type_Tile.Saw_Blade)
                return true;
            else
                return false;
        }
    }
}