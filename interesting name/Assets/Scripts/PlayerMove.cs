using System.Numerics;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float speed;
    private Rigidbody2D body;
    private bool grounded;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        body.linearVelocity = new UnityEngine.Vector2(horizontalInput * speed, body.linearVelocityY);

        //flip player when moving left and right (for animations/sprites) + movement
        if(horizontalInput > 0.01f)
            transform.localScale = UnityEngine.Vector3.one;
        else if (horizontalInput < -0.01f)
            transform.localScale = new UnityEngine.Vector3(-1, 1, 1);

        if(Input.GetKey(KeyCode.UpArrow) && grounded)
            Jump();
    }

    //simplifying jumping and all its conditions
    private void Jump()
    {
        body.linearVelocity = new UnityEngine.Vector2(body.linearVelocityX, speed);
        grounded = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ground")
            grounded = true;
    }
}
