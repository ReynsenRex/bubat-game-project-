using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWHitbox : MonoBehaviour
{
    public float damageAmount = 10.0f; // Amount of damage to apply

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to an enemy
        if (other.CompareTag("Enemy"))
        {
            // Try to get the EnemyHealth component
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageAmount);
                Debug.Log("Enemy hit by player hitbox (EnemyHealth), damage applied: " + damageAmount);
            }

            // Try to get the EnemyHealthNormies component
            EnemyHealthNormies enemyHealthNormies = other.GetComponent<EnemyHealthNormies>();
            if (enemyHealthNormies != null)
            {
                enemyHealthNormies.TakeDamage(damageAmount);
                Debug.Log("Enemy hit by player hitbox (EnemyHealthNormies), damage applied: " + damageAmount);
            }
        }
    }

    public void DeactivateHitbox()
    {
        gameObject.SetActive(false);
    }

    public void ReactivateHitbox()
    {
        gameObject.SetActive(true);
    }
}