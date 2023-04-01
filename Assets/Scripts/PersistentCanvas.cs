using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PersistentCanvas : MonoBehaviour
{
    public TMP_Text pointsNumber;
    public GameObject soundButton;
    public GameObject numberChangeEffect;
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
            if (Input.GetKeyDown(KeyCode.V))
            {
                NextLevel();
                GoToStartScene();
            }
        }
    }

    public void CreateButtonSound()
    {
        Instantiate(soundButton, transform.position, transform.rotation);
    }

    public void CreateNumberChangeEffect(Vector3 pos, string textString, Color textColor, float speedYMultiplier)
    {
        GameObject nc = Instantiate(numberChangeEffect, pos, transform.rotation, transform);
        nc.GetComponent<TMP_Text>().text = textString;
        nc.GetComponent<TMP_Text>().color = textColor;
        nc.GetComponent<NumberChangeText>().moveYSpeed *= speedYMultiplier;
        nc.GetComponent<RectTransform>().anchoredPosition = pos;
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
        LevelGenerator.isBossStage = false;
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
