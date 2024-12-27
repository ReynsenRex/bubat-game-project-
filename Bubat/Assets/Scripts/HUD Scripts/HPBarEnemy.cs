using UnityEngine;
using UnityEngine.UI;

public class HPBarEnemy : MonoBehaviour
{
    public Image enemyHPBar; // Health bar image
    public EnemyHealthNormies enemyHealthNormies; // Reference to the EnemyHealth script
    private float maxHP; // Maximum health of the enemy
    private float speed = 1; // Speed of health bar animation
    public Transform player; // Reference to the player's transform

    private void Start()
    {
        if (enemyHealthNormies != null)
        {
            maxHP = enemyHealthNormies.health;
        }
    }
    // Update is called once per frame
    void Update()
    {
        // Update the health bar based on the enemy's current health
        if (enemyHealthNormies != null && maxHP == 0)
        {
            maxHP = enemyHealthNormies.health; // Update maxHP if it changes dynamically
        }

        if (enemyHealthNormies != null)
        {
            UpdateHP(enemyHPBar, enemyHealthNormies.health, maxHP);
        }
        
        // Make the health bar face the player
        if (player != null)
        {
            FacePlayer();
        }
    }

    void UpdateHP(Image hpBar, float currentHP, float maxHP)
    {
        if (maxHP > 0)
        {
            float target = currentHP / maxHP; // Calculate health as a fraction
            hpBar.fillAmount = Mathf.MoveTowards(hpBar.fillAmount, target, speed * Time.deltaTime); // Smoothly update the health bar
        }
        
    }

    void FacePlayer()
    {
        // Rotate the health bar to face the player
        Vector3 direction = (player.position - transform.position).normalized; // Get the direction to the player
        Quaternion lookRotation = Quaternion.LookRotation(direction); // Create a rotation that looks at the player
        transform.rotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0); // Apply the rotation, keeping the health bar upright
    }
}
