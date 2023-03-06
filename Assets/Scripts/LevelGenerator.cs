using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public int bottomPositionY = 7;
    public int levelLength = 12;
    public int spread = 2;
    public GameObject[] enemyPrefabs;

    public static int campaignDifficulty = 0;

    int prefabMin, prefabMax = 0;

    void Start()
    {
        SetEnemyPrefabRange(campaignDifficulty);
        SetLevelLength(campaignDifficulty);
        SetLevelDensity(campaignDifficulty);

        for (int i = 0; i < levelLength; i++)
        {
            if (i % spread == 0)
            {
                // Spawn main shape.
                float randomX = Random.Range(-2f, 2f);
                Instantiate(enemyPrefabs[Random.Range(prefabMin, prefabMax)], new Vector3(randomX, bottomPositionY + i, 0), transform.rotation);

                // Random chance to spawn another shape at the same Y position.
                if (Random.Range(0, campaignDifficulty) > 5)
                {
                    float distance;
                    if (randomX > 0)
                    {
                        distance = -1.5f;
                    }
                    else
                    {
                        distance = 1.5f;
                    }

                    int extraShapePrefabMax = prefabMin + 2;

                    if (extraShapePrefabMax > prefabMax)
                    {
                        extraShapePrefabMax = prefabMax;
                    }

                    Instantiate(enemyPrefabs[Random.Range(prefabMin, extraShapePrefabMax)], new Vector3(randomX + distance, bottomPositionY + i, 0), transform.rotation);
                }
            }
        }
    }

    void SetEnemyPrefabRange(int reference)
    {
        prefabMin = reference - 5;
        prefabMax = reference + 1;

        if (prefabMin < 0)
        {
            prefabMin = 0;
        }
        if (prefabMin > enemyPrefabs.Length - 1)
        {
            prefabMin = enemyPrefabs.Length - 1;
        }
        if (prefabMax > enemyPrefabs.Length)
        {
            prefabMax = enemyPrefabs.Length;
        }
    }

    void SetLevelLength(int reference)
    {
        levelLength = Mathf.Clamp((spread * 9) + (spread * Mathf.FloorToInt(reference / 1.1f)), 0, spread * 1000);
    }

    void SetLevelDensity(int reference)
    {
        if(reference <= 10)
        {
            spread = 3;
        }
        else if (reference > 10 && reference <= 30)
        {
            spread = 2;
        }
        else if (reference > 30)
        {
            spread = 1;
        }
    }

    public static void MoveEnemies(float y)
    {
        Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
        foreach(Enemy enemy in enemies)
        {
            if (enemy.gameObject.transform.position.y > 6 && enemy.gameObject.transform.position.y + y >= 6)
            {
                enemy.gameObject.transform.Translate(Vector3.up * y);
            }
        }
    }
}
