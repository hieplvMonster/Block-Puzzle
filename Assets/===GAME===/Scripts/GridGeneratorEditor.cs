using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridGeneratorEditor : EditorWindow
{
    private GameObject nodePrefab;
    private ArrowPz tilePrefab;
    private StonePz stonePrefab;
    private SawBladePz sawBladePrefab;
    private BombPz bombPrefab;
    private int gridWidth = 5;
    private int gridHeight = 5;
    private float nodeSpacing = 1.5f;
    private string GridName = string.Empty;
    private Transform parentTransform;
    private Node[,] nodes;

    [MenuItem("Tools/Grid Generator")]
    public static void ShowWindow()
    {
        GetWindow<GridGeneratorEditor>("Grid Generator");
    }
    int num = 0;
    private void OnGUI()
    {
        GUILayout.Label("Grid Settings", EditorStyles.boldLabel);

        nodePrefab = (GameObject)EditorGUILayout.ObjectField("Node Prefab", nodePrefab, typeof(GameObject), false);
        tilePrefab = (ArrowPz)EditorGUILayout.ObjectField("Arrow block", tilePrefab, typeof(ArrowPz), false);
        stonePrefab = (StonePz)EditorGUILayout.ObjectField("Stone block", stonePrefab, typeof(StonePz), false);
        sawBladePrefab = (SawBladePz)EditorGUILayout.ObjectField("Saw blade block", sawBladePrefab, typeof(SawBladePz), false);
        bombPrefab = (BombPz)EditorGUILayout.ObjectField("Bomb block", bombPrefab, typeof(BombPz), false);
        gridWidth = EditorGUILayout.IntField("Grid Width", gridWidth);
        gridHeight = EditorGUILayout.IntField("Grid Height", gridHeight);
        nodeSpacing = EditorGUILayout.FloatField("Node Spacing", nodeSpacing);
        GridName = EditorGUILayout.TextField("Grid Name", GridName);

        if (GUILayout.Button("Generate Grid"))
        {
            if (nodePrefab != null)
            {
                GenerateGrid();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Please assign a Node Prefab.", "OK");
            }
        }

        EditorGUILayout.BeginVertical();
        CreateTile();
        CreateStoneBlock();
        CreateSawBladeBlock();
        CreateBombBlock();
        DeleteTile();
        EditorGUILayout.EndVertical();
    }

    private void DeleteTile()
    {
        if (Selection.count == 0) return;
        Node _node = Selection.activeGameObject.GetComponent<Node>();

        if (GUILayout.Button("Delete Tile"))
        {
            if (_node is null)
            {
                Debug.LogError("Selected game object is not Node!");
                return;
            }
            Debug.Log($"Destroy Tile! at {_node.name}");
            _node.GetMapTile().RemoveTile(_node.GetTile());
            DestroyImmediate(_node.GetTile().gameObject);
        }
    }

    private void CreateTile()
    {
        if (Selection.count == 0) return;
        Selection.activeGameObject.TryGetComponent<Node>(out Node _node);
        if (GUILayout.Button("Create Arrow"))
        {
            if (_node is null)
            {
                Debug.LogError("Selected game object is not Node!");
                return;
            }
            ArrowPz o = Instantiate(tilePrefab, Vector3.zero, Quaternion.identity).GetComponent<ArrowPz>();
            o.transform.SetParent(_node.transform);
            _node.SetTile(o);
            o.X = _node.X; o.Y = _node.Y;
            Debug.Log($"Set Tile {o.X},{o.Y} with node {_node.X},{_node.Y}");
            o.SetVisual();
            _node.GetMapTile().AddTile(o);
            _node.GetMapTile().TotalArrow++;
            //_node.ShowTitle(false);
            o.transform.localPosition
                = Vector3.zero;
        }
    }
    private void CreateStoneBlock()
    {
        if (Selection.count == 0) return;
        Selection.activeGameObject.TryGetComponent<Node>(out Node _node);
        if (GUILayout.Button("Create Stone"))
        {
            if (_node is null)
            {
                Debug.LogError("Selected game object is not Node!");
                return;
            }
            StonePz o = Instantiate(stonePrefab, Vector3.zero, Quaternion.identity).GetComponent<StonePz>();
            o.transform.SetParent(_node.transform);
            _node.SetTile(o);
            o.X = _node.X; o.Y = _node.Y;
            Debug.Log($"<color=gray>Set Stone {o.X},{o.Y} with node {_node.X},{_node.Y}</color>");
            o.SetVisual();
            _node.GetMapTile().AddTile(o);
            //_node.ShowTitle(false);
            o.transform.localPosition
                = Vector3.zero;
        }
    }
    private void CreateSawBladeBlock()
    {
        if (Selection.count == 0) return;
        Selection.activeGameObject.TryGetComponent<Node>(out Node _node);
        if (GUILayout.Button("Create Saw Blade"))
        {
            if (_node is null)
            {
                Debug.LogError("Selected game object is not Node!");
                return;
            }
            SawBladePz o = Instantiate(sawBladePrefab, Vector3.zero, Quaternion.identity).GetComponent<SawBladePz>();
            o.transform.SetParent(_node.transform);
            _node.SetTile(o);
            o.X = _node.X; o.Y = _node.Y;
            Debug.Log($"<color=gray>Set Saw Blade {o.X},{o.Y} with node {_node.X},{_node.Y}</color>");
            o.SetVisual();
            _node.GetMapTile().AddTile(o);
            //_node.ShowTitle(false);
            o.transform.localPosition
                = Vector3.zero;
        }
    }
    private void CreateBombBlock()
    {
        if (Selection.count == 0) return;
        Selection.activeGameObject.TryGetComponent<Node>(out Node _node);
        if (GUILayout.Button("Create Bomb"))
        {
            if (_node is null)
            {
                Debug.LogError("Selected game object is not Node!");
                return;
            }
            BombPz o = Instantiate(bombPrefab, Vector3.zero, Quaternion.identity).GetComponent<BombPz>();
            o.transform.SetParent(_node.transform);
            _node.SetTile(o);
            o.X = _node.X; o.Y = _node.Y;
            Debug.Log($"<color=orange>Set Bomb {o.X},{o.Y} with node {_node.X},{_node.Y}</color>");
            o.SetVisual();
            _node.GetMapTile().AddTile(o);
            //_node.ShowTitle(false);
            o.transform.localPosition
                = Vector3.zero;
        }
    }
    private void GenerateGrid()
    {
        num++;
        // Create a new parent object for the grid
        GameObject gridParent = new GameObject(GridName);
        gridParent.AddComponent<MapTile>();
        parentTransform = gridParent.transform;

        nodes = new Node[gridWidth, gridHeight];
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                // Instantiate a new node at the correct position
                GameObject nodeObject = Instantiate(nodePrefab, new Vector3(x * nodeSpacing, y * nodeSpacing, 0), Quaternion.identity);
                nodeObject.name = $"Node_{x}_{y}";
                nodeObject.transform.parent = parentTransform;

                Node node = nodeObject.GetComponent<Node>();
                node.Setup(x, y, gridParent.GetComponent<MapTile>());
                nodes[x, y] = node;

                //// Set neighbors
                //if (x > 0)
                //{
                //    node.west = nodes[x - 1, y];
                //    nodes[x - 1, y].east = node;
                //}
                //if (y > 0)
                //{
                //    node.south = nodes[x, y - 1];
                //    nodes[x, y - 1].north = node;
                //}
            }
        }
        gridParent.GetComponent<MapTile>().CreateMap(gridWidth, gridHeight)
            .SetNodesMap(nodes);
    }
    // Function to get the neighbor node at a given position
    private Node GetNode(int x, int y)
    {
        // Check if the position is within the grid bounds
        if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
        {
            // Get the node object
            GameObject nodeObject = GameObject.Find($"Node_{x}_{y}");
            if (nodeObject != null)
            {
                // Return the node script
                return nodeObject.GetComponent<Node>();
            }
        }
        // Return null if the position is outside the grid bounds
        return null;
    }
}
