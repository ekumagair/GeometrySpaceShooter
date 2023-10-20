using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAwayEffect : MonoBehaviour
{
    private SpriteRenderer _sr;
    private float _alpha = 1f;

    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        _alpha = _sr.color.a;
    }

    void Update()
    {
        if (Time.timeScale != 0.0f)
        {
            _sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, _alpha);
            _alpha -= 7.5f * Time.deltaTime;

            transform.localScale *= 1.005f;

            if (_alpha <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}
