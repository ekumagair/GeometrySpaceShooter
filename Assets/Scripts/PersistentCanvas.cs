using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PersistentCanvas : MonoBehaviour
{
    public TMP_Text pointsNumber;
    public GameObject[] soundButton;
    public GameObject numberChangeEffect;
    public GameObject fadeOutOverlay;
    public TMP_Text levelText;

    public static PersistentCanvas reference;

    private void Awake()
    {
        reference = this;
    }

    void Start()
    {
        if (GameStats.currentLevelType == GameStats.LevelType.PRESET)
        {
            levelText.enabled = false;
        }
    }

    void Update()
    {
        pointsNumber.text = GameStats.points.ToString();

#if UNITY_EDITOR || UNITY_STANDALONE
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
            if (Input.GetKeyDown(KeyCode.D))
            {
                GameStats.points -= 12345678;
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                GameStats.points *= GameStats.points;
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                GameplayManager.instance.NextLevel();
                GameplayManager.instance.GoToStartScene();
            }
        }
#endif
    }

    public void CreateButtonSound(int b)
    {
        if (soundButton[b] != null)
        {
            Instantiate(soundButton[b], transform.position, transform.rotation);
        }
    }

    public void CreateNumberChangeEffect(Vector3 pos, string textString, Color textColor, float speedYMultiplier, float fadeOutMultiplier)
    {
        GameObject nc = Instantiate(numberChangeEffect, pos, transform.rotation, transform);
        nc.GetComponent<TMP_Text>().text = textString;
        nc.GetComponent<TMP_Text>().color = textColor;
        nc.GetComponent<NumberChangeText>().moveYSpeed *= speedYMultiplier;
        nc.GetComponent<NumberChangeText>().fadeOutSpeed *= fadeOutMultiplier;
        nc.GetComponent<RectTransform>().anchoredPosition = pos;
    }

    public void CreateFadeOutOverlay()
    {
        Instantiate(fadeOutOverlay, transform.position, transform.rotation, transform);
    }
}
