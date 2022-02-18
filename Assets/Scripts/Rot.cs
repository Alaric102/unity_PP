using UnityEngine;

public class Rot : MonoBehaviour
{
    void Update()
    {
        // Rotate the object around its local y axis at 1 degree per second
        transform.RotateAround(transform.position, transform.forward, Time.deltaTime * 90f);
    }
}