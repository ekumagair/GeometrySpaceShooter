using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        GameStats.initializedGame = true;
    }

    void OnEnable()
    {
        LoadingScreen.calledLoadScreen = false;
        GameStats.SetFPSFromConfiguration();
    }

    public void RevivePlayer()
    {
        Player.instance.gameObject.SetActive(true);
        Player.instance.InitializePlayer(6);
        Player.instance.healthScript.Revive();
        HUD.instance.windowVictory.SetActive(false);
        HUD.instance.windowLose.SetActive(false);
        HUD.instance.removeAdsButton.gameObject.SetActive(false);
        HUD.instance.windowExtra.gameObject.SetActive(false);
        HUD.instance.createdWindow = false;
    }

    public void MultiplyCurrentScoreBy2()
    {
        GameStats.AddPoints(GameStats.currentLevelPoints);
        GameStats.multipliedCurrentScore = true;
        GameStats.SaveStats();
    }

    public void NextLevel()
    {
        if (GameStats.currentLevelType == GameStats.LevelType.MAIN)
        {
            GameStats.level++;
            LevelGenerator.campaignDifficulty++;
        }

        GameStats.currentLevelType = GameStats.LevelType.MAIN;
        LevelGenerator.isBossStage = false;
        GameStats.SaveStats();
    }

    public void GoToStartScene()
    {
        Time.timeScale = 1.0f;
        StartScene.startOverride = StartScene.StartOverride.Default;
        LoadingScreen.CallLoadScreen(LoadingScreen.Scenes.Start);
    }

    public void GoToStartSceneUpgrades()
    {
        Time.timeScale = 1.0f;
        StartScene.startOverride = StartScene.StartOverride.GoToUpgrades;
        LoadingScreen.CallLoadScreen(LoadingScreen.Scenes.Start);
    }

    public void GoToGameScene()
    {
        Time.timeScale = 1.0f;
        StartScene.startOverride = StartScene.StartOverride.Default;
        LoadingScreen.CallLoadScreen(LoadingScreen.Scenes.Game);
    }

    public void GoToRemoveAds()
    {
        if (Player.victory)
        {
            NextLevel();
        }

        StartScene.startOverride = StartScene.StartOverride.GoToRemoveAds;
        LoadingScreen.CallLoadScreen(LoadingScreen.Scenes.Start);
    }
}
