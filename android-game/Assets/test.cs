using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
    }


    // Update is called once per frame
    void Update()
    {
        // CastRayToWorld();
        if (Input.GetMouseButtonDown(0))
            DestroyThem();
    }

    void DestroyThem()
    {
        gameObject.AddComponent<TriangleExplosion>();

        StartCoroutine(gameObject.GetComponent<TriangleExplosion>().SplitMesh(true));
    }
    void CastRayToWorld()
    {
        var Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var point = Ray.origin + (Ray.direction * 4.5f);
        Debug.Log("World point " + point);
    }
}
