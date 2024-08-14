using DG.Tweening;
using DG.Tweening.Core.Easing;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform center; // The object to orbit around
    public float radius = 1f; // The distance from the center
    public float duration = .2f;
    public float angleRot = 90;
    public float rootAng = 0f;

    //void Update()
    //{
    //    //// Calculate the angle in radians
    //    //float angle = Time.time * angularSpeed * Mathf.Deg2Rad;
     
    //    // Calculate the orbiting object's position
    //    float x = center.position.x + radius * Mathf.Cos(angleRot * Mathf.Deg2Rad);
    //    float y = center.position.y + radius * Mathf.Sin(angleRot * Mathf.Deg2Rad); // Adjust y as needed for your orbit plane
    //    float z = center.position.z;

    //    transform.position = new Vector3(x, y, z);
    //}
    Coroutine coRot = null;

    [Button("Rotate Around")]
    public void RotateAroundRoot()
    {
        Debug.Log("Play!");
        if (coRot == null)
            coRot = StartCoroutine(IRotateAround(center, angleRot, duration));
    }
    [SerializeField] AnimationCurve curve;
    public float time = 0;
    public float _angle;
    IEnumerator IRotateAround(Transform root, float angle, float duration)
    {
        time = 0;
        while (time <= duration)
        {
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
            _angle = angle * curve.Evaluate(time / duration);
            SetPosByRadAngle((_angle + rootAng) * Mathf.Deg2Rad); ;
        }
        SetPosByRadAngle((angle + rootAng) * Mathf.Deg2Rad);
        rootAng += angle;
        coRot = null;
    }

    void SetPosByRadAngle(float radAngle)
    {
        Vector3 offset = center.position;
        float x = offset.x + radius * Mathf.Cos(radAngle);
        float y = offset.y + radius * Mathf.Sin(radAngle);
        float z = offset.z;
        transform.position = new Vector3(x, y, z);
    }
}
