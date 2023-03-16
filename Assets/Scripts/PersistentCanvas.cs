using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PersistentCanvas : MonoBehaviour
{
    public TMP_Text pointsNumber;
    public GameObject soundButton;
    public TMP_Text levelText;

    void Update()
    {
        pointsNumber.text = GameStats.points.ToString();

        // Debug.
        if (Debug.isDebugBuild)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                GameStats.points += 200;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                GameStats.points -= 200;
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                GameStats.points *= GameStats.points;
            }
        }
    }

    public void SetLevelText()
    {
        if (levelText != null)
        {
            levelText.text += " " + GameStats.level.ToString();
        }
    }

    public void CreateButtonSound()
    {
        Instantiate(soundButton, transform.position, transform.rotation);
    }

    public void MultiplyCurrentScoreBy2()
    {
        GameStats.AddPoints(GameStats.currentLevelPoints);
        GameStats.multipliedCurrentScore = true;
        GameStats.SaveStats();
    }

    public void NextLevel()
    {
        GameStats.level++;
        LevelGenerator.campaignDifficulty++;
        GameStats.SaveStats();
    }

    public void GoToStartScene()
    {
        StartScene.goToUpgrades = false;
        SceneManager.LoadScene("StartScene");
    }

    public void GoToStartSceneUpgrades()
    {
        StartScene.goToUpgrades = true;
        SceneManager.LoadScene("StartScene");
    }
}
