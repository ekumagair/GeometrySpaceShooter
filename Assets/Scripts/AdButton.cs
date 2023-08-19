using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdButton : MonoBehaviour
{
    public bool destroyIfMultipliedScore = false;

    Button button;

    void Start()
    {
        button = GetComponent<Button>();

        if (GameStats.enableAdButttons == false)
        {
            button.interactable = false;
        }
    }

    void Update()
    {
        if (destroyIfMultipliedScore && GameStats.multipliedCurrentScore)
        {
            Destroy(gameObject);
        }
    }
}
