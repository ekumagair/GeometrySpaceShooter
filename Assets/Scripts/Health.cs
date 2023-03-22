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

    public enum DeathType
    {
        Destroy,
        Deactivate
    }
    public DeathType deathType;

    SpriteRenderer _sr;

    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
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
}
