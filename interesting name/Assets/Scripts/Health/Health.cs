using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float startingHealth = 100f;
    [SerializeField] private bool isPlayer = false;
    
    [Header("Events")]
    public UnityEvent<float> OnHealthChanged; // for healthbar updates
    public UnityEvent OnDeath; // for death logic
    public UnityEvent OnHurt;  // for hurt effects

    //private Animator anim;

    public float CurrentHealth { get; private set; }
    public float MaxHealth => startingHealth;
    private void Update()
{
    if (isPlayer && Input.GetKeyDown(KeyCode.Q))
    {
        TakeDamage(10);
        Debug.Log("Damage dealt to player");
    }
}

    private void Awake()
    {
        CurrentHealth = startingHealth;
        //anim = GetComponent<Animator>();
    }

    public void TakeDamage(float damage)
    {
        damage = Mathf.Max(0, damage); 
        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, startingHealth);
        
        OnHealthChanged?.Invoke(CurrentHealth);

        if (CurrentHealth > 0)
        {
            OnHurt?.Invoke();
            if (isPlayer) Debug.Log($"Took {damage} damage! Health: {CurrentHealth}");
        }
        else
        {
            OnDeath?.Invoke();
            if (isPlayer) {
                Debug.Log("Player died!");
                //anim.SetBool("death", CurrentHealth <= 0);
            }
                
        }
    }

    public void Heal(float amount)
    {
        amount = Mathf.Max(0, amount);
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, startingHealth);
        OnHealthChanged?.Invoke(CurrentHealth);
    }
}