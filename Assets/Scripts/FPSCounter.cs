using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    TMP_Text counter;

    void Start()
    {
        counter = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (counter != null)
        {
            counter.text = "FPS: " + (int)(1f / Time.unscaledDeltaTime);
        }
    }
}
