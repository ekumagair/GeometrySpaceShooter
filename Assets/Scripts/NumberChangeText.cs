using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NumberChangeText : MonoBehaviour
{
    public float moveYSpeed = 1;
    public float alpha = 1;
    public float fadeOutSpeed = 1;

    TMP_Text tmp;

    void Start()
    {
        tmp = GetComponent<TMP_Text>();
    }

    void Update()
    {
        transform.position += transform.up * moveYSpeed * Time.deltaTime;

        tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, alpha);
        alpha -= 4f * fadeOutSpeed * Time.deltaTime;

        if (alpha <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
