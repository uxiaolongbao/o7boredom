using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float startingHealth = 100f;
    [SerializeField] private bool isPlayer = false;

    [Header("Events")]
    public UnityEvent<float> OnHealthChanged; // for healthbar updates
    public UnityEvent OnDeath; // for death logic
    public UnityEvent OnHurt;  // for hurt effects

    [Header("iFrames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    private SpriteRenderer spriteRend;
    private bool invulnerable;

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
        spriteRend = GetComponent<SpriteRenderer>();
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
            StartCoroutine(Invunerability());
        }
        else
        {
            OnDeath?.Invoke();
            if (isPlayer)
            {
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
    
    private IEnumerator Invunerability()
    {
        invulnerable = true;
        Physics2D.IgnoreLayerCollision(7, 3, true);
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }
        Physics2D.IgnoreLayerCollision(7, 3, false);
        invulnerable = false;
    }
}