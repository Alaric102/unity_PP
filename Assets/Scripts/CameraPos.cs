using UnityEngine;

public class CameraPos : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    void FixedUpdate ()
    {
        Vector3 desPos = target.position + offset;
        Vector3 smoothedPos = Vector3.Lerp(target.position, desPos, smoothSpeed);
        transform.position = smoothedPos;

        // transform.LookAt(target);
    }
}
