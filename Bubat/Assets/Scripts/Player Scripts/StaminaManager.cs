using UnityEngine;

public class StaminaManager : MonoBehaviour
{
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 5f; // Adjust this value to control regeneration speed
    private bool isRegenerating = true;

    private void Start()
    {
        currentStamina = maxStamina;
    }

    private void Update()
    {
        if (isRegenerating)
        {
            RegenerateStamina();
        }
    }

    public void UseStamina(float amount)
    {
        if (amount <= currentStamina)
        {
            currentStamina -= amount;
            isRegenerating = false; // Pause regeneration when stamina is used
            Invoke("EnableRegeneration", 1.0f); // Enable regeneration after a short delay
        }
        else
        {
            Debug.Log("Not enough stamina!");
        }

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }

    private void RegenerateStamina()
    {
        currentStamina += staminaRegenRate * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }

    private void EnableRegeneration()
    {
        isRegenerating = true;
    }

    public bool HasEnoughStamina(float amount)
    {
        return currentStamina >= amount;
    }
}
