using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100.0f; // Total health of the player
    private bool isDead = false; // Track if the player is dead
    private bool isInvincible = false; // Track if the player is currently invincible
    public float invincibilityDuration = 0.3f; // Duration of invincibility after taking damage
    private Scene currScene;

    // Maximum health for the player
    public float maxHealth = 100.0f;

    private void Awake()
    {
        currScene = SceneManager.GetActiveScene();
    }

    // Public method to check if the player is dead
    public bool IsDead()
    {
        return isDead;
    }

    public void TakeDamage(float damage)
    {
        if (isDead || isInvincible) return;

        health -= damage;
        Debug.Log("Player took damage: " + damage + ". Remaining health: " + health);

        StartCoroutine(InvincibilityCoroutine());

        if (health <= 0)
        {
            Die();
        }
    }

    public void RestoreHealth(float amount)
    {
        if (isDead)
        {
            Debug.LogWarning("Cannot restore health. Player is dead.");
            return;
        }

        float oldHealth = health;
        health = Mathf.Min(health + amount, maxHealth);
        Debug.Log($"Restored {health - oldHealth} health. Current health: {health}");
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Player has died.");
        if(currScene.name == "MainBoss")
        {
            SceneManager.LoadScene("CSC_Ending");
        }
        else
        {
            SceneManager.LoadScene("Main Menu");
        }
    }

    public void ResetHealth()
    {
        health = maxHealth;
        isDead = false;
        Debug.Log("Player health reset. Current health: " + health);
    }
}
