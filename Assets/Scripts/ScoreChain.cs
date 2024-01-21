using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreChain : MonoBehaviour
{
    public TMP_Text multiplierText;
    public Image imageFill;
    public Image imageFillBg;
    public Image starIcon;
    public GameObject tierUpSound;

    public static ScoreChain instance;
    public static float scoreMultiplier;
    public static float scoreMultiplierMinimum = 1.0f;
    public static int tier;
    public static int currentKills;
    public static int killsForNextTier;

    private void Awake()
    {
        instance = this;
        ResetVariables();
    }

    private void Start()
    {
        ResetVariables();
    }

    private void Update()
    {
        scoreMultiplier = scoreMultiplierMinimum + (0.25f * tier);
        killsForNextTier = 5 + (2 * tier);

        if (scoreMultiplier < scoreMultiplierMinimum)
        {
            scoreMultiplier = scoreMultiplierMinimum;
        }
        if (tier < 0)
        {
            tier = 0;
        }

        if (currentKills >= killsForNextTier)
        {
            AddTier();
        }

        imageFill.fillAmount = (float)currentKills / (float)killsForNextTier;
        multiplierText.text = "x" + scoreMultiplier.ToString();
    }

    private void OnDestroy()
    {
        ResetVariables();
    }

    public void AddTier()
    {
        if (tierUpSound != null)
        {
            AudioObject snd = Instantiate(tierUpSound, transform).GetComponent<AudioObject>();
            snd.pitchMultMin = GetSoundPitchFromTier();
            snd.pitchMultMax = GetSoundPitchFromTier();
        }

        currentKills = 0;
        tier++;
    }

    public float GetSoundPitchFromTier()
    {
        return Mathf.Clamp(1.0f + (0.2f * tier), 1f, 3f);
    }

    public void RemoveTier()
    {
        currentKills = 0;

        if (tier > 0)
        {
            tier--;
        }
    }

    public void ResetVariables()
    {
        scoreMultiplier = scoreMultiplierMinimum;
        tier = 0;
        currentKills = 0;

        if (scoreMultiplier < 1.0f)
        {
            scoreMultiplier = 1.0f;
        }
        if (scoreMultiplierMinimum < 1.0f)
        {
            scoreMultiplierMinimum = 1.0f;
        }
    }

    public void Display(bool show)
    {
        multiplierText.enabled = show;
        starIcon.enabled = show;
        imageFill.enabled = show;
        imageFillBg.enabled = show;
    }
}
