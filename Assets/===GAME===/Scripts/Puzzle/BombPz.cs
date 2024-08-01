using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombPz : MonoBehaviour, ITile
{
    int x, y;

    [SerializeField] GameObject explosionEff;
    [ShowInInspector]
    public int X { get => x; set => x = value; }
    [ShowInInspector]
    public int Y { get => y; set => y = value; }
    public Type_Tile Type { get => Type_Tile.Bomb; }
    public MapTile mapTile { get; set; }

    public void OnDestroyTile()
    {
    }

    public void OnMove1Tile(int _x, int _y)
    {
        Debug.Log("Bomb Do nothing!");
    }

    TilePz cacheTile = null;
    List<TilePz> tilesCheck;

    [BoxGroup("Explode Tile"), Button("Explode"), GUIColor(1, .67f, 0)]
    public void OnTap()
    {
        // Show 
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
    }
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
    [SerializeField] MapTile m;
    void Start()
    {
        x = 2; y = 2;
        ITile tile = GetComponent<ITile>();
        mapTile = m;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetMapTile(MapTile mapTile)
    {
        this.mapTile = mapTile;
    }
}
