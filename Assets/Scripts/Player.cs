using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float moveSpeed;
    public GameObject projectile;
    public GameObject attackSound;
    public GameObject projectileAuto;
    public GameObject attackAutoSound;
    public GameObject finishLine;
    public bool ignoreInput = false;
    public SpriteRenderer highlightSprite;

    public static int startHealth = 2;
    public static float firingSpeedDivider = 1.0f;
    public static float moveSpeedMultiplier = 1.0f;
    public static float projectileSpeedMultiplier = 1.0f;
    public static int shootLevel = 1;
    public static int projectileDamage = 1;
    public static int projectilePerforation = 1;
    public static int projectileAutoDamage = 0;
    public static bool isDead = false;
    public static bool victory = false;

    public static bool hasInput = false;
    public static Player instance = null;

    [HideInInspector] public Health healthScript;
    [HideInInspector] public int[] conditionTimers;

    private bool _detectedVictory = false;
    private float _inputDuration = 0;
    private Vector2 _targetPosition;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        Time.timeScale = 1.0f;
        instance = this;
        conditionTimers = new int[GameConstants.PLAYER_COND_AMOUNT];
        ResetConditions();
    }

    void Start()
    {
        isDead = false;
        victory = false;
        _detectedVictory = false;
        _inputDuration = 0;
        GameStats.currentLevelPoints = 0;
        InitializePlayer(0);

        // Fail-safe. Don't let the player do 0 or negative damage.
        if (projectileDamage < 1)
        {
            projectileDamage = 1;
        }

        // Hide highlight sprite if is ignoring input.
        if (highlightSprite != null && ignoreInput == true)
        {
            highlightSprite.enabled = false;
        }
    }

    void Update()
    {
        // Detect mouse or touch input.
        if (Input.GetMouseButton(0))
        {
            _targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.touchCount > 0)
        {
            _targetPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
        }

        // Prevent player from going outside of the screen.
        _targetPosition = new Vector2(Mathf.Clamp(_targetPosition.x, -2.5f, 2.5f), Mathf.Clamp(_targetPosition.y, -5.0f, 5.0f));

        if (Input.GetMouseButton(0) || Input.touchCount > 0)
        {
            hasInput = true;

            if (_inputDuration < float.MaxValue - 1.0f)
            {
                _inputDuration += Time.deltaTime;
            }
        }
        else
        {
            hasInput = false;
            _inputDuration = 0;
        }

        // Move towards input.
        if (HasMovementConditions())
        {
            transform.position = Vector2.MoveTowards(transform.position, _targetPosition, Time.deltaTime * moveSpeed * moveSpeedMultiplier);
        }

        // Show or hide highlight sprite.
        if (highlightSprite != null)
        {
            highlightSprite.enabled = HasMovementConditions();
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        // Debug inputs.
        if (Debug.isDebugBuild)
        {
            if (Input.GetKeyDown(KeyCode.M) && ignoreInput == false)
            {
                SceneManager.LoadScene("StartScene");
            }
            if (Input.GetKeyDown(KeyCode.T) && ignoreInput == false)
            {
                Time.timeScale = Time.timeScale == 1.0f ? 0.2f : 1.0f;
            }
            if (Input.GetKeyDown(KeyCode.K) && ignoreInput == false)
            {
                GameStats.enemiesKilledTotal += 1000;
                GameStats.SaveStats();
            }
            if (Input.GetKeyDown(KeyCode.L) && ignoreInput == false)
            {
                GameStats.enemiesKilledTotal = 0;
                GameStats.SaveStats();
            }
        }
#endif
    }

    private IEnumerator Shoot()
    {
        yield return new WaitForSeconds(0.6f / firingSpeedDivider);

        // Create shot if touching screen.
        if (hasInput && ignoreInput == false)
        {
            float shotScale = 1.0f;

            if (conditionTimers[0] > 0)
            {
                shotScale *= 3.0f;
            }

            Combat.CreateShot(projectile, transform, 0, gameObject, _spriteRenderer.color, projectileSpeedMultiplier, projectileDamage, projectilePerforation, shotScale);

            if (shootLevel > 1)
            {
                for (int i = 1; i < Mathf.RoundToInt((shootLevel - 1) / 2) + 1; i++)
                {
                    Combat.CreateShot(projectile, transform, 4 * i, gameObject, _spriteRenderer.color, projectileSpeedMultiplier, projectileDamage, projectilePerforation, shotScale);
                }
                for (int i = 1; i < Mathf.RoundToInt((shootLevel - 1) / 2) + 1; i++)
                {
                    Combat.CreateShot(projectile, transform, -4 * i, gameObject, _spriteRenderer.color, projectileSpeedMultiplier, projectileDamage, projectilePerforation, shotScale);
                }
            }

            if (attackSound != null)
            {
                Instantiate(attackSound, transform.position, transform.rotation);
            }
        }

        // Killed all enemies.
        if (GameObject.FindGameObjectsWithTag("Enemy").Length <= 0 && victory == false && _detectedVictory == false && ignoreInput == false)
        {
            Instantiate(finishLine, new Vector3(-3, 7, 0), Quaternion.Euler(0, 0, 0));

            MovingObject[] bgs = FindObjectsOfType<MovingObject>();

            foreach (MovingObject bg in bgs)
            {
                bg.speed *= 5;
            }

            _detectedVictory = true;
        }

        StartCoroutine(Shoot());
    }

    private IEnumerator ShootAuto()
    {
        yield return new WaitForSeconds(1.8f);

        float shotScale = 1.0f;

        if (conditionTimers[0] > 0)
        {
            shotScale *= 3.0f;
        }

        if (projectileAutoDamage > 0 && GameObject.FindGameObjectsWithTag("Enemy").Length > 0 && victory == false && ignoreInput == false && ClosestEnemy(true) != null)
        {
            Combat.CreateShot(projectileAuto, transform, Combat.AimDirection(ClosestEnemy(false).transform, transform), gameObject, _spriteRenderer.color, -1f, projectileAutoDamage, shotScale);
            
            if (attackAutoSound != null)
            {
                Instantiate(attackAutoSound, transform.position, transform.rotation);
            }
        }

        StartCoroutine(ShootAuto());
    }

    private IEnumerator ConditionTimers()
    {
        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < conditionTimers.Length; i++)
        {
            if (conditionTimers[i] > 0)
            {
                conditionTimers[i] -= 1;
            }
        }

        StartCoroutine(ConditionTimers());
    }

    public void InitializePlayer(float invincibilityTime)
    {
        _targetPosition = transform.position;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        healthScript = GetComponent<Health>();
        healthScript.health = startHealth;
        isDead = false;
        victory = false;

        StopAllCoroutines();
        StartCoroutine(Shoot());
        StartCoroutine(ShootAuto());
        ResetConditions();
        StartCoroutine(ConditionTimers());
        GameStats.SaveStats();

        if (invincibilityTime > 0)
        {
            StartCoroutine(healthScript.Invincibility(invincibilityTime));
        }
    }

    public void ResetConditions()
    {
        for (int i = 0; i < conditionTimers.Length; i++)
        {
            conditionTimers[i] = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            victory = true;
            ignoreInput = true;
            Combat.DestroyAllProjectiles();

            if (GameStats.currentLevelType == GameStats.LevelType.PRESET)
            {
                GameStats.completedExtraLevels[PresetLevels.currentPresetLevel] = 1;
            }

            GameStats.SaveStats();

            // Show level end ad.
            if (AdInterstitialManager.instance != null && PurchaseManager.instance.HasRemovedAds() == false)
            {
                AdInterstitialManager.instance.ShowAd();
            }
        }
        else if (collision.gameObject.CompareTag("Item"))
        {
            // Collecting an item.
            Item itemScript = collision.gameObject.GetComponent<Item>();

            // Give points.
            if (itemScript.givePoints > 0)
            {
                GameStats.AddPoints(itemScript.givePoints);
                PersistentCanvas.reference.CreateNumberChangeEffect(HUD.hudTopLeftCorner, "+" + itemScript.givePoints.ToString(), Color.green, -0.55f, 0.25f);
            }

            // Give health.
            if (itemScript.giveHealth > 0)
            {
                healthScript.health += itemScript.giveHealth;
                PersistentCanvas.reference.CreateNumberChangeEffect(HUD.hudBottomRightCorner, "+" + itemScript.giveHealth.ToString(), Color.green, 0.55f, 0.25f);
            }

            // Give condition.
            if (itemScript.giveCondition > -1)
            {
                if (conditionTimers[itemScript.giveCondition] < itemScript.giveConditionTime)
                {
                    conditionTimers[itemScript.giveCondition] = itemScript.giveConditionTime;
                }
            }

            foreach (GameObject obj in itemScript.createOnCollect)
            {
                if (obj != null)
                {
                    Instantiate(obj, transform.position, transform.rotation);
                }
            }

            // Destroy the item.
            Destroy(collision.gameObject);
        }
    }

    private bool IsTouchingButton()
    {
        if (HUD.instance != null)
        {
            return _targetPosition.x > HUD.instance.pauseButtonWorldPosition.x - 2.1f && _targetPosition.x < HUD.instance.pauseButtonWorldPosition.x + 2.1f && _targetPosition.y > HUD.instance.pauseButtonWorldPosition.y - 2.1f && _targetPosition.y < HUD.instance.pauseButtonWorldPosition.y + 2.1f;
        }
        else
        {
            return false;
        }
    }

    private bool HasMovementConditions()
    {
        return hasInput == true && ignoreInput == false && Time.timeScale != 0.0f && victory == false && isDead == false && (!IsTouchingButton() || _inputDuration > 0.5f);
    }

    public Enemy ClosestEnemy(bool onScreenOnly)
    {
        GameObject[] enemyList = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject toReturn = null;
        float shortestDistance = float.MaxValue;

        for (int i = 0; i < enemyList.Length; i++)
        {
            float distance = Vector2.Distance(enemyList[i].transform.position, gameObject.transform.position);

            if (distance < shortestDistance && (enemyList[i].transform.position.y < 5 || onScreenOnly == false))
            {
                shortestDistance = distance;
                toReturn = enemyList[i];
            }
        }

        if (toReturn != null)
        {
            if (toReturn.GetComponent<Enemy>() != null)
            {
                return toReturn.GetComponent<Enemy>();
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }
}
