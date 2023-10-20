using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public int bottomPositionY = 7;
    public int levelLength = 12;
    public static int spread = 2;
    public GameObject[] enemyPrefabs;
    public GameObject[] bossPrefabs;
    public GameObject[] itemPrefabs;

    public static int campaignDifficulty = 0;
    public static bool isBossStage = false;

    private int prefabMin, prefabMax = 0;

    void Start()
    {
        isBossStage = false;
        
        if (GameStats.currentLevelType == GameStats.LevelType.MAIN)
        {
            CreateCampaignLevel();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void CreateCampaignLevel()
    {
        SetEnemyPrefabRange(campaignDifficulty);
        SetLevelDensity(campaignDifficulty);
        SetLevelLength(campaignDifficulty);

        for (int i = 0; i < levelLength; i++)
        {
            if (i % spread == 0)
            {
                // Spawn main shape.
                float randomX = Random.Range(-2f, 2f);
                Instantiate(enemyPrefabs[Random.Range(prefabMin, prefabMax)],
                                        new Vector3(randomX, bottomPositionY + i, 0),
                                        transform.rotation);

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

                    Instantiate(enemyPrefabs[Random.Range(prefabMin, extraShapePrefabMax)],
                                            new Vector3(randomX + distance, bottomPositionY + i, 0),
                                            transform.rotation);
                }
            }
        }

        // Create boss.
        if (GameStats.level % 7 == 0 && GameStats.level > 1)
        {
            isBossStage = true;
            int chosenBoss = (GameStats.level / 7) - 1;

            if (chosenBoss > bossPrefabs.Length - 1)
            {
                chosenBoss = bossPrefabs.Length - 1;
            }

            DestroyAllEnemiesAboveY(16);
            Instantiate(bossPrefabs[chosenBoss], new Vector3(0, 20, 0), transform.rotation);
        }
        else
        {
            // If this isn't a boss level.

            if (GameStats.level > 20)
            {
                // Create an extra health item.
                CreateItem(1, Random.Range(bottomPositionY + 2, levelLength / 2));
            }
            if (GameStats.level > 30)
            {
                // Create extra points item.
                CreateItem(0, Random.Range(bottomPositionY + 2, levelLength / 3));
            }
            if (GameStats.level > 50)
            {
                // Create another extra points item.
                CreateItem(0, Random.Range(levelLength / 3, levelLength / 2));
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
        if (prefabMin > enemyPrefabs.Length - 5)
        {
            prefabMin = enemyPrefabs.Length - 5;
        }
        if (prefabMax > enemyPrefabs.Length)
        {
            prefabMax = enemyPrefabs.Length;
        }
    }

    void SetLevelDensity(int reference)
    {
        if (reference <= 100)
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
        levelLength = Mathf.Clamp((spread * 9) + (spread * Mathf.FloorToInt(reference / 1.3f)),
                                   0,
                                   spread * 200);
    }

    public static void MoveEnemies(float y)
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach(Enemy enemy in enemies)
        {
            if (enemy.gameObject.transform.position.y > 6 && enemy.gameObject.transform.position.y + y >= 6 && spread > 2 && enemy.canBeMovedY)
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
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            if (enemy.transform.position.y > y)
            {
                Destroy(enemy.gameObject);
            }
        }
    }

    void CreateItem(int type, float posY)
    {
        var item = Instantiate(itemPrefabs[type], new Vector3(Random.Range(-2f, 2f), bottomPositionY + posY, 0), transform.rotation);
        Item itemScript = item.GetComponent<Item>();

        switch (type)
        {
            case 0:
                itemScript.givePoints = Mathf.RoundToInt(120 * levelLength) + 100;
                break;

            case 1:
                itemScript.giveHealth = Mathf.RoundToInt(Player.startHealth / 1.75f) + 2;
                break;

            default:
                break;
        }
    }
}
