using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutOverlay : MonoBehaviour
{
    public float fadeIncrement;
    public float timeBeforeFade;

    private Image _image;
    private float _alpha = 1f;
    private float _timer = 0f;

    void Start()
    {
        _image = GetComponent<Image>();
        _alpha = _image.color.a;
        _timer = 0f;
    }

    void Update()
    {
        if (Time.timeScale != 0.0f)
        {
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, _alpha);

            if (_timer > timeBeforeFade)
            {
                _alpha -= fadeIncrement * Time.deltaTime;
                if (_alpha <= 0f)
                {
                    Destroy(gameObject);
                }
            }

            _timer += Time.deltaTime;
        }
    }
}
