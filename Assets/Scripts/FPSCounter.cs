using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    private TMP_Text _counter;

    void Start()
    {
        _counter = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (_counter != null)
        {
            _counter.text = "FPS: " + (int)(1f / Time.unscaledDeltaTime);
        }
    }
}
