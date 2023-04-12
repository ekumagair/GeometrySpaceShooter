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
    public SpriteRenderer highlightSprite;
    public AdInterstitialManager adInterstitial;

    public static int startHealth = 2;
    public static float firingSpeedDivider = 1.0f;
    public static float moveSpeedMultiplier = 1.0f;
    public static float projectileSpeedMultiplier = 1.0f;
    public static int shootLevel = 1;
    public static int projectileDamage = 1;
    public static bool isDead = false;
    public static bool victory = false;

    public static bool hasInput = false;
    bool detectedVictory = false;
    Vector2 targetPosition;
    SpriteRenderer spriteRenderer;
    Health healthScript;
    PersistentCanvas persistentCanvas;

    void Start()
    {
        Time.timeScale = 1.0f;
        isDead = false;
        victory = false;
        detectedVictory = false;
        GameStats.currentLevelPoints = 0;
        persistentCanvas = GameObject.Find("PersistentCanvas").GetComponent<PersistentCanvas>();
        InitializePlayer(0);

        // Load level end ad.
        if (adInterstitial != null)
        {
            adInterstitial.LoadAd();
        }

        // Fail-safe. Don't let the player do 0 or negative damage.
        if (projectileDamage < 1)
        {
            projectileDamage = 1;
        }

        // Hide highlight sprite if is ignoring input.
        if(highlightSprite != null && ignoreInput == true)
        {
            highlightSprite.enabled = false;
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
            targetPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
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
        if (hasInput == true && ignoreInput == false)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, Time.deltaTime * moveSpeed * moveSpeedMultiplier);
        }

        // Show or hide highlight sprite.
        if(highlightSprite != null)
        {
            if (ignoreInput == false && victory == false && isDead == false && Time.timeScale != 0.0f)
            {
                highlightSprite.enabled = hasInput;
            }
            else
            {
                highlightSprite.enabled = false;
            }
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

            MovingObject[] bgs = FindObjectsOfType<MovingObject>();

            foreach (MovingObject bg in bgs)
            {
                bg.speed *= 5;
            }

            detectedVictory = true;
        }

        StartCoroutine(Shoot());
    }

    public void InitializePlayer(float invincibilityTime)
    {
        targetPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        healthScript = GetComponent<Health>();
        healthScript.health = startHealth;
        isDead = false;
        victory = false;

        StopAllCoroutines();
        StartCoroutine(Shoot());
        GameStats.SaveStats();

        if(invincibilityTime > 0)
        {
            StartCoroutine(healthScript.Invincibility(invincibilityTime));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Finish"))
        {
            victory = true;
            ignoreInput = true;
            GameStats.SaveStats();
            DestroyAllProjectiles();

            // Show level end ad.
            if(adInterstitial != null)
            {
                adInterstitial.ShowAd();
            }
        }
        else if (collision.gameObject.CompareTag("Item"))
        {
            Item itemScript = collision.gameObject.GetComponent<Item>();
            GameStats.AddPoints(itemScript.givePoints);
            persistentCanvas.CreateNumberChangeEffect(new Vector3(-85, 250, 0), "+" + itemScript.givePoints.ToString(), Color.green, -0.55f, 0.25f);

            foreach (GameObject obj in itemScript.createOnCollect)
            {
                if (obj != null)
                {
                    Instantiate(obj, transform.position, transform.rotation);
                }
            }

            Destroy(collision.gameObject);
        }
    }

    public static void DestroyAllProjectiles()
    {
        Projectile[] projectiles = GameObject.FindObjectsOfType<Projectile>();

        if(projectiles.Length > 0)
        {
            foreach (Projectile p in projectiles)
            {
                Destroy(p.gameObject);
            }
        }
    }
}
