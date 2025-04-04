using System.Numerics;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float speed;
    private Rigidbody2D body;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        body.linearVelocity = new UnityEngine.Vector2(Input.GetAxis("Horizontal") * speed, body.linearVelocityY);

        if(Input.GetKey(KeyCode.UpArrow))
            body.linearVelocity = new UnityEngine.Vector2(body.linearVelocityX, speed/5);
    }
}
