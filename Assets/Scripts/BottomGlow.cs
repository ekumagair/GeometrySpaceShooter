using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomGlow : MonoBehaviour
{
    bool touchingEnemy = false;
    float alpha = 0;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);

        if(touchingEnemy && alpha < 1)
        {
            alpha += Time.deltaTime;
        }
        if (!touchingEnemy && alpha > 0)
        {
            alpha -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            touchingEnemy = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            touchingEnemy = false;
        }
    }
}
