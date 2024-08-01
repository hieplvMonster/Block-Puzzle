using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPz : MonoBehaviour, ITile
{
    int x, y;
    public MapTile mapTile { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public int X { get => x; set => x = value; }
    public int Y { get => x; set => x = value; }
    public Type_Tile Type { get => Type_Tile.Arrow; }

    [SerializeField, EnumToggleButtons, HideLabel] Direction direction;

    public void OnDestroyTile()
    {
    }

    public void OnMove1Tile(int _x, int _y)
    {
    }

    public void OnTap()
    {
    }

    public void SetMapTile(MapTile mapTile)
    {
    }

    Node targetFind = null;
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
    [BoxGroup("Move Tile"), Button("Move"), GUIColor(0, 1, .03f)]
    public void MoveTile()
    {
        mapTile.onMoveTile?.Invoke(x, y);
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
                        mapTile.nodes[x, y].SetTile(itile: null);
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
                    //Debug.Log(gameObject.name);
                    gameObject.SetActive(false);
                    //mapTile.RemoveTile(this);
                });
            Debug.LogError("Out");
        }
        //// Check target node
        //if (!FindtargetNode(out Node tar))
        //{

        //}
    }
}
