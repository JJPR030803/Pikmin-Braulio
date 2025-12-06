using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CircularHealthBar : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image healthFillImage;
    //[SerializeField] private Image staminaFillImage; // Optional for stamina ring
    [SerializeField] private TextMeshProUGUI healthText; // Optional
    
    [Header("Player References")]
    [SerializeField] private PlayerHealth playerHealth;
    //[SerializeField] private PlayerStamina playerStamina; // Optional
    
    [Header("Visual Settings")]
    [SerializeField] private Color healthyColor = new Color(0.2f, 1f, 0.2f); // Green
    [SerializeField] private Color lowHealthColor = new Color(1f, 0.2f, 0.2f); // Red
    [SerializeField] private float lowHealthThreshold = 0.3f; // 30% health
    [SerializeField] private Color staminaColor = new Color(0.2f, 1f, 0.2f); // Green
    [SerializeField] private bool smoothTransition = true;
    [SerializeField] private float transitionSpeed = 5f;
    [SerializeField] private bool smoothColorTransition = true; // NEW
    [SerializeField] private float colorTransitionSpeed = 3f; // NEW
    
    private float targetHealthFill;
    private float targetStaminaFill;
    private Color currentHealthColor; // NEW
    private Color targetHealthColor; // NEW
    
    void Start()
    {
        // Set initial colors
        currentHealthColor = healthyColor;
        targetHealthColor = healthyColor;
        
        if (healthFillImage != null)
            healthFillImage.color = healthyColor;
            
        
        
        // Subscribe to events
        if (playerHealth != null)
            playerHealth.OnHealthChanged.AddListener(UpdateHealth);
            
        
    }
    
    void Update()
    {
        // Smooth fill amount transition
        if (smoothTransition)
        {
            if (healthFillImage != null)
            {
                healthFillImage.fillAmount = Mathf.Lerp(
                    healthFillImage.fillAmount, 
                    targetHealthFill, 
                    Time.deltaTime * transitionSpeed
                );
            }
            
           
            
        }
        
        // Smooth color transition (NEW)
        if (smoothColorTransition && healthFillImage != null)
        {
            currentHealthColor = Color.Lerp(
                currentHealthColor,
                targetHealthColor,
                Time.deltaTime * colorTransitionSpeed
            );
            healthFillImage.color = currentHealthColor;
        }
    }
    
    public void UpdateHealth(float healthPercentage)
    {
        targetHealthFill = healthPercentage;
        
        if (!smoothTransition && healthFillImage != null)
        {
            healthFillImage.fillAmount = healthPercentage;
        }
        
        // Update color based on threshold (NEW)
        if (healthPercentage <= lowHealthThreshold)
        {
            targetHealthColor = lowHealthColor;
        }
        else
        {
            targetHealthColor = healthyColor;
        }
        
        // Instant color change if smooth transition is off
        if (!smoothColorTransition && healthFillImage != null)
        {
            healthFillImage.color = targetHealthColor;
            currentHealthColor = targetHealthColor;
        }
        
        // Update text
        if (healthText != null && playerHealth != null)
        {
            healthText.text = Mathf.Ceil(playerHealth.GetCurrentHealth()).ToString();
        }
    }
    
    
    
    void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged.RemoveListener(UpdateHealth);
    }
}
