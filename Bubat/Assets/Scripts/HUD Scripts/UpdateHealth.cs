using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    public Slider slider;
    public Image currentHealthBar;
    public TMP_Text ratioText;

    private float hitpoint = 150;
    private float maxHitpoint = 150;

    private void Start()
    {
        // Initially update the health bar
        UpdateHealthbar();
    }

    private void UpdateHealthbar()
    {
        float ratio = hitpoint / maxHitpoint;
        // Update the slider
        slider.value = ratio;

        // Optionally update the ratio text, disable it for now
        if (ratioText != null)
        {
            ratioText.gameObject.SetActive(false); // Disable the text
        }
    }

    private void TakeDamage(float damage)
    {
        hitpoint -= damage;
        if (hitpoint < 0)
        {
            hitpoint = 0;
        }
        UpdateHealthbar();
    }

    private void HealHealth(float heal)
    {
        hitpoint += heal;
        if (hitpoint > maxHitpoint)
        {
            hitpoint = maxHitpoint;
        }
        UpdateHealthbar();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the player collides with an enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(10); // Adjust the damage amount as needed
        }
    }
}
