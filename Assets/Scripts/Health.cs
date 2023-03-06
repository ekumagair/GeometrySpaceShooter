using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int health = 2;
    public int pointsOnDeath = 0;
    public float moveEnemiesOnDeath;

    public enum DeathType
    {
        Destroy,
        Deactivate
    }
    public DeathType deathType;

    public void TakeDamage(int amount)
    {
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
        LevelGenerator.MoveEnemies(moveEnemiesOnDeath);

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
