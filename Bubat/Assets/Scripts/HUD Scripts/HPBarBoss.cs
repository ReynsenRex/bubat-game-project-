using UnityEngine;
using UnityEngine.UI;

public class HPBarBoss : MonoBehaviour
{
    public Image enemyHPBar; // Health bar image
    public EnemyHealth enemyHealthBoss; // Reference to the EnemyHealth script
    private float maxHP = 50.0f; // Maximum health of the enemy
    private float speed = 1; // Speed of health bar animation
    public Transform player; // Reference to the player's transform

    private void Start()
    {
        if (enemyHealthBoss != null)
        {
            maxHP = enemyHealthBoss.health;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Update the health bar based on the enemy's current health
        if (enemyHealthBoss != null && maxHP != 0)
        {
            UpdateHP(enemyHPBar, enemyHealthBoss.health, maxHP);
        }

        // Make the health bar face the player
        if (player != null)
        {
            FacePlayer();
        }
    }

    void UpdateHP(Image hpBar, float currentHP, float maxHP)
    {
        float target = currentHP / maxHP; // Calculate health as a fraction
        hpBar.fillAmount = Mathf.MoveTowards(hpBar.fillAmount, target, speed * Time.deltaTime); // Smoothly update the health bar
    }

    void FacePlayer()
    {
        // Rotate the health bar to face the player
        Vector3 direction = (player.position - transform.position).normalized; // Get the direction to the player
        Quaternion lookRotation = Quaternion.LookRotation(direction); // Create a rotation that looks at the player
        transform.rotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0); // Apply the rotation, keeping the health bar upright
    }
}
