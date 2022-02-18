using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drozhanie : MonoBehaviour
{
    private float c = 0.01f;
    private float X, Y, Z;
    void Start()
    {
        X = transform.position.x;
        Y = transform.position.y;
        Z = transform.position.z;
    }

    void Update()
    {
        System.Random random = new System.Random();

        float deltaX = (float)(random.NextDouble() * 2 * c - c);
        float deltaZ = (float)(random.NextDouble() * 2 * c - c);

        transform.position = new Vector3(X + deltaX, Y, Z + deltaZ);
    }
}
