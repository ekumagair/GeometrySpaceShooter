using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAwayEffect : MonoBehaviour
{
    SpriteRenderer _sr;
    float alpha = 1f;

    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        alpha = _sr.color.a;
    }

    void Update()
    {
        _sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, alpha);
        alpha -= 0.0075f;

        transform.localScale *= 1.0025f;

        if(alpha <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
