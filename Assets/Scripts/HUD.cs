using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HUD : MonoBehaviour
{
    public GameObject statTarget;
    public TMP_Text healthNumber;
    public GameObject windowVictory;

    bool createdWindow = false;
    PersistentCanvas persistentCanvas;
    Health statTargetHealth;

    void Start()
    {
        persistentCanvas = GameObject.Find("PersistentCanvas").GetComponent<PersistentCanvas>();
        statTargetHealth = statTarget.GetComponent<Health>();
        windowVictory.SetActive(false);
        createdWindow = false;

        persistentCanvas.SetLevelText();
    }

    void Update()
    {
        healthNumber.text = statTargetHealth.health.ToString();

        if (createdWindow == false)
        {
            if (Player.victory == true)
            {
                windowVictory.SetActive(true);
                windowVictory.GetComponent<Animator>().Play("WindowAppearFromAbove");
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
}
