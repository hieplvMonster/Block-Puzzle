using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    [SerializeField] int level;
    Node[,] nodes;
    TileMapData data;
    MapTile maptile;
    public int turnMoveAvailable = 0;

    Transform parentTransform;
    [Button("LoadMap", Stretch = false)]
    public void LoadMap()
    {
        data = Resources.Load<TileMapData>(level.ToString("D6"));

        #region Draw Node
        GameObject gridParent = new GameObject("Map");
        maptile = gridParent.AddComponent<MapTile>();
        parentTransform = gridParent.transform;
        turnMoveAvailable = data.minMoveTurn;

        nodes = new Node[data.mapSize, data.mapSize];
        for (int y = 0; y < data.mapSize; y++)
        {
            for (int x = 0; x < data.mapSize; x++)
            {
                // Instantiate a new node at the correct position
                GameObject nodeObject = Instantiate(TilePreset.Instance.nodePrefab, new Vector3(x * 1.5f, y * 1.5f, 0), Quaternion.identity);
                nodeObject.name = $"Node_{x}_{y}";
                nodeObject.transform.parent = parentTransform;

                Node node = nodeObject.GetComponent<Node>();
                node.Setup(x, y, gridParent.GetComponent<MapTile>());
                node.UnVisual();
                nodes[x, y] = node;
            }
        }
        gridParent.GetComponent<MapTile>().CreateMap(data.mapSize, data.mapSize)
            .SetNodesMap(nodes);
        #endregion

        #region Set Data Node
        foreach (var vector in data.mapNodes.Keys)
        {
            nodes[vector.x, vector.y].typeNode = data.mapNodes[vector];
        }
        #endregion

        #region Draw Tile
        foreach (var key in data.mapTiles.Keys)
        {
            if (key == Type_Tile.Arrow)
            {
                // Draw Arrow
                DrawArrow(data.mapTiles[key]);
            }
        }
        #endregion
        #region SETUP CAMERA
        Camera.main.transform.position = data.posCam;
        Camera.main.orthographicSize = data.sizeCameraOrtho;
        #endregion
    }

    ArrowPz arrowClone;
    void DrawArrow(List<Vector2Int> dataPos)
    {
        foreach (var pos in dataPos)
        {
            arrowClone = Instantiate(TilePreset.Instance.arrowPrefab, Vector3.zero, Quaternion.identity);
            arrowClone.transform.SetParent(nodes[pos.x, pos.y].transform);
            nodes[pos.x, pos.y].SetTile(arrowClone);
            arrowClone.X = pos.x; arrowClone.Y = pos.y;
            arrowClone.SetDirection(data.arrowsDirection[new Vector2Int(pos.x, pos.y)]);
            arrowClone.SetVisual();
            maptile.AddTile(arrowClone);
            arrowClone.AddActionOnMoveTile();
            arrowClone.transform.localPosition = Vector3.zero;
        }
        maptile.TotalArrow = dataPos.Count;
    }

    [Header("Action Tap")]
    [SerializeField] Camera cameraMain;
    [SerializeField] LayerMask layerNode;

    Node nodeFind;
    Vector2 pos;
    Collider2D[] cols = new Collider2D[10];

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            pos = InputAction.ScreenToWorldVector2(cameraMain);
            if (Physics2D.OverlapCircleNonAlloc(pos, .2f, cols, layerNode) > 0)
            {
                nodeFind = cols[0].transform.GetComponent<Node>();
                if (nodeFind.HaveTile)
                {
                    nodeFind.GetTile().OnTap(out bool canTap);
                    if (canTap) turnMoveAvailable--;
                    if (turnMoveAvailable <= 0)
                    {
                        if(maptile.TotalArrow==0)
                        {
                            // TODO: GAME WIN
                        }
                        else
                        {
                            // TODO: GAME LOSE
                        }
                    }
                }
            }
        }
    }
}
