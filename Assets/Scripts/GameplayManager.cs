using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
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
        Player.Instance.gameObject.SetActive(true);
        Player.Instance.InitializePlayer(6);
        Player.Instance.healthScript.Revive();
        HUD.Instance.windowVictory.SetActive(false);
        HUD.Instance.windowLose.SetActive(false);
        HUD.Instance.removeAdsButton.gameObject.SetActive(false);
        HUD.Instance.windowExtra.gameObject.SetActive(false);
        HUD.Instance.createdWindow = false;
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
