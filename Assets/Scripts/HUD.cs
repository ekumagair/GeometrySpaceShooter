using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HUD : MonoBehaviour
{
    public GameObject statTarget;
    public TMP_Text healthNumber;
    public TMP_Text pointsThisLevelNumberWin;
    public TMP_Text pointsThisLevelNumberLose;
    public GameObject windowVictory;
    public GameObject windowLose;

    bool createdWindow = false;
    PersistentCanvas persistentCanvas;
    Health statTargetHealth;
    GameObject player;

    void Start()
    {
        persistentCanvas = GameObject.Find("PersistentCanvas").GetComponent<PersistentCanvas>();
        statTargetHealth = statTarget.GetComponent<Health>();
        windowVictory.SetActive(false);
        windowLose.SetActive(false);
        createdWindow = false;
        player = GameObject.FindGameObjectWithTag("Player");

        persistentCanvas.SetLevelText();
    }

    void Update()
    {
        healthNumber.text = statTargetHealth.health.ToString();
        pointsThisLevelNumberWin.text = GameStats.currentLevelPoints.ToString();
        pointsThisLevelNumberLose.text = GameStats.currentLevelPoints.ToString();

        if (createdWindow == false)
        {
            if (Player.victory == true)
            {
                windowVictory.SetActive(true);
                windowVictory.GetComponent<Animator>().Play("WindowAppearFromAbove");
                GameStats.SaveStats();
                createdWindow = true;
            }
            if (Player.isDead == true)
            {
                windowLose.SetActive(true);
                windowLose.GetComponent<Animator>().Play("WindowAppearFromAbove");
                GameStats.SaveStats();
                createdWindow = true;
            }
        }

        if (Debug.isDebugBuild)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                statTarget.SetActive(true);
                statTarget.GetComponent<Player>().InitializePlayer();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                LevelGenerator.campaignDifficulty--;
                GameStats.level--;
                SceneManager.LoadScene("GameScene");
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                LevelGenerator.campaignDifficulty++;
                GameStats.level++;
                SceneManager.LoadScene("GameScene");
            }
        }
    }

    public void RevivePlayer()
    {
        if (player != null)
        {
            player.SetActive(true);
            player.GetComponent<Player>().InitializePlayer();
            windowVictory.SetActive(false);
            windowLose.SetActive(false);
            createdWindow = false;
        }
    }
}
