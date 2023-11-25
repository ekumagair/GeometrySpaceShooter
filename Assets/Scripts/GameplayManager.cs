using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public void RevivePlayer()
    {
        Player.instance.gameObject.SetActive(true);
        Player.instance.InitializePlayer(6);
        Player.instance.healthScript.Revive();
        HUD.instance.windowVictory.SetActive(false);
        HUD.instance.windowLose.SetActive(false);
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
        StartScene.goToUpgrades = false;
        LoadingScreen.CallLoadScreen(LoadingScreen.Scenes.Start);
    }

    public void GoToStartSceneUpgrades()
    {
        StartScene.goToUpgrades = true;
        LoadingScreen.CallLoadScreen(LoadingScreen.Scenes.Start);
    }

    public void GoToGameScene()
    {
        StartScene.goToUpgrades = false;
        LoadingScreen.CallLoadScreen(LoadingScreen.Scenes.Game);
    }
}
