using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour
{
    public List<TilePz> tiles = new List<TilePz>();
    public Node[,] nodes;
    public int totalX, totalY;
    private void Awake()
    {
        GetMapNode();
        for (int i = 0; i < totalX; i++)
            for (int j = 0; j < totalY; j++)
            {
                nodes[i, j].Setup(i, j, this);
                //Debug.Log($"{i},{j} - {newNodes[i][j]}");
            }
    }
    private void Start()
    {
        for (int i = 0; i < totalX; i++)
            for (int j = 0; j < totalY; j++)
            {
                if (nodes[i, j].GetTile())
                    nodes[i, j].SetTile(nodes[i, j].GetTile());
            }
    }
    void GetMapNode()
    {
        int cCount = transform.childCount;
        nodes = new Node[totalX, totalY];
        for (int i = 0; i < cCount; i++)
        {
            int index = i;
            int y = i / totalX;
            int x = i % totalX;
            nodes[x, y] = transform.GetChild(index).GetComponent<Node>();
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
    public void AddTile(TilePz tilePz)
    {
        tiles.Add(tilePz);
        tilePz.SetMapTile(this);
    }
    bool isMoveTile = false;

    //public Action onRemove1Tile = null;
    public void RemoveTile(TilePz tilePz)
    {
        tiles.Remove(tilePz);
        nodes[tilePz.x, tilePz.y].SetTile(tile: null);
        //onRemove1Tile?.Invoke();
    }
    public delegate void OnMoveTile(int x, int y);
    public OnMoveTile onMoveTile = null;
}
