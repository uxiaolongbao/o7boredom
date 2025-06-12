using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float knockbackForce = 10f;
    private int damage = 20;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Health health = other.GetComponent<Health>();
            if (health != null)
                health.TakeDamage(damage);

            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = (other.transform.position - transform.position).normalized;
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
            }

            PlayerMove move = other.GetComponent<PlayerMove>();
            if (move != null)
                move.DisableMovement(0.5f); // disable input for 0.5 seconds
        }
    }
}