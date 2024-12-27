using UnityEngine;
using UnityEngine.UI;

public class StaminaBarScript : MonoBehaviour
{
    public StaminaManager staminaManager; // Reference to the StaminaManager script
    public Image staminaBar; // UI Image component for the stamina bar

    void Update()
    {
        if (staminaManager != null && staminaBar != null)
        {
            // Update the stamina bar's fill amount based on the current stamina
            staminaBar.fillAmount = staminaManager.currentStamina / staminaManager.maxStamina;
        }
    }
}
