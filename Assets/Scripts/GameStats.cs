using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameStats
{
    public static int level = 1;
    public static int points = 0;
    public static int currentLevelPoints = 0;
    public static bool multipliedCurrentScore = false;
    public static int[] upgradePrice = new int[6];
    public static uint[] upgradePurchaseAmount = new uint[6];

    // Enable ad buttons.
    public static bool enableAdButttons = true;

    // Level type:
    public static int currentLevelType = 0;
    // 0 = Main campaign.
    // 1 = Preset level.

    public static void AddPoints(int p)
    {
        points += p;
        currentLevelPoints += p;

        // Score limit (2 billion)
        if (points > 2000000000)
        {
            points = 2000000000;
            currentLevelPoints = points;
        }
    }

    public static void SaveStats()
    {
        PlayerPrefs.Save();
        SaveSystem.SaveData();

        //Debug.Log("Saved stats");
    }

    public static void LoadStats()
    {
        PlayerData data = SaveSystem.LoadData();

        if (data != null)
        {
            level = data.level;
            points = data.points;
            LevelGenerator.campaignDifficulty = data.campaignDifficulty;
            Player.startHealth = data.startHealth;
            Player.firingSpeedDivider = data.firingSpeed;
            Player.moveSpeedMultiplier = data.moveSpeed;
            Player.projectileSpeedMultiplier = data.projectileSpeed;
            Player.shootLevel = data.shootLevel;
            Player.projectileDamage = data.projectileDamage;

            Options.soundVolume = data.optionSfx;
            Options.musicVolume = data.optionMusic;
            Options.projectileTrails = data.optionTrails;
            Options.projectileImpacts = data.optionImpacts;

            upgradePrice = new int[6];
            for (int i = 0; i < data.upgradePrice.Length; i++)
            {
                GameStats.upgradePrice[i] = data.upgradePrice[i];
            }

            upgradePurchaseAmount = new uint[6];
            for (int i = 0; i < data.upgradePurchaseAmount.Length; i++)
            {
                GameStats.upgradePurchaseAmount[i] = data.upgradePurchaseAmount[i];
            }
        }

        //Debug.Log("Loaded stats");
    }
}
