using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public float speed;
    public bool looping = true;

    void Update()
    {
        if (Player.isDead == false && Player.victory == false)
        {
            transform.Translate(-transform.up * speed * Time.deltaTime);

            if (transform.position.y <= -1 && looping)
            {
                transform.position = Vector3.zero;
            }
        }
    }
}
