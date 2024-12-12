using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCHealth : MonoBehaviour
{

	// Start is called before the first frame update
	public float health = 100.0f; // Total health of the enemy
	private bool isDead = false; // Track if the enemy is dead

	// Method to apply damage to the enemy
	public void TakeDamage(float damage)
	{
		if (isDead) return;

		health -= damage; // Subtract damage from health
		Debug.Log("Putri took damage: " + damage + ". Remaining health: " + health);


		if (health <= 0)
		{
			Die(); // Call the Die method if health is depleted
            SceneManager.LoadScene("MainMenu");
        }
	}


	// Method to handle enemy death
	private void Die()
	{
		isDead = true; // Set the enemy as dead
		Debug.Log("Putri has died.");
		
		// Destroy the enemy GameObject
		Destroy(gameObject);
		SceneManager.LoadScene("MainMenu");
	}

}
