using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public float speed;
    public bool isLoopingBackground = false;
    public float destroyIfUnderY = -100;

    void Update()
    {
        if (Player.isDead == false && Player.victory == false)
        {
            transform.Translate(-transform.up * speed * Time.deltaTime);

            if (isLoopingBackground && transform.position.y <= -1)
            {
                transform.position = new Vector3(0, 0, transform.position.z);
            }
            if(transform.position.y < destroyIfUnderY)
            {
                Destroy(gameObject);
            }
        }
    }
}
