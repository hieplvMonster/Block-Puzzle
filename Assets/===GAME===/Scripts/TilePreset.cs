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

    public Sprite GetAssetArrow(Direction direction) => sprites[(int)direction];

}
