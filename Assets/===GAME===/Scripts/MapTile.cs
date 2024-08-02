using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour
{
    public List<TileBase> tiles = new List<TileBase>();

    int totalArrow;
    [ShowInInspector]
    public int TotalArrow
    {
        get => totalArrow;
        set
        {
            if (totalArrow <= 0)
            {
                // TODO: Game win
                OnClearMap?.Invoke();
                totalArrow = 0;
            }
            else
                totalArrow = value;
        }
    }
    public Node[,] nodes;
    public int totalX, totalY;
    private void Awake()
    {
        GetMapNode();
        for (int i = 0; i < totalX; i++)
            for (int j = 0; j < totalY; j++)
            {
                //nodes[i, j].Setup(i, j, this);
                Debug.Log(nodes[i, j].gameObject.name);
            }
    }
    private void Start()
    {
        //for (int i = 0; i < totalX; i++)
        //    for (int j = 0; j < totalY; j++)
        //    {
        //        if (nodes[i, j].GetTile())
        //            nodes[i, j].SetTile(nodes[i, j].GetTile());
        //    }
    }
    void GetMapNode()
    {
        totalArrow = 0;
        int cCount = transform.childCount;
        nodes = new Node[totalX, totalY];
        for (int i = 0; i < cCount; i++)
        {
            int index = i;
            int y = i / totalX;
            int x = i % totalX;
            nodes[x, y] = transform.GetChild(index).GetComponent<Node>();
            TotalArrow++;
        }
    }
    public MapTile CreateMap(int x, int y)
    {
        nodes = new Node[x, y];
        totalX = x; totalY = y;

        //Debug.Log(newNodes.Count);
        //Debug.Log(newNodes[0].Count);
        //tiles = new TilePz[r, c];
        //Debug.LogError($"Total lenght = {tiles.Length}");
        return this;
    }
    public void SetNodesMap(Node[,] nodes)
    {
        this.nodes = nodes;
        for (int i = 0; i < totalX; i++)
            for (int j = 0; j < totalY; j++)
            {
                //newNodes[i][j] = nodes[i, j];
            }
    }
    public void AddTile(TileBase tilePz)
    {
        tiles.Add(tilePz);
        tilePz.SetMapTile(this);
    }
    bool isMoveTile = false;

    //public Action onRemove1Tile = null;
    public void RemoveTile(TileBase tilePz)
    {
        tiles.Remove(tilePz);
        //nodes[tilePz.X, tilePz.Y].SetTile(null);
        //onRemove1Tile?.Invoke();
    }
    public delegate void OnMoveTile(int x, int y);
    public OnMoveTile onMoveTile = null;

    public event Action OnClearMap = null;
}
