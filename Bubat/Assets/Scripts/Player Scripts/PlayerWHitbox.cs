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
	        }
	
	        // Try to get the EnemyHealthNormies component
	        EnemyHealthNormies enemyHealthNormies = other.GetComponent<EnemyHealthNormies>();
	        if (enemyHealthNormies != null)
	        {
	            enemyHealthNormies.TakeDamage(damageAmount);
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