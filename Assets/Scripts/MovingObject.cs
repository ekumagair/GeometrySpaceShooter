using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public float speed;
    public float destroyIfUnderY = -100;
    public bool isLoopingBackground = false;
    public bool isFinishLine = false;

    void Update()
    {
        if (Player.isDead == false && Player.victory == false)
        {
            // Movement.
            if (isLoopingBackground == false || (isLoopingBackground == true && Options.backgroundType == 0))
            {
                transform.Translate(-transform.up * speed * Time.deltaTime);
            }

            // Other properties.
            if (isLoopingBackground == true && Options.backgroundType == 2)
            {
                gameObject.SetActive(false);
            }
            if (isLoopingBackground == true && transform.position.y <= -1)
            {
                transform.position = new Vector3(0, 0, transform.position.z);
            }
            if (isFinishLine == true && transform.position.y < -8)
            {
                transform.position = new Vector3(transform.position.x, 7, transform.position.z);
            }
            if (transform.position.y < destroyIfUnderY)
            {
                Destroy(gameObject);
            }
        }
    }
}
