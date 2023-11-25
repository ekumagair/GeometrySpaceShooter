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
    public float shootY = 6f;
    public float stopY = -100f;
    public bool canBeMovedY = true;
    public SpriteRenderer outline;
    public GameObject unavoidableExplosion;
    public GameObject warningBlipSound;
    public GameObject attackSound;

    private SpriteRenderer _sr;
    private Health _healthScript;
    private GameObject _player;

    public enum AttackType
    {
        None,
        ShootForward,
        ShootAtPlayer,
        SixShots,
        ShootAtRandomAngle,
        FiveShotsAtPlayer
    }
    public AttackType attackType;

    // Enemy difficulties:
    // 1 = Very easy.
    // 2 = Easy.
    // 3 = Normal.
    // 4 = Hard.
    // 5 = Very hard.
    // 6 = Ultra hard.
    // 7 = Extreme.

    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        _healthScript = GetComponent<Health>();
        _player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(Attack());
        StartCoroutine(OutlineFlash());

        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    void Update()
    {
        if (Player.isDead == false && transform.position.y > stopY && Time.timeScale > 0.0f)
        {
            transform.Translate(-transform.up * moveYSpeed * Time.deltaTime);
        }
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(firingSpeed);

        if (transform.position.y < shootY && Player.isDead == false)
        {
            switch (attackType)
            {
                case AttackType.None:
                    break;

                case AttackType.ShootForward:
                    Combat.CreateShot(projectile, transform, 90, gameObject, _sr.color, projectileSpeedMultiplier, projectileDamage);
                    break;

                case AttackType.ShootAtPlayer:
                    Combat.CreateShot(projectile, transform, Combat.AimDirection(_player.transform, transform), gameObject, _sr.color, -projectileSpeedMultiplier, projectileDamage);
                    break;

                case AttackType.SixShots:
                    Combat.CreateShot(projectile, transform, 0, gameObject, _sr.color, projectileSpeedMultiplier, projectileDamage);
                    Combat.CreateShot(projectile, transform, -22.5f, gameObject, _sr.color, projectileSpeedMultiplier, projectileDamage);
                    Combat.CreateShot(projectile, transform, 22.5f, gameObject, _sr.color, projectileSpeedMultiplier, projectileDamage);
                    Combat.CreateShot(projectile, transform, -67.5f, gameObject, _sr.color, projectileSpeedMultiplier, projectileDamage);
                    Combat.CreateShot(projectile, transform, 67.5f, gameObject, _sr.color, projectileSpeedMultiplier, projectileDamage);
                    Combat.CreateShot(projectile, transform, 90, gameObject, _sr.color, projectileSpeedMultiplier, projectileDamage);
                    break;

                case AttackType.ShootAtRandomAngle:
                    Combat.CreateShot(projectile, transform, Random.Range(-90f, 90f), gameObject, _sr.color, projectileSpeedMultiplier, projectileDamage);
                    break;

                case AttackType.FiveShotsAtPlayer:
                    Combat.CreateShot(projectile, transform, Combat.AimDirection(_player.transform, transform) + 4f, gameObject, _sr.color, -projectileSpeedMultiplier, projectileDamage);
                    Combat.CreateShot(projectile, transform, Combat.AimDirection(_player.transform, transform) + 2f, gameObject, _sr.color, -projectileSpeedMultiplier, projectileDamage);
                    Combat.CreateShot(projectile, transform, Combat.AimDirection(_player.transform, transform), gameObject, _sr.color, -projectileSpeedMultiplier, projectileDamage);
                    Combat.CreateShot(projectile, transform, Combat.AimDirection(_player.transform, transform) - 2f, gameObject, _sr.color, -projectileSpeedMultiplier, projectileDamage);
                    Combat.CreateShot(projectile, transform, Combat.AimDirection(_player.transform, transform) - 4f, gameObject, _sr.color, -projectileSpeedMultiplier, projectileDamage);
                    break;

                default:
                    break;
            }

            if (attackSound != null)
            {
                Instantiate(attackSound, transform.position, transform.rotation);
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
        yield return new WaitForSeconds(0.15f);

        if (transform.position.y < -3 && outline != null)
        {
            outline.enabled = !outline.enabled;

            if (warningBlipSound != null && Player.isDead == false && Player.victory == false)
            {
                Instantiate(warningBlipSound, transform.position, transform.rotation);
            }
        }
        if (transform.position.y < -5)
        {
            CreateUnavoidableExplosion();
        }

        StartCoroutine(OutlineFlash());
    }

    void CreateUnavoidableExplosion()
    {
        Instantiate(unavoidableExplosion, transform.position, Quaternion.Euler(-90, 0, 0));

        if (projectileDamage < 1)
        {
            projectileDamage = 1;
        }
        if (attackType == AttackType.FiveShotsAtPlayer)
        {
            projectileDamage *= 5;
        }
        _player.GetComponent<Health>().TakeDamage(projectileDamage);

        _healthScript.moveEnemiesOnDeath = 0;
        _healthScript.pointsOnDeath = 0;
        _healthScript.Die(false);
    }
}
