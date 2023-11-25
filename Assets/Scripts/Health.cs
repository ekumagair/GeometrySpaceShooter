using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int health = 2;
    public int pointsOnDeath = 0;
    public float afterHitInvincibleTime = 0;
    public float moveEnemiesOnDeath;
    public GameObject damageEffect;
    public GameObject damageSound;
    public GameObject deathParticles;
    public bool createDamageChangeText = false;
    public bool createDeathChangeText = true;
    public bool invincible = false;

    public enum DeathType
    {
        Destroy,
        Deactivate
    }
    public DeathType deathType;

    private SpriteRenderer _sr;
    private bool _dead = false;
    private Player _playerComponent = null;

    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        _dead = false;
        _playerComponent = GetComponent<Player>();
        invincible = false;
    }

    public void TakeDamage(int amount)
    {
        if (_dead == true)
        {
            return;
        }

        if (damageEffect != null)
        {
            var ifx = Instantiate(damageEffect, transform.position, transform.rotation);
            SpriteRenderer ifxSpriteRenderer = ifx.GetComponent<SpriteRenderer>();

            if (_sr != null && ifxSpriteRenderer != null)
            {
                ifxSpriteRenderer.color = _sr.color;
                ifxSpriteRenderer.flipX = _sr.flipX;
                ifxSpriteRenderer.flipY = _sr.flipY;
            }
        }
        if (damageSound != null)
        {
            Instantiate(damageSound, transform.position, transform.rotation);
        }
        if (PersistentCanvas.reference != null && createDamageChangeText == true)
        {
            // Show lost health on the bottom right corner of the screen.
            PersistentCanvas.reference.CreateNumberChangeEffect(HUD.hudBottomRightCorner, "-" + amount.ToString(), new Color(1f, 0.5f, 0.5f), 1, 1);
        }

        // Check tag.
        if (tag.Contains("Player"))
        {
            ScoreChain.instance.RemoveTier();
        }

        // Reduce health.
        health -= amount;

        if (health < 0)
        {
            health = 0;
        }
        if (health <= 0)
        {
            Die(true);
        }
        else if (afterHitInvincibleTime > 0 && invincible == false)
        {
            StartCoroutine(Invincibility(afterHitInvincibleTime));
        }
    }

    public void Die(bool givePoints)
    {
        if (_dead == true)
        {
            return;
        }

        _dead = true;

        // Give points.
        int pointsToGive = Mathf.RoundToInt(pointsOnDeath * ScoreChain.scoreMultiplier);
        if (givePoints == true)
        {
            if (Debug.isDebugBuild) { Debug.Log("Mult: " + ScoreChain.scoreMultiplier); }
            GameStats.AddPoints(pointsToGive);
        }

        // Check tag.
        if (tag.Contains("Enemy"))
        {
            ScoreChain.currentKills++;
        }

        invincible = false;

        if ((moveEnemiesOnDeath < 0 && transform.position.y > 3) || moveEnemiesOnDeath > 0)
        {
            LevelGenerator.MoveEnemies(moveEnemiesOnDeath);
        }
        else if (moveEnemiesOnDeath < 0 && transform.position.y < -1)
        {
            LevelGenerator.MoveEnemies(moveEnemiesOnDeath * -2);
        }

        if (deathParticles != null)
        {
            var deathEffect = Instantiate(deathParticles, transform.position, transform.rotation);
            deathEffect.transform.rotation = Quaternion.Euler(-90, 0, 0);
            ParticleSystem.MainModule deathEffectMainModule = deathEffect.GetComponent<ParticleSystem>().main;
            deathEffectMainModule.startColor = _sr.color;
        }
        if (PersistentCanvas.reference != null && pointsToGive != 0 && createDeathChangeText == true)
        {
            // Show gained points on the top left corner of the screen.
            PersistentCanvas.reference.CreateNumberChangeEffect(HUD.hudTopLeftCorner, "+" + pointsToGive.ToString(), Color.white, -0.5f, 1);
        }

        switch (deathType)
        {
            case DeathType.Destroy:
                Destroy(gameObject);
                break;

            case DeathType.Deactivate:
                if (_playerComponent != null)
                {
                    Player.instance.StopAllCoroutines();
                    Player.isDead = true;
                }
                gameObject.SetActive(false);
                break;

            default:
                break;
        }
    }

    public IEnumerator Invincibility(float time)
    {
        if (invincible == true)
        {
            yield break;
        }

        invincible = true;
        StartCoroutine(InvincibilityBlink());

        yield return new WaitForSeconds(time);

        invincible = false;
    }

    public IEnumerator InvincibilityBlink()
    {
        while (invincible == true)
        {
            _sr.enabled = !_sr.enabled;

            yield return new WaitForSeconds(0.1f);
        }

        _sr.enabled = true;
    }

    public void Revive()
    {
        _dead = false;
    }
}
