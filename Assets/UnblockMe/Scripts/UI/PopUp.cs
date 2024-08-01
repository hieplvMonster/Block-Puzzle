using UnityEngine;
using System.Collections;

public class PopUp : MonoBehaviour {
    public GameObject TransBG;
	// Use this for initialization
	void OnEnable () {
        TransBG.transform.position = new Vector3(0f, 0f, 0f);
        BoxCollider2D[] ColliderArray = FindObjectsOfType<BoxCollider2D>();
        foreach (BoxCollider2D Collider in ColliderArray)
        {
            Collider.enabled = false;
        }
	}

    void OnDisable()
    {
        TransBG.transform.position = new Vector3(500f, 500f, 0f);
        BoxCollider2D[] ColliderArray = FindObjectsOfType<BoxCollider2D>();
        foreach (BoxCollider2D Collider in ColliderArray)
        {
            Collider.enabled = true;
        }
    }

	// Update is called once per frame
	void Update () {
	
	}
}
