using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StartScene : MonoBehaviour
{
    public GameObject[] canvases;
    public GameObject startObjects;
    public GameObject[] upgradePages;
    public int currentUpgradePage = 0;
    public Button upgradePreviousButton;
    public Button upgradeNextButton;
    public TMP_Text upgradePageText;
    public TMP_Text versionText;

    public static bool goToUpgrades = false;
    PersistentCanvas persistentCanvas;
    Camera mainCamera;

    void Awake()
    {
        // If has saved data, load the data and the prices for each upgrade.
        if (SaveSystem.SaveExists())
        {
            GameStats.LoadStats();

            Upgrade[] upgrades = FindObjectsOfType<Upgrade>();
            foreach (var upgrade in upgrades)
            {
                int gameStatsPrice = GameStats.upgradePrice[(int)upgrade.upgradeType];

                if (gameStatsPrice > 0)
                {
                    upgrade.price = gameStatsPrice;
                }
            }
        }
    }

    void Start()
    {
        Time.timeScale = 1.0f;
        persistentCanvas = GameObject.Find("PersistentCanvas").GetComponent<PersistentCanvas>();
        mainCamera = Camera.main;
        GameStats.currentLevelPoints = 0;
        GameStats.multipliedCurrentScore = false;

        if (GameStats.points < 0)
        {
            GameStats.points = 0;
        }

        if(goToUpgrades)
        {
            GoToUpgrades();
            persistentCanvas.CreateButtonSound();
            goToUpgrades = false;
        }
        else
        {
            GoToStart();
        }

        if(versionText != null)
        {
            versionText.text = "v" + Application.version.ToString();
        }
    }

    void Update()
    {
        for (int i = 0; i < upgradePages.Length; i++)
        {
            if(i == currentUpgradePage)
            {
                upgradePages[i].gameObject.SetActive(true);
            }
            else
            {
                upgradePages[i].gameObject.SetActive(false);
            }
        }

        if(currentUpgradePage == 0)
        {
            upgradePreviousButton.interactable = false;
        }
        else
        {
            upgradePreviousButton.interactable = true;
        }

        if (currentUpgradePage == upgradePages.Length - 1)
        {
            upgradeNextButton.interactable = false;
        }
        else
        {
            upgradeNextButton.interactable = true;
        }

        upgradePageText.text = (currentUpgradePage + 1).ToString() + "/" + upgradePages.Length.ToString();

        if (Debug.isDebugBuild)
        {
            if(Input.GetKeyDown(KeyCode.Delete))
            {
                SaveSystem.DeleteSave();
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
            }
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    void ChooseOneCanvas(int c)
    {
        for (int i = 0; i < canvases.Length; i++)
        {
            if(i == c)
            {
                canvases[i].gameObject.SetActive(true);
            }
            else
            {
                canvases[i].gameObject.SetActive(false);
            }
        }

        if (startObjects != null)
        {
            if (c == 0)
            {
                startObjects.SetActive(true);
            }
            else
            {
                startObjects.SetActive(false);
            }
        }
    }

    public void GoToStart()
    {
        //mainCamera.transform.position = new Vector3(0, 0, mainCamera.transform.position.z);
        ChooseOneCanvas(0);
    }

    public void GoToUpgrades()
    {
        //mainCamera.transform.position = new Vector3(10, 0, mainCamera.transform.position.z);
        ChooseOneCanvas(1);
    }

    public void GoToCredits()
    {
        //mainCamera.transform.position = new Vector3(20, 0, mainCamera.transform.position.z);
        ChooseOneCanvas(2);
    }

    public void GoToOptions()
    {
        //mainCamera.transform.position = new Vector3(30, 0, mainCamera.transform.position.z);
        ChooseOneCanvas(3);
    }

    public void ChangeUpgradePage(int increment)
    {
        currentUpgradePage += increment;
    }
}
