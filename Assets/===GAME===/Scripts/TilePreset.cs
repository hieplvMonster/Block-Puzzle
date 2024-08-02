using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Create New Tile Preset", fileName = "New Tile Preset")]
public class TilePreset : ScriptableObject
{
    static TilePreset instance;

    public static TilePreset Instance
    {
        get
        {
            if (instance == null)
                instance = Resources.Load<TilePreset>("Tile Preset");
            return instance;
        }
    }
    [SerializeField] List<Sprite> sprites = new List<Sprite>();
    [SerializeField] Sprite spritesStone;
    [SerializeField] Sprite spritesBomb;
    [SerializeField] Sprite spritesSawBlade;

    public Sprite GetAssetArrow(Direction direction) => sprites[(int)direction];
    public Sprite GetAssetStone() => spritesStone;
    public Sprite GetAssetBomb() => spritesBomb;
    public Sprite GetAssetSawBlade() => spritesSawBlade;

}
