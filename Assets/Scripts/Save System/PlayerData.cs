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
    public int[] upgradePrice = new int[6];
    public uint[] upgradePurchaseAmount = new uint[6];

    public float optionSfx;
    public float optionMusic;
    public int optionTrails;
    public int optionImpacts;

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

        upgradePrice = new int[6];
        for (int i = 0; i < upgradePrice.Length; i++)
        {
            upgradePrice[i] = GameStats.upgradePrice[i];
        }

        upgradePurchaseAmount = new uint[6];
        for (int i = 0; i < upgradePurchaseAmount.Length; i++)
        {
            upgradePurchaseAmount[i] = GameStats.upgradePurchaseAmount[i];
        }

        optionSfx = Options.soundVolume;
        optionMusic = Options.musicVolume;
        optionTrails = Options.projectileTrails;
        optionImpacts = Options.projectileImpacts;
    }
}