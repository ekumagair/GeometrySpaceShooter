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

    public static bool enableAdButttons = true;

    public static void AddPoints(int p)
    {
        points += p;
        currentLevelPoints += p;

        // Score limit (2 billion).
        if(points > 2000000000)
        {
            points = 2000000000;
            currentLevelPoints = points;
        }
    }

    public static void SaveStats()
    {
        /*
        PlayerPrefs.SetInt("stat_level", level);
        PlayerPrefs.SetInt("stat_points", points);
        PlayerPrefs.SetInt("stat_campaign_difficulty", LevelGenerator.campaignDifficulty);
        PlayerPrefs.SetInt("player_start_hp", Player.startHealth);
        PlayerPrefs.SetFloat("player_firing_speed", Player.firingSpeedDivider);
        PlayerPrefs.SetFloat("player_move_speed", Player.moveSpeedMultiplier);
        PlayerPrefs.SetFloat("player_projectile_speed", Player.projectileSpeedMultiplier);
        PlayerPrefs.SetInt("player_shoot_level", Player.shootLevel);
        PlayerPrefs.SetInt("player_projectile_damage", Player.projectileDamage);

        PlayerPrefs.SetFloat("option_sfx", Options.soundVolume);
        PlayerPrefs.SetFloat("option_music", Options.musicVolume);
        PlayerPrefs.SetInt("option_trails", Options.projectileTrails);
        PlayerPrefs.SetInt("option_impacts", Options.projectileImpacts);
        */

        PlayerPrefs.Save();
        SaveSystem.SaveData();

        Debug.Log("Saved stats");
    }

    public static void LoadStats()
    {
        /*
        level = PlayerPrefs.GetInt("stat_level");
        points = PlayerPrefs.GetInt("stat_points");
        LevelGenerator.campaignDifficulty = PlayerPrefs.GetInt("stat_campaign_difficulty");
        Player.startHealth = PlayerPrefs.GetInt("player_start_hp");
        Player.firingSpeedDivider = PlayerPrefs.GetFloat("player_firing_speed");
        Player.moveSpeedMultiplier = PlayerPrefs.GetFloat("player_move_speed");
        Player.projectileSpeedMultiplier = PlayerPrefs.GetFloat("player_projectile_speed");
        Player.shootLevel = PlayerPrefs.GetInt("player_shoot_level");
        Player.projectileDamage = PlayerPrefs.GetInt("player_projectile_damage");

        Options.soundVolume = PlayerPrefs.GetFloat("option_sfx");
        Options.musicVolume = PlayerPrefs.GetFloat("option_music");
        Options.projectileTrails = PlayerPrefs.GetInt("option_trails");
        Options.projectileImpacts = PlayerPrefs.GetInt("option_impacts");
        */

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

            for (int i = 0; i < data.upgradePrice.Length; i++)
            {
                GameStats.upgradePrice[i] = data.upgradePrice[i];
            }
        }

        Debug.Log("Loaded stats");
    }
}
