using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Checkbox : MonoBehaviour
{
    public Image checkImage;
    public GameObject pressSound;

    public enum Value
    {
        ProjectileTrails
    }
    public Value value;

    void Update()
    {
        switch (value)
        {
            case Value.ProjectileTrails:
                checkImage.enabled = IntToBool(Options.projectileTrails);
                break;

            default:
                break;
        }
    }

    bool IntToBool(int integer)
    {
        return integer == 1;
    }

    public void ButtonSound()
    {
        if (pressSound != null)
        {
            Instantiate(pressSound, transform.position, transform.rotation);
        }
    }
}
