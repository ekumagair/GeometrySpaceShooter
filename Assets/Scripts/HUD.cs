using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization;

public class HUD : MonoBehaviour
{
    public GameObject statTarget;
    public TMP_Text healthNumber;
    public TMP_Text levelText;
    public TMP_Text extraLevelText;
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
    public static Vector3 hudTopLeftCorner = new Vector3(-85, 250, 0);
    public static Vector3 hudBottomRightCorner = new Vector3(100, -190, 0);
    public static HUD instance;

    [HideInInspector] public bool createdWindow = false;
    [HideInInspector] public Vector3 pauseButtonWorldPosition;

    private Health _statTargetHealth;
    private Image _pauseButtonImage;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        _statTargetHealth = statTarget.GetComponent<Health>();
        windowVictory.SetActive(false);
        windowLose.SetActive(false);
        createdWindow = false;
        previousTimeScale = 1.0f;
        _pauseButtonImage = pauseButton.gameObject.GetComponent<Image>();
        pauseButtonWorldPosition = Camera.main.ScreenToWorldPoint(pauseButton.transform.position);
        pauseText.gameObject.SetActive(false);
        ScoreChain.instance.Display(true);

        levelText.enabled = GameStats.currentLevelType == GameStats.LevelType.MAIN;
        extraLevelText.enabled = GameStats.currentLevelType == GameStats.LevelType.PRESET;
    }

    void Update()
    {
        healthNumber.text = _statTargetHealth.health.ToString();
        if (_statTargetHealth.health > Player.startHealth)
        {
            healthNumber.color = Color.green;
        }
        else if (_statTargetHealth.health > Mathf.RoundToInt(Player.startHealth / 4) && _statTargetHealth.health > 1 && _statTargetHealth.health <= Player.startHealth)
        {
            healthNumber.color = Color.white;
        }
        else if (_statTargetHealth.health <= Mathf.RoundToInt(Player.startHealth / 4) || _statTargetHealth.health <= 1)
        {
            healthNumber.color = Color.red;
        }

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
                ScoreChain.instance.Display(false);
                createdWindow = true;
            }
            if (Player.isDead == true)
            {
                windowLose.SetActive(true);
                windowLose.GetComponent<Animator>().Play("WindowAppearFromAbove");
                GameStats.SaveStats();
                ScoreChain.instance.Display(false);
                createdWindow = true;
            }

            if (GameStats.level <= 2 && Player.hasInput == false && Time.timeScale != 0.0f)
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
                GameplayManager.instance.RevivePlayer();
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

    public void TogglePause()
    {
        if (pauseButtonSound != null)
        {
            Instantiate(pauseButtonSound, transform.position, transform.rotation);
        }

        if (Time.timeScale != 0.0f)
        {
            pauseText.gameObject.SetActive(true);
            _pauseButtonImage.sprite = pauseButtonPressed;
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0.0f;
        }
        else
        {
            pauseText.gameObject.SetActive(false);
            _pauseButtonImage.sprite = pauseButtonDefault;
            Time.timeScale = previousTimeScale;
        }
    }
}
