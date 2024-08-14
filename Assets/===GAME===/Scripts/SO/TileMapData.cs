using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapData : SerializedScriptableObject
{
    public int mapSize;
    public float sizeCameraOrtho;
    public Vector3 posCam;

    [Header("MAP NODE")] public Dictionary<Vector2Int, Type_Node> mapNodes = new Dictionary<Vector2Int, Type_Node>();
    [Header("MAP TILE")] public Dictionary<Type_Tile, List<Vector2Int>> mapTiles = new Dictionary<Type_Tile, List<Vector2Int>>();
    [Header("MAP DIRECTION ARROW")] public Dictionary<Vector2Int, Direction> arrowsDirection = new Dictionary<Vector2Int, Direction>();
    public int minMoveTurn;

    public void AddMapNode(MapTile map)
    {
        foreach (var node in map.nodes)
        {
            Vector2Int key = new Vector2Int(node.X, node.Y);
            if (!mapNodes.ContainsKey(key))
            {
                mapNodes.Add(key, node.typeNode);
            }
        }
    }
    public void AddMapTile(MapTile map)
    {
        minMoveTurn = 0;
        tilesCanDestroyByBomb.Clear();
        foreach (var tile in map.tiles)
        {
            if (!mapTiles.ContainsKey(tile.Type))
            {
                mapTiles.Add(tile.Type, new List<Vector2Int>());
            }
            mapTiles[tile.Type].Add(new Vector2Int(tile.X, tile.Y));
            if (tile.Type == Type_Tile.Arrow)
            {
                minMoveTurn++;
                if (!arrowsDirection.ContainsKey(new Vector2Int(tile.X, tile.Y)))
                    arrowsDirection.Add(new Vector2Int(tile.X, tile.Y), (tile as ArrowPz).GetDirection());
            }
            if (tile.Type == Type_Tile.Bomb)
            {
                BombPz bomb = tile as BombPz;
                bomb.CheckTileAround();
                foreach (var x in bomb.tilesCheck)
                {
                    if (!tilesCanDestroyByBomb.Contains(x) && x.Type == Type_Tile.Arrow)
                    {
                        tilesCanDestroyByBomb.Add(x);
                    }
                }
            }
        }
        minMoveTurn -= tilesCanDestroyByBomb.Count;
    }
    HashSet<TileBase> tilesCanDestroyByBomb = new HashSet<TileBase>();

}
