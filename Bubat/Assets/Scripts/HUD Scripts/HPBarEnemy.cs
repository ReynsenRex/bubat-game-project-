using UnityEngine;
using UnityEngine.UI;

public class HPBarEnemy : MonoBehaviour
{
    public Image enemyHPBar; // Health bar image
    public EnemyHealthNormies enemyHealthNormies; // Reference to the PlayerHealth script
    private float maxHP = 100.0f; // Maximum health of the player
    private float speed = 1; // Speed of health bar animation

    // Update is called once per frame
    void Update()
    {
        // Get the player's current health and update the health bar
        if (enemyHealthNormies != null)
        {
            UpdateHP(enemyHPBar, enemyHealthNormies.health, maxHP);
        }

        // Optional: Keep the health bar facing the camera (if needed)
        // Uncomment if the health bar is world-space UI
        // transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }

    void UpdateHP(Image hpBar, float currentHP, float maxHP)
    {
        float target = currentHP / maxHP; // Calculate health as a fraction
        hpBar.fillAmount = Mathf.MoveTowards(hpBar.fillAmount, target, speed * Time.deltaTime); // Smoothly update the health bar
    }
}
