using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    public float moveYSpeed;
    public float firingSpeed = 1f;
    public GameObject projectile;
    public float projectileSpeedMultiplier = 1.0f;
    public int projectileDamage = 1;
    public SpriteRenderer outline;
    public GameObject unavoidableExplosion;

    SpriteRenderer spriteRenderer;
    GameObject player;

    public enum AttackType
    {
        None,
        ShootForward,
        ShootAtPlayer,
        SixShots,
        ShootAtRandomAngle
    }
    public AttackType attackType;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        outline.enabled = false;
        StartCoroutine(Attack());
        StartCoroutine(OutlineFlash());
    }

    void Update()
    {
        if (Player.isDead == false)
        {
            transform.Translate(-transform.up * moveYSpeed * Time.deltaTime);
        }
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(firingSpeed);

        if (transform.position.y < 6f && Player.isDead == false)
        {
            switch (attackType)
            {
                case AttackType.None:
                    break;

                case AttackType.ShootForward:
                    Combat.CreateShot(projectile, transform, 90, gameObject, spriteRenderer.color, projectileSpeedMultiplier, projectileDamage);
                    break;

                case AttackType.ShootAtPlayer:
                    Combat.CreateShot(projectile, transform, Combat.AimDirection(player.transform, transform) / 2, gameObject, spriteRenderer.color, -projectileSpeedMultiplier, projectileDamage);
                    break;

                case AttackType.SixShots:
                    Combat.CreateShot(projectile, transform, 0, gameObject, spriteRenderer.color, projectileSpeedMultiplier, projectileDamage);
                    Combat.CreateShot(projectile, transform, -22.5f, gameObject, spriteRenderer.color, projectileSpeedMultiplier, projectileDamage);
                    Combat.CreateShot(projectile, transform, 22.5f, gameObject, spriteRenderer.color, projectileSpeedMultiplier, projectileDamage);
                    Combat.CreateShot(projectile, transform, -67.5f, gameObject, spriteRenderer.color, projectileSpeedMultiplier, projectileDamage);
                    Combat.CreateShot(projectile, transform, 67.5f, gameObject, spriteRenderer.color, projectileSpeedMultiplier, projectileDamage);
                    Combat.CreateShot(projectile, transform, 90, gameObject, spriteRenderer.color, projectileSpeedMultiplier, projectileDamage);
                    break;

                case AttackType.ShootAtRandomAngle:
                    Combat.CreateShot(projectile, transform, Random.Range(-90f, 90f), gameObject, spriteRenderer.color, projectileSpeedMultiplier, projectileDamage);
                    break;

                default:
                    break;
            }
        }
        else
        {
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        }

        StartCoroutine(Attack());
    }

    IEnumerator OutlineFlash()
    {
        yield return new WaitForSeconds(0.1f);

        if(transform.position.y < -3)
        {
            outline.enabled = !outline.enabled;
        }
        if(transform.position.y < -5)
        {
            CreateUnavoidableExplosion();
        }

        StartCoroutine(OutlineFlash());
    }

    void CreateUnavoidableExplosion()
    {
        Instantiate(unavoidableExplosion, transform.position, Quaternion.Euler(-90, 0, 0));
        player.GetComponent<Health>().TakeDamage(1);
        Destroy(gameObject);
    }
}
