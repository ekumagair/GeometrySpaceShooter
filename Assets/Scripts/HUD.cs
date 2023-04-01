using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public GameObject statTarget;
    public TMP_Text healthNumber;
    public TMP_Text pointsThisLevelNumberWin;
    public TMP_Text pointsThisLevelNumberLose;
    public GameObject windowVictory;
    public GameObject windowLose;
    public Button pauseButton;
    public GameObject pauseButtonSound;
    public Sprite pauseButtonDefault;
    public Sprite pauseButtonPressed;
    public TMP_Text pauseText;
    public TMP_Text basicInstructions;

    public static float previousTimeScale = 1.0f;

    bool createdWindow = false;
    PersistentCanvas persistentCanvas;
    Health statTargetHealth;
    GameObject player;
    Image pauseButtonImage;

    void Start()
    {
        persistentCanvas = GameObject.Find("PersistentCanvas").GetComponent<PersistentCanvas>();
        statTargetHealth = statTarget.GetComponent<Health>();
        windowVictory.SetActive(false);
        windowLose.SetActive(false);
        createdWindow = false;
        previousTimeScale = 1.0f;
        player = GameObject.FindGameObjectWithTag("Player");
        pauseButtonImage = pauseButton.gameObject.GetComponent<Image>();
        pauseText.gameObject.SetActive(false);
    }

    void Update()
    {
        healthNumber.text = statTargetHealth.health.ToString();
        pointsThisLevelNumberWin.text = GameStats.currentLevelPoints.ToString();
        pointsThisLevelNumberLose.text = GameStats.currentLevelPoints.ToString();

        if (createdWindow == false)
        {
            pauseButton.gameObject.SetActive(true);

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

            if(GameStats.level <= 2 && Player.hasInput == false && Time.timeScale != 0.0f)
            {
                basicInstructions.enabled = true;
            }
            else
            {
                basicInstructions.enabled = false;
            }
        }
        else
        {
            pauseButton.gameObject.SetActive(false);
            basicInstructions.enabled = false;
        }

        if (Debug.isDebugBuild)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RevivePlayer();
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
            player.GetComponent<Player>().InitializePlayer(5);
            windowVictory.SetActive(false);
            windowLose.SetActive(false);
            createdWindow = false;
        }
    }

    public void TogglePause()
    {
        if(pauseButtonSound != null)
        {
            Instantiate(pauseButtonSound, transform.position, transform.rotation);
        }

        if(Time.timeScale != 0.0f)
        {
            pauseText.gameObject.SetActive(true);
            pauseButtonImage.sprite = pauseButtonPressed;
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0.0f;
        }
        else
        {
            pauseText.gameObject.SetActive(false);
            pauseButtonImage.sprite = pauseButtonDefault;
            Time.timeScale = previousTimeScale;
        }
    }
}
