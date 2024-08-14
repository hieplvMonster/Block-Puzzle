using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TileGenerator : MonoBehaviour
{
    [SerializeField] MapTile maptile;
    [SerializeField] ArrowPz arrowPrefab;
    [SerializeField] string dataPath = "Assets/===GAME===/Resources/";
    [OnInspectorGUI] private void Space1() { GUILayout.Space(10); }

    [HorizontalGroup("Split", .5f)]
    [Button("Create map tile", ButtonSizes.Large), GUIColor(0, 1, 0)]

    public void CreateMapTile()
    {
        int x = 0, y = 0;
        int dir = 0;
        int count = 0;
        int s = 1;

        // Khởi tạo vị trí bắt đầu đúng
        x = Mathf.FloorToInt(maptile.totalX / 2) - (maptile.totalX % 2 == 0 ? 1 : 0);
        y = Mathf.FloorToInt(maptile.totalY / 2) - (maptile.totalY % 2 == 0 ? 1 : 0);

        if (maptile.nodes[x, y].IsSelect)
            CreateTileAvailable(maptile.nodes[x, y]);
        // Duyệt từng vòng xoắn ốc


        for (int k = 1; k <= maptile.totalX - 1; k++)
        {
            // Duyệt từng hướng trong vòng xoắn ốc
            for (int j = 0; j < (k < maptile.totalX - 1 ? 2 : 3); j++)
            {
                // Duyệt từng bước trong hướng hiện tại
                for (int i = 0; i < s; i++)
                {
                    // Kiểm tra giới hạn mảng **sau** khi di chuyển
                    switch (dir)
                    {
                        case 0: y++; break; // Phải
                        case 1: x++; break; // Xuống
                        case 2: y--; break; // Trái
                        case 3: x--; break; // Lên
                    }

                    // Kiểm tra giới hạn mảng
                    if (x >= 0 && x < maptile.totalX && y >= 0 && y < maptile.totalX)
                    {
                        //Debug.Log($"{x}-{y} : {matrix[x, y]}");
                        Node o = maptile.nodes[x, y];
                        if (o.IsSelect) CreateTileAvailable(o);
                        count++;
                    }
                    else
                    {
                        // Nếu vượt quá giới hạn mảng, chuyển hướng
                        dir = (dir + 1) % 4;
                        break;
                    }
                }
                dir = (dir + 1) % 4; // Chuyển hướng
            }
            s++; // Tăng số bước cho vòng xoắn ốc tiếp theo
        }
    }
    public void CreateTileAvailable(Node node)
    {
        List<int> directions = new List<int>() { 0, 1, 2, 3 };

        // Create a arrow clone for check way out
        ArrowPz arrowPz = new ArrowPz();
        arrowPz.X = node.X;
        arrowPz.Y = node.Y;
        arrowPz.SetMapTile(maptile);

        while (directions.Count > 0)
        {
            directions.Shuffle();
            arrowPz.SetDirection((Direction)directions.First());
            if (arrowPz.FindNode())
            {
                // Create Arrow with direction in this node
                CreateArrowPuzzle(node, (Direction)directions.First());
                Debug.Log($"{node.X}-{node.Y} === {(Direction)directions.First()}");
                break;
            }
            else
                directions.RemoveAt(0);
        }
    }

    void CreateArrowPuzzle(Node node, Direction direction)
    {
        ArrowPz arrow = Instantiate(arrowPrefab, node.transform);
        arrow.transform.position = node.transform.position;
        arrow.transform.rotation = Quaternion.identity;
        node.SetTile(arrow);

        arrow.SetMapTile(maptile);
        arrow.SetDirection(direction);
        arrow.SetVisual();
        arrow.X = node.X;
        arrow.Y = node.Y;
        maptile.AddTile(arrow);
        arrow.AddActionOnMoveTile();
        maptile.TotalArrow++;
    }
    [HorizontalGroup("Split/right")]
    [Button("Clear Map", ButtonSizes.Large), GUIColor(1, .2f, 0)]
    public void ClearMapTile()
    {
        foreach (var x in maptile.tiles)
        {
            DestroyImmediate(x.gameObject);
        }
        maptile.tiles.Clear();
    }
    [OnInspectorGUI] private void Space2() { GUILayout.Space(20); }

    [Button("Export Data Map", ButtonSizes.Large, DirtyOnClick = true, Stretch = false), GUIColor(0.4f, 0.8f, 1)]

    public void ExportMapData()
    {
        TileMapData data = ScriptableObject.CreateInstance<TileMapData>();
        data.mapSize = maptile.totalX;
        data.sizeCameraOrtho = Camera.main.orthographicSize;
        data.posCam = Camera.main.transform.position;
        data.AddMapNode(maptile);
        data.AddMapTile(maptile);
        string path;
        int id = 0;
        do
        {
            path = string.Format($"{dataPath}{id.ToString("D6")}.asset");
            id++;
        }
        while (AssetDatabase.LoadAssetAtPath<TileMapData>(path) != null);

        //if 
        //{
        //    Debug.LogError("There have an asset with the same name!!!\nPLEASE CHANGE THE NAME MAP!!!");
        //    return;
        //}
        AssetDatabase.CreateAsset(data, path);
        AssetDatabase.SaveAssets();
        Debug.Log($"SAVE MAP {(id - 1).ToString("D6")} !");
    }

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
                nodeFind.IsSelect = !nodeFind.IsSelect;
            }
            //if (InputAction.HaveNodeInPosition(pos, layerNode, .2f, out nodeFind))
            //{
            //    nodeFind.IsSelect = !nodeFind.IsSelect;
            //}
        }
    }
}
