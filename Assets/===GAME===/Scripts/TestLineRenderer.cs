using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLineRenderer : MonoBehaviour
{
    [SerializeField] LineRenderer line;
    int posCount = 1;
    private void Awake()
    {
        posCount = 1;
        line.positionCount = posCount;
        line.SetPosition(0, transform.position);
    }
    [SerializeField] Vector2 pos;
    [Button("Set Point")]
    public void AddPoint()
    {
        posCount++;
        line.positionCount = posCount;
        line.SetPosition(posCount - 1, pos);
    }
}
