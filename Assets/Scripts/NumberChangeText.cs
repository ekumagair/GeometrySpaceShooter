using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NumberChangeText : MonoBehaviour
{
    public float moveYSpeed = 1;
    public float alpha = 1;
    public float fadeOutSpeed = 1;

    float speedMultiplier = 1.25f;
    TMP_Text tmp;

    void Start()
    {
        tmp = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (Time.timeScale != 0.0f)
        {
            transform.position += transform.up * moveYSpeed * speedMultiplier * Time.deltaTime;

            if (speedMultiplier > 0)
            {
                speedMultiplier -= Time.deltaTime * 2;
            }
            if (speedMultiplier < 0)
            {
                speedMultiplier = 0;
            }

            tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, alpha);
            alpha -= 4f * fadeOutSpeed * Time.deltaTime;
        }

        if (alpha <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
