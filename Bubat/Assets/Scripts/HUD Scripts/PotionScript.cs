using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PotionScript : MonoBehaviour
{
    public Image potionImg;
    public Movement movement = new Movement();
    public TMP_Text potionCount;
    private int maxPotion;
    private int currentUses;

    // Start is called before the first frame update
    void Start()
    {
        maxPotion = movement.maxHealUses;
        currentUses = movement.remainingHealUses;
        UpdatePotionUI();
    }

    // Update is called once per frame
    void Update()
    {
        UsePotion();  
    }

    public void UsePotion()
    {
        if (currentUses > 0)
        {
            currentUses--; // Decrease the potion count
            UpdatePotionUI(); // Update the UI
            Debug.Log($"Potion used! Remaining potions: {currentUses}");

            // Add your potion effect logic here (e.g., heal the player)
        }
        else
        {
            Debug.Log("No potions left!");
        }
    }

    private void UpdatePotionUI()
    {
        // Update the potion count text
        potionCount.text = currentUses.ToString();

        // Check if potions are all used up
        if (currentUses == 0)
        {
            // Make the potion image black and white
            potionImg.color = Color.gray;
        }
        else
        {
            // Reset to normal color if potions are available
            potionImg.color = Color.white;
        }
    }
}
