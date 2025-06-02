using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    private float direction;
    private bool hit;
    private float lifetime;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        lifetime = 0;
        hit = false;
        boxCollider.enabled = true;

        rb.linearVelocity = new Vector2(direction * speed, 0);
    }

    private void Update()
    {
        lifetime += Time.deltaTime;
        if (lifetime > 3)
        {
            gameObject.SetActive(false);
        }

        if (Mathf.Abs(rb.linearVelocity.y) > 0.1f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        hit = true;
        rb.linearVelocity = Vector2.zero;
        boxCollider.enabled = false;
        gameObject.SetActive(false);

    }

    public void SetDirection(float _direction)
    {
        lifetime = 0;
        direction = _direction;
        gameObject.SetActive(true);
        float localScaleX = transform.localScale.x;
        if (Mathf.Sign(localScaleX) != _direction)
            localScaleX = -localScaleX;

        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
