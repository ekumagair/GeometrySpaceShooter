using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int givePoints = 0;
    public int giveHealth = 0;
    public int giveCondition = -1;
    public int giveConditionTime = 0;
    public GameObject[] createOnCollect;
}
