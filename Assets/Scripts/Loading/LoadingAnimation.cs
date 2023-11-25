using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingAnimation : MonoBehaviour
{
    private Image _img;

    void Awake()
    {
        _img = GetComponent<Image>();
        _img.fillClockwise = true;
    }

    void Update()
    {
        if (_img.fillClockwise)
        {
            if (_img.fillAmount < 1)
            {
                _img.fillAmount += Time.deltaTime * 2f;
            }
            else
            {
                _img.fillClockwise = false;
            }
        }
        else
        {
            if (_img.fillAmount > 0)
            {
                _img.fillAmount -= Time.deltaTime * 2f;
            }
            else
            {
                _img.fillClockwise = true;
            }
        }
    }
}
