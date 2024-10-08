using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArrowPz : TileBase
{
    public override Type_Tile Type { get => Type_Tile.Arrow; }


    [SerializeField, EnumToggleButtons, HideLabel] Direction direction;
    public void SetDirection(Direction dir) => direction = dir;
    public Direction GetDirection() => direction;
    protected override void Awake()
    {
        base.Awake();
    }
    public void AddActionOnMoveTile() => mapTile.onMoveTile += OnMove1Tile;
    public override void OnDestroyTile()
    {
        mapTile.TotalArrow--;
        base.OnDestroyTile();
    }

    public override void OnMove1Tile(int _x, int _y)
    {
        base.OnMove1Tile(_x, _y);
    }

    public override void OnTap(out bool canTap)
    {
        base.OnTap(out canTap);
        canTap = true;
        MoveTile();
    }

    public override void SetMapTile(MapTile mapTile)
    {
        base.SetMapTile(mapTile);
    }
    #region Button Inspector
    [Button("Set Visual Tile")]
    public override void SetVisual()
    {
        mainSprite.GetComponent<SpriteRenderer>().sprite = TilePreset.Instance.GetAssetArrow(direction);
    }
    #endregion

    Node targetFind = null;
    [BoxGroup("Move Tile"), Button("Find target node", ButtonSizes.Medium)]
    public bool FindNode()
    {
        resultX = x; resultY = y;
        if (!FindtargetNode(out targetFind))
        {
            if (targetFind == null)
            {
                //Debug.LogError("STAY!");
            }
            else
            {
                if (targetFind.IsLock && targetFind.IsTrafficPole)
                    Debug.LogError("Find lock bt traffic" + targetFind.name);
                else
                    Debug.LogError("Find " + targetFind.name);
            }
            return false;
        }
        else
        {
            //Debug.LogError("Out");
            return true;
        }
    }

    int resultX, resultY, nextX, nextY;
    /// <summary>
    /// Func check wayout of tile in map node
    /// </summary>
    /// <param name="target"> target node if tile stop in map</param>
    /// <returns> result = true when tile out map node, if not = false</returns>
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
        // if move out grid
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
            Debug.Log($"<color=green>OUT in {x}-{y} with {direction}</color>");
            return true;
        }
        // if node is traffic pole and lock
        if (mapTile.nodes[nextX, nextY].IsLock)
        {
            Debug.LogError("!");
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
            return false;
        }
        // else check if have tile
        if (mapTile.nodes[nextX, nextY].HaveTile)
        {
            // if tile is saw blade
            if (mapTile.nodes[nextX, nextY].GetTile().Type == Type_Tile.Saw_Blade)
            {
                Debug.Log("1");
                target = mapTile.nodes[nextX, nextY];
                return false;
            }
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
            // if target node is itself
            if (nextX == x && nextY == y)
                target = null;
            // else not
            else
            {
                target = mapTile.nodes[nextX, nextY];
            }
            return false;
        }
        else
        {
            // check if node is change direction node?

            resultX = nextX;
            resultY = nextY;
            target = mapTile.nodes[nextX, nextY];
            if (target.IsNodeChangeDirection())
                return false;
            return FindtargetNode(out targetFind);
        }
    }
    [BoxGroup("Move Tile"), Button("Move"), GUIColor(0, 1, .03f)]
    public void MoveTile(bool isNewMove = true)
    {
        if (TypeOverlay != Type_Overlay.None) return;

        resultX = x; resultY = y;
        if (!FindtargetNode(out targetFind))
        {
            if (targetFind == null)
            {
                Debug.LogError("STAY!");
                if (isNewMove)
                    mapTile.onMoveTile?.Invoke(x, y);
            }
            else
            {
                Debug.LogError("Find " + targetFind.name);
                transform.DOMove(targetFind.transform.position, .5f)
                    .OnComplete(() =>
                    {
                        if (isNewMove)
                            mapTile.onMoveTile?.Invoke(x, y);
                        // Destroy when move into saw blade

                        mapTile.nodes[x, y].SetTile(null);
                        targetFind.SetTile(this);
                        X = targetFind.X; Y = targetFind.Y;
                        transform.SetParent(targetFind.transform);
                        if (targetFind.IsNodeChangeDirection())
                        {
                            direction = targetFind.GetDirection();
                            MoveTile(false);
                        }
                        if (targetFind.GetTile().Type == Type_Tile.Saw_Blade)
                        {
                            OnDestroyTile();
                        }
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
                    if (isNewMove)
                        mapTile.onMoveTile?.Invoke(x, y);
                    OnDestroyTile();
                });
            Debug.LogError("Out");
        }
    }
    private void OnDisable()
    {
        mapTile.onMoveTile -= OnMove1Tile;
    }
}
