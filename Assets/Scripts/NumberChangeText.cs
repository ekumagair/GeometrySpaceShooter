using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NumberChangeText : MonoBehaviour
{
    public float moveYSpeed = 1;
    public float alpha = 1;
    public float fadeOutSpeed = 1;

    private float _speedMultiplier = 1.25f;
    private TMP_Text _tmp;

    void Start()
    {
        _tmp = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (Time.timeScale != 0.0f)
        {
            transform.position += transform.up * moveYSpeed * _speedMultiplier * Time.deltaTime;

            if (_speedMultiplier > 0)
            {
                _speedMultiplier -= Time.deltaTime * 2;
            }
            if (_speedMultiplier < 0)
            {
                _speedMultiplier = 0;
            }

            _tmp.color = new Color(_tmp.color.r, _tmp.color.g, _tmp.color.b, alpha);
            alpha -= 4f * fadeOutSpeed * Time.deltaTime;
        }

        if (alpha <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
