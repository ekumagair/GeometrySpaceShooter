using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameStats
{
    public static int level = 1;
    public static int points = 0;
    public static int currentLevelPoints = 0;
    public static int enemiesKilledTotal = 0;
    public static int claimedRewardsTotal = 0;
    public static bool multipliedCurrentScore = false;
    public static int[] upgradePrice = new int[GameConstants.UPGRADE_AMOUNT];
    public static uint[] upgradePurchaseAmount = new uint[GameConstants.UPGRADE_AMOUNT];
    public static int[] claimedRewards = new int[GameConstants.REWARDS_AMOUNT];
    public static int[] completedExtraLevels = new int[GameConstants.EXTRA_LEVELS_AMOUNT];

    // Initialized game.
    public static bool initializedGame = false;
    public static bool failedGenuine = false;
    public static bool loadStatsFinished = false;

    // Enable ad buttons.
    public static bool enableAdButttons = true;

    // Level type:
    public enum LevelType
    {
        MAIN,
        PRESET
    }
    public static LevelType currentLevelType = LevelType.MAIN;

    public static void AddPoints(int p)
    {
        points += p;
        currentLevelPoints += p;

        // Score limit (2 billion)
        if (points > GameConstants.MAX_POINTS)
        {
            points = GameConstants.MAX_POINTS;
        }
        if (currentLevelPoints > GameConstants.MAX_POINTS)
        {
            currentLevelPoints = GameConstants.MAX_POINTS;
        }
    }

    public static void SaveStats()
    {
        PlayerPrefs.Save();
        SaveSystem.SaveData();

        //if (Debug.isDebugBuild) { Debug.Log("Saved stats"); }
    }

    public static void LoadStats()
    {
        loadStatsFinished = false;
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

            upgradePrice = new int[GameConstants.UPGRADE_AMOUNT];
            for (int i = 0; i < data.upgradePrice.Length; i++)
            {
                if (i > upgradePrice.Length || data.upgradePrice.Length < 1)
                {
                    continue;
                }
                upgradePrice[i] = data.upgradePrice[i];
            }

            upgradePurchaseAmount = new uint[GameConstants.UPGRADE_AMOUNT];
            for (int i = 0; i < data.upgradePurchaseAmount.Length; i++)
            {
                if (i > upgradePurchaseAmount.Length || data.upgradePurchaseAmount.Length < 1)
                {
                    continue;
                }
                upgradePurchaseAmount[i] = data.upgradePurchaseAmount[i];
            }

            // Version conditions
            if (data.savedVersion != "0.9" && data.savedVersion != "1.0" && data.savedVersion != "1.1")
            {
                // Versions 1.2 onwards.
                Player.projectileAutoDamage = data.projectileAutoDamage;
                Player.projectilePerforation = data.projectilePerforation;

                if (data.savedVersion != "1.2" && data.savedVersion != "1.2.1")
                {
                    // Versions 1.3 onwards.
                    Options.backgroundType = data.optionBackground;
                    GameStats.enemiesKilledTotal = data.enemiesKilledTotal;

                    if (data.iapRemovedAdsOnce == true)
                    {
                        PurchaseManager.removedAdsOnce = true;
                    }

                    claimedRewardsTotal = data.claimedRewardsTotal;
                    claimedRewards = new int[GameConstants.REWARDS_AMOUNT];
                    ScoreChain.scoreMultiplierMinimum = data.scoreMultiplierMinimum;
                    for (int i = 0; i < data.claimedRewards.Length; i++)
                    {
                        if (i > claimedRewards.Length || data.claimedRewards.Length < 1)
                        {
                            continue;
                        }
                        claimedRewards[i] = data.claimedRewards[i];
                    }

                    completedExtraLevels = new int[GameConstants.EXTRA_LEVELS_AMOUNT];
                    for (int i = 0; i < data.completedExtraLevels.Length; i++)
                    {
                        if (i > completedExtraLevels.Length || data.completedExtraLevels.Length < 1)
                        {
                            continue;
                        }
                        completedExtraLevels[i] = data.completedExtraLevels[i];
                    }
                }
                else
                {
                    Options.backgroundType = 0;
                    GameStats.enemiesKilledTotal = 0;
                    claimedRewardsTotal = 0;
                    claimedRewards = new int[GameConstants.REWARDS_AMOUNT];
                    ScoreChain.scoreMultiplierMinimum = 1.0f;
                    completedExtraLevels = new int[GameConstants.EXTRA_LEVELS_AMOUNT];
                }
            }
            else
            {
                Player.projectileAutoDamage = 0;
                Player.projectilePerforation = 1;
            }
        }

        loadStatsFinished = true;

        //if (Debug.isDebugBuild) { Debug.Log("Loaded stats"); }
    }
}
