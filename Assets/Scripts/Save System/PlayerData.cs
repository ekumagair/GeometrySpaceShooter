using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int level;
    public int points;
    public int campaignDifficulty;
    public int startHealth;
    public float firingSpeed;
    public float moveSpeed;
    public float projectileSpeed;
    public int shootLevel;
    public int projectileDamage;
    public int projectilePerforation;
    public int projectileAutoDamage;
    public int[] upgradePrice;
    public uint[] upgradePurchaseAmount;
    
    public float optionSfx;
    public float optionMusic;
    public int optionTrails;
    public int optionImpacts;
    public int optionBackground;
    public int optionFps;

    public bool iapRemoveAds;
    public bool iapRemovedAdsOnce;

    public int enemiesKilledTotal;
    public int claimedRewardsTotal;
    public float scoreMultiplierAtStart;
    public int[] claimedRewards;
    public int[] completedExtraLevels;

    public string savedVersion;

    public PlayerData()
    {
        savedVersion = Application.version.ToString();

        level = GameStats.level;
        points = GameStats.points;
        campaignDifficulty = LevelGenerator.campaignDifficulty;
        startHealth = Player.startHealth;
        firingSpeed = Player.firingSpeedDivider;
        moveSpeed = Player.moveSpeedMultiplier;
        projectileSpeed = Player.projectileSpeedMultiplier;
        shootLevel = Player.shootLevel;
        projectileDamage = Player.projectileDamage;
        projectilePerforation = Player.projectilePerforation;
        projectileAutoDamage = Player.projectileAutoDamage;

        upgradePrice = new int[GameConstants.UPGRADE_AMOUNT];
        for (int i = 0; i < upgradePrice.Length; i++)
        {
            upgradePrice[i] = GameStats.upgradePrice[i];
        }

        upgradePurchaseAmount = new uint[GameConstants.UPGRADE_AMOUNT];
        for (int i = 0; i < upgradePurchaseAmount.Length; i++)
        {
            upgradePurchaseAmount[i] = GameStats.upgradePurchaseAmount[i];
        }

        optionSfx = Options.soundVolume;
        optionMusic = Options.musicVolume;
        optionTrails = Options.projectileTrails;
        optionImpacts = Options.projectileImpacts;
        optionBackground = Options.backgroundType;
        optionFps = Options.fpsSetting;

        iapRemoveAds = PurchaseManager.removedAds;
        iapRemovedAdsOnce = PurchaseManager.removedAdsOnce;

        enemiesKilledTotal = GameStats.enemiesKilledTotal;

        claimedRewardsTotal = GameStats.claimedRewardsTotal;
        claimedRewards = new int[GameConstants.REWARDS_AMOUNT];
        scoreMultiplierAtStart = ScoreChain.scoreMultiplierAtStart;
        for (int i = 0; i < claimedRewards.Length; i++)
        {
            claimedRewards[i] = GameStats.claimedRewards[i];
        }

        completedExtraLevels = new int[GameConstants.EXTRA_LEVELS_AMOUNT];
        for (int i = 0; i < completedExtraLevels.Length; i++)
        {
            completedExtraLevels[i] = GameStats.completedExtraLevels[i];
        }
    }
}
