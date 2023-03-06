using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyIfOutsideScreen : MonoBehaviour
{
    void Update()
    {
        if(transform.position.x > 4 || transform.position.x < -4 || transform.position.y > 5 || transform.position.y < -5)
        {
            Destroy(gameObject);
        }
    }
}
