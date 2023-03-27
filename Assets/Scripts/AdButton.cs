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

        if (Debug.isDebugBuild == true)
        {
            button.interactable = false;
        }
    }

    void Update()
    {
        if(destroyIfMultipliedScore && GameStats.multipliedCurrentScore)
        {
            Destroy(gameObject);
        }
    }
}
