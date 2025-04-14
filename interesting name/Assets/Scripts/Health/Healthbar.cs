using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private Image totalHealthBar;
    [SerializeField] private Image currentHealthBar;

    [Header("Settings")]
    [SerializeField] private float smoothSpeed = 5f;

    private void Start()
    {
        // Initialize visuals
        totalHealthBar.fillAmount = 1f;
        currentHealthBar.fillAmount = playerHealth.CurrentHealth / playerHealth.MaxHealth;
        
        // Subscribe to health changes
        playerHealth.OnHealthChanged.AddListener(UpdateHealthBar);
    }

    private void UpdateHealthBar(float currentHealth)
    {
        float targetFill = currentHealth / playerHealth.MaxHealth;
        StartCoroutine(SmoothHealthChange(targetFill));
    }

    private System.Collections.IEnumerator SmoothHealthChange(float targetFill)
    {
        while (Mathf.Abs(currentHealthBar.fillAmount - targetFill) > 0.01f)
        {
            currentHealthBar.fillAmount = Mathf.Lerp(
                currentHealthBar.fillAmount,
                targetFill,
                smoothSpeed * Time.deltaTime
            );
            yield return null;
        }
        currentHealthBar.fillAmount = targetFill; // Snap to final value
    }
}