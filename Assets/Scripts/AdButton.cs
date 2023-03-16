using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdButton : MonoBehaviour
{
    public bool destroyIfMultipliedScore = false;

    void Update()
    {
        if(destroyIfMultipliedScore && GameStats.multipliedCurrentScore)
        {
            Destroy(gameObject);
        }
    }
}
