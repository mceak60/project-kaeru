using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int health; // Current health
    public int numHearts; // Maximum health, or the number of heart containers the player currently has

    public Image[] hearts; // Array of heart containers
    public Sprite fullHeart; // Sprite for full heart
    public Sprite emptyHeart; // Sprite for empty heart

    public PlayerController controller; // Reference to the PlayerController script, only used to set the knockback property

    public float startIFrames; // Amount of time the player has invincibility frames
    private float iFrames; // Used for counting down I-frames
    private bool invincible = false; // Whether the player is currently invincible

    public float startKnockbackTime; // Amount of time the player takes knockabck
    private float knockbackTime; // Used for counting knockbackTime
    public float knockbackAmount; // The force we use to knock back the player
    private bool knockback = false; // Whether the player is currently taking knockback

    public SpriteRenderer playerSprite; // Reference to the player sprite
    public float startFlashTime; // How long each individual flash lasts (a flash is basically changing the sprite color)
    private float flashTime; // Used for the flash clock
    private Color defaultColor; // Should be white, or Color(1,1,1,1)

    private Collider2D lastCollision;

    private void Start()
    {
        iFrames = startIFrames;
        knockbackTime = startKnockbackTime;
        flashTime = startFlashTime;
        defaultColor = playerSprite.color;
    }

    private void Update()
    {
        DisplayHearts();
        UpdateInvincibility();
        UpdateKnockback();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            GetComponent<PlayerDeath>().Die();
        }
    }

    /*
    // If the player collides with the enemy's hitbox, apply damage and knockback
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!invincible)
        {
            if (col.gameObject.CompareTag("Enemy"))
            {
                lastCollision = col;
                invincible = true;
                knockback = true;
                controller.knockback = true;
                TakeDamage(1);
                //Debug.Log("Invincible");
                playerSprite.color = Color.gray;
            }
        }
    }

    // If the player stays in the enemy's hitbox after their invincibility runs out, reapply damage and knockback
    private void OnTriggerStay2D(Collider2D col)
    {
        if (!invincible)
        {
            if (col.gameObject.CompareTag("Enemy"))
            {
                lastCollision = col;
                invincible = true;
                knockback = true;
                controller.knockback = true;
                TakeDamage(1);
                //Debug.Log(col);
                playerSprite.color = Color.gray;
            }
        }
    }
    */

    //Display heart containers and current health
    private void DisplayHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (health > numHearts)
                health = numHearts;

            if (i < health)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;

            if (i < numHearts)
                hearts[i].enabled = true;
            else
                hearts[i].enabled = false;
        }
    }

    private void UpdateInvincibility()
    {
        //Check to see if invincibility has run out
        if (invincible)
        {
            if (iFrames <= 0)
            {
                //Debug.Log("Not invincible");
                invincible = false;
                iFrames = startIFrames;
                playerSprite.color = defaultColor;
            }
            else
                iFrames -= Time.deltaTime;

            // Make the sprite flash while player is invincible
            if (flashTime < 0)
            {
                if (playerSprite.color == defaultColor)
                    playerSprite.color = Color.gray;
                else
                    playerSprite.color = defaultColor;

                flashTime = startFlashTime;
            }
            else
                flashTime -= Time.deltaTime;
        }
    }

    private void UpdateKnockback()
    {
        if (knockback)
        {
            if (knockbackTime <= 0)
            {
                // Stop applying knockback
                controller.knockback = false;
                knockback = false;
                knockbackTime = startKnockbackTime;
            }
            else
            {
                // Apply knockback
                Rigidbody2D playerRB = GetComponent<Rigidbody2D>();
                Vector2 moveDirection = playerRB.transform.position - lastCollision.transform.position;
                if(moveDirection.x <= 0)
                    playerRB.velocity = new Vector2(-1, 0.35f) * knockbackAmount;
                else
                    playerRB.velocity = new Vector2(1, 0.35f) * knockbackAmount;

                knockbackTime -= Time.deltaTime;
            }
        }
    }

    public void MakeInvincible()
    {
        invincible = true;
    }

    public bool GetInvincible()
    {
        return invincible;
    }

    public void SetKnockback(bool b)
    {
        knockback = b;
    }

    public void SetCollision(Collider2D col)
    {
        lastCollision = col;
    }
}