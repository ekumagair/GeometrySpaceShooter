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
    public GameObject[] formations;

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

        // Check if this is a boss level or not.
        if (GameStats.level % 7 == 0 && GameStats.level > 1)
        {
            // Is a boss level.

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

            #region Items
            // Items.
            if (GameStats.level > 20)
            {
                // Create an extra health item.
                int pos = Random.Range(bottomPositionY + 2, levelLength / 2);
                CreateItem(1, pos);

                if (Random.Range(0, 100) == 0)
                {
                    // If you are lucky, create another extra health item.
                    CreateItem(1, pos + 1);
                }
            }
            if (GameStats.level > 30)
            {
                // Create extra points item.
                CreateItem(0, Random.Range(bottomPositionY + 2, levelLength / 3));
            }
            if (GameStats.level > 40)
            {
                // Create shot size increase item.
                CreateItem(2, Random.Range(levelLength / 4, levelLength / 3));
            }
            if (GameStats.level > 50)
            {
                // Create another extra points item.
                int pos = Random.Range(levelLength / 3, levelLength / 2);
                CreateItem(0, pos);

                if (Random.Range(0, 100) == 0)
                {
                    // If you are lucky, create more points items.
                    CreateItem(0, pos + 1);
                    CreateItem(0, pos + 2);
                    CreateItem(0, pos + 3);
                }
            }
            if (GameStats.level > 60)
            {
                if (Random.Range(0, GameStats.level) >= 40)
                {
                    // Random chance: Create another shot size increase item.
                    CreateItem(2, Random.Range(levelLength / 3, levelLength / 2));
                }
            }
            #endregion

            #region Enemy Formations
            // Formations.
            int posPrimary = Random.Range(levelLength / 4, levelLength / 3);
            int posSecondary = Random.Range((levelLength / 3) + 1, levelLength / 2);

            int posOffset = Random.Range(5, 20) * (Random.Range(0, 2) == 0 ? 1 : -1);

            if (GameStats.level >= 9 && GameStats.level <= 15)
            {
                // Triangles 1.
                if (Random.Range(0, 5) == 0)
                {
                    CreateFormation(0, posPrimary);
                }
                if (Random.Range(0, GameStats.level) >= 12)
                {
                    CreateFormation(0, posSecondary);
                }
            }
            else if (GameStats.level >= 16 && GameStats.level <= 22)
            {
                // Triangles 2.
                if (Random.Range(0, 2) == 0)
                {
                    CreateFormation(1, posPrimary);
                }
                if (Random.Range(0, GameStats.level) >= 19)
                {
                    CreateFormation(1, posSecondary); // Extra.
                }
            }
            else if (GameStats.level >= 23 && GameStats.level <= 29)
            {
                // Triangles 3.
                CreateFormation(2, posPrimary);

                if (Random.Range(0, GameStats.level) >= 26)
                {
                    CreateFormation(2, posSecondary); // Extra.
                }
            }
            else if (GameStats.level >= 30 && GameStats.level <= 36)
            {
                // Triangles 4.
                CreateFormation(3, posPrimary);

                if (Random.Range(0, GameStats.level) >= 33)
                {
                    CreateFormation(3, posSecondary); // Extra.
                }
            }
            else if (GameStats.level >= 37 && GameStats.level <= 43)
            {
                // Triangles 5.
                CreateFormation(4, posPrimary);

                if (Random.Range(0, GameStats.level) >= 40)
                {
                    CreateFormation(4, posSecondary); // Extra.
                }
            }
            else if (GameStats.level >= 44 && GameStats.level <= 50)
            {
                // Triangles 6.
                CreateFormation(5, posPrimary);

                if (Random.Range(0, GameStats.level) >= 47)
                {
                    CreateFormation(5, posSecondary); // Extra.
                }
            }
            else if (GameStats.level >= 51)
            {
                // Triangles 7.
                CreateFormation(6, posPrimary);

                if (Random.Range(0, GameStats.level) >= 54)
                {
                    CreateFormation(6, posSecondary); // Extra.
                }
            }

            if (GameStats.level >= 45 && GameStats.level <= 65)
            {
                // Pentagons 6.
                CreateFormation(7, posPrimary + posOffset);

                if (Random.Range(0, GameStats.level) >= 50)
                {
                    CreateFormation(7, posSecondary + posOffset); // Extra.
                }
            }
            else if (GameStats.level >= 66)
            {
                // Pentagons 7.
                CreateFormation(8, posPrimary + posOffset);

                if (Random.Range(0, GameStats.level) >= 70)
                {
                    CreateFormation(8, posSecondary + posOffset); // Extra.
                }
            }

            // Hexagon wall.
            if (Random.Range(0, GameStats.level) >= 50)
            {
                CreateFormation(9, Random.Range(bottomPositionY, levelLength / 2));
            }

            // Misc.
            if (Random.Range(0, GameStats.level) >= 70)
            {
                CreateFormation(Random.Range(0, 2) == 0 ? 6 : 8, Random.Range(bottomPositionY, levelLength / 2));
            }
            #endregion
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

    void CreateFormation(int type, float posY)
    {
        Instantiate(formations[type], new Vector3(0, bottomPositionY + posY, 0), transform.rotation);
    }
}
