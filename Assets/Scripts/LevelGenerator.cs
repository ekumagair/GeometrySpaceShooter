using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public int bottomPositionY = 7;
    public int levelLength = 12;
    public static int spread = 2;
    public GameObject[] enemyPrefabs;
    public GameObject[] bossPrefabs;

    public static int campaignDifficulty = 0;
    public static bool isBossStage = false;

    int prefabMin, prefabMax = 0;

    void Start()
    {
        isBossStage = false;
        SetEnemyPrefabRange(campaignDifficulty);
        SetLevelDensity(campaignDifficulty);
        SetLevelLength(campaignDifficulty);

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

        // Create boss.
        if(GameStats.level % 7 == 0 && GameStats.level > 1)
        {
            isBossStage = true;
            int chosenBoss = (GameStats.level / 7) - 1;

            if(chosenBoss > bossPrefabs.Length - 1)
            {
                chosenBoss = bossPrefabs.Length - 1;
            }

            DestroyAllEnemiesAboveY(16);
            Instantiate(bossPrefabs[chosenBoss], new Vector3(0, 20, 0), transform.rotation);
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

    void SetLevelDensity(int reference)
    {
        if(reference <= 100)
        {
            spread = 3;
        }
        else if (reference > 100 && reference <= 200)
        {
            spread = 2;
        }
        else if (reference > 200)
        {
            spread = 1;
        }
    }

    void SetLevelLength(int reference)
    {
        levelLength = Mathf.Clamp((spread * 9) + (spread * Mathf.FloorToInt(reference / 1.1f)), 0, spread * 500);
    }

    public static void MoveEnemies(float y)
    {
        Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
        foreach(Enemy enemy in enemies)
        {
            if (enemy.gameObject.transform.position.y > 6 && enemy.gameObject.transform.position.y + y >= 6 && LevelGenerator.spread > 2 && enemy.canBeMovedY)
            {
                float movementMultiplier = 1.0f;

                if (y < 0)
                {
                    // If the position is low, move less.
                    if (enemy.gameObject.transform.position.y + y < 8)
                    {
                        movementMultiplier *= 0.25f;
                    }
                    if (enemy.gameObject.transform.position.y + y < 12)
                    {
                        movementMultiplier *= 0.5f;
                    }
                    if (enemy.gameObject.transform.position.y + y < 24)
                    {
                        movementMultiplier *= 0.5f;
                    }
                }

                enemy.gameObject.transform.Translate(Vector3.up * (y * movementMultiplier));
            }
        }
    }

    public static void DestroyAllEnemiesAboveY(float y)
    {
        Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            if(enemy.transform.position.y > y)
            {
                Destroy(enemy.gameObject);
            }
        }
    }
}
