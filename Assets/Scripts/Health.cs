using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int health = 2;
    public int pointsOnDeath = 0;
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

    SpriteRenderer _sr;
    PersistentCanvas persistentCanvas;

    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        persistentCanvas = GameObject.Find("PersistentCanvas").GetComponent<PersistentCanvas>();
        invincible = false;
    }

    public void TakeDamage(int amount)
    {
        if(damageEffect != null)
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
        if(damageSound != null)
        {
            Instantiate(damageSound, transform.position, transform.rotation);
        }
        if (persistentCanvas != null && createDamageChangeText == true)
        {
            // Show lost health on the bottom right corner of the screen.
            persistentCanvas.CreateNumberChangeEffect(new Vector3(100, -190, 0), "-" + amount.ToString(), new Color(1f, 0.5f, 0.5f), 1, 1);
        }

        health -= amount;

        if(health < 0)
        {
            health = 0;
        }
        if(health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        GameStats.AddPoints(pointsOnDeath);
        invincible = false;

        if ((moveEnemiesOnDeath < 0 && transform.position.y > 3) || moveEnemiesOnDeath > 0)
        {
            LevelGenerator.MoveEnemies(moveEnemiesOnDeath);
        }
        else if (moveEnemiesOnDeath < 0 && transform.position.y < -1)
        {
            LevelGenerator.MoveEnemies(moveEnemiesOnDeath * -2);
        }

        if(deathParticles != null)
        {
            var deathEffect = Instantiate(deathParticles, transform.position, transform.rotation);
            deathEffect.transform.rotation = Quaternion.Euler(-90, 0, 0);
            ParticleSystem.MainModule deathEffectMainModule = deathEffect.GetComponent<ParticleSystem>().main;
            deathEffectMainModule.startColor = _sr.color;
        }
        if(persistentCanvas != null && pointsOnDeath != 0 && createDeathChangeText == true)
        {
            // Show gained points on the top left corner of the screen.
            persistentCanvas.CreateNumberChangeEffect(new Vector3(-85, 250, 0), "+" + pointsOnDeath.ToString(), Color.white, -0.5f, 1);
        }

        switch (deathType)
        {
            case DeathType.Destroy:
                Destroy(gameObject);
                break;

            case DeathType.Deactivate:
                if(GetComponent<Player>() != null)
                {
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
        invincible = true;

        yield return new WaitForSeconds(time);

        invincible = false;
    }
}
