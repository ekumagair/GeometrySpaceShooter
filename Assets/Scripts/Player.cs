using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float moveSpeed;
    public GameObject projectile;
    public GameObject finishLine;
    public bool ignoreInput = false;

    public static int startHealth = 2;
    public static float firingSpeedDivider = 1.0f;
    public static float moveSpeedMultiplier = 1.0f;
    public static float projectileSpeedMultiplier = 1.0f;
    public static int shootLevel = 1;
    public static int projectileDamage = 1;
    public static bool isDead = false;
    public static bool victory = false;

    bool hasInput = false;
    bool detectedVictory = false;
    Vector2 targetPosition;
    SpriteRenderer spriteRenderer;
    Health healthScript;

    void Start()
    {
        GameStats.currentLevelPoints = 0;
        InitializePlayer();

        // Fail-safe. Don't let the player do 0 or negative damage.
        if(projectileDamage < 1)
        {
            projectileDamage = 1;
        }
    }

    void Update()
    {
        // Detect mouse or touch input.
        if(Input.GetMouseButton(0))
        {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if(Input.touchCount > 0)
        {
            targetPosition = Input.GetTouch(0).position;
        }

        // Prevent player from going outside of the screen.
        targetPosition = new Vector2(Mathf.Clamp(targetPosition.x, -2.5f, 2.5f), Mathf.Clamp(targetPosition.y, -5.0f, 5.0f));

        if (Input.GetMouseButton(0) || Input.touchCount > 0)
        {
            hasInput = true;
        }
        else
        {
            hasInput = false;
        }

        // Move towards input.
        if (hasInput && ignoreInput == false)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, Time.deltaTime * moveSpeed * moveSpeedMultiplier);
        }

        // Debug.
        if(Debug.isDebugBuild)
        {
            if(Input.GetKeyDown(KeyCode.M) && ignoreInput == false)
            {
                SceneManager.LoadScene("StartScene");
            }
        }
    }

    IEnumerator Shoot()
    {
        yield return new WaitForSeconds(0.6f / firingSpeedDivider);

        // Create shot if touching screen.
        if (hasInput && ignoreInput == false)
        {
            Combat.CreateShot(projectile, transform, 0, gameObject, spriteRenderer.color, projectileSpeedMultiplier, projectileDamage);

            if (shootLevel > 1)
            {
                for (int i = 1; i < Mathf.RoundToInt((shootLevel - 1) / 2) + 1; i++)
                {
                    Combat.CreateShot(projectile, transform, 4 * i, gameObject, spriteRenderer.color, projectileSpeedMultiplier, projectileDamage);
                }
                for (int i = 1; i < Mathf.RoundToInt((shootLevel - 1) / 2) + 1; i++)
                {
                    Combat.CreateShot(projectile, transform, -4 * i, gameObject, spriteRenderer.color, projectileSpeedMultiplier, projectileDamage);
                }
            }
        }

        // Killed all enemies.
        if (GameObject.FindGameObjectsWithTag("Enemy").Length <= 0 && victory == false && detectedVictory == false && ignoreInput == false)
        {
            Instantiate(finishLine, new Vector3(-3, 7, 0), Quaternion.Euler(0, 0, 0));

            Background[] bgs = FindObjectsOfType<Background>();

            foreach (Background bg in bgs)
            {
                bg.speed *= 3;
            }

            detectedVictory = true;
        }

        StartCoroutine(Shoot());
    }

    public void InitializePlayer()
    {
        targetPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        healthScript = GetComponent<Health>();
        healthScript.health = startHealth;
        isDead = false;
        victory = false;
        detectedVictory = false;

        StopAllCoroutines();
        StartCoroutine(Shoot());
        GameStats.SaveStats();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Finish"))
        {
            victory = true;
            ignoreInput = true;
        }
    }
}
