using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] Transform root;
    [SerializeField] float speed;
    [SerializeField] float from, to;
    private void Update()
    {
        //transform.RotateAround(root.position, Vector3.forward, speed);
    }
    [ContextMenu("Rotate")]
    public void RotateAround()
    {
        //DOVirtual.Float(from, to, speed, (val) =>
        //{
        //    //transform.Rotate  
        //});
        transform.DOLocalMoveY(5, 2)
            .OnComplete(() => gameObject.SetActive(false));
    }
}
