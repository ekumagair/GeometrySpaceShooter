using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericRotate : MonoBehaviour
{
    // A GameObject with this component will constantly rotate on the specified axes.
    public Vector3 speed;

    void Update()
    {
        transform.Rotate(speed.x * Time.deltaTime, speed.y * Time.deltaTime, speed.z * Time.deltaTime);
    }
}
