using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWHitbox : MonoBehaviour
{
    public float damageAmount = 10.0f; // Amount of damage to apply

    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter called with: " + other.name);

        // Check if the collider belongs to an enemy
        if (other.CompareTag("Enemy"))
    {
        Debug.Log("Enemy detected: " + other.name);

        // Try to get the EnemyMovementNormies component
        EnemyMovementNormies enemyMovement = other.GetComponent<EnemyMovementNormies>();
        if (enemyMovement != null)
        {
            enemyMovement.TakeDamage(); // Call the TakeDamage method to trigger hit logic
            Debug.Log("Enemy hit by player hitbox, damage applied: " + damageAmount);
        }

        // Try to get the EnemyHealth component
        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damageAmount);
            if (this.CompareTag("PlayerHitbox"))
            {
                audioManager.PlaySFX(audioManager.enemyHurt);
            }
            Debug.Log("EnemyHealth component found, damage applied: " + damageAmount);
        }

        // Try to get the EnemyHealthNormies component
        EnemyHealthNormies enemyHealthNormies = other.GetComponent<EnemyHealthNormies>();
        if (enemyHealthNormies != null)
        {
            enemyHealthNormies.TakeDamage(damageAmount);
            if (this.CompareTag("PlayerHitbox"))
            {
                audioManager.PlaySFX(audioManager.enemyHurt);
            }
            Debug.Log("EnemyHealthNormies component found, damage applied: " + damageAmount);
        }
        }
    }

    public IEnumerator ApplyDamageBuff()
    {
        Debug.Log("ApplyDamageBuff called");
        float originalDamage = damageAmount;
        damageAmount = 15f; // Example: 50% damage increase
        Debug.Log("Damage buff applied, new damage amount: " + damageAmount);

        yield return new WaitForSeconds(10.0f); // Buff duration: 10 seconds

        damageAmount = originalDamage; // Revert the buff
        Debug.Log("Damage buff reverted, original damage amount: " + damageAmount);
    }

    public void DeactivateHitbox()
    {
        Debug.Log("Hitbox deactivated");
        gameObject.SetActive(false);
    }

    public void ReactivateHitbox()
    {
        Debug.Log("Hitbox reactivated");
        gameObject.SetActive(true);
    }


}