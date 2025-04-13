using System.Numerics;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMove : MonoBehaviour
{
    //movement
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    private Rigidbody2D body;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;
    private bool isJumping; //ik theres a warning for this but immma use this later
    [SerializeField] private float wallSlideSpeed = 4f;
    [SerializeField] private float wallJumpHorizontalForce = 15f;
    private bool isWallJumping;
    //collision
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    //animation
    public Animator animator;
    //coyote time
    [SerializeField] private float coyoteTime = 0.1f;
    private float coyoteTimeCounter;
    //variable jump height
    [SerializeField] private float jumpHoldMultiplier = 0.5f;
    [SerializeField] private float minJumpHeight = 3f;
    private bool isHoldingJump;
    //Slippery make player uncontrollable
    [SerializeField] private float SlipperySpeedMultiplier = 0.3f;
    private bool isSlipping = false; 
    

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (isWallJumping) 
            return;
        //horizontalInput = Input.GetAxis("Horizontal"); Original Code
        //New code to help reduce movement input for player when slipping
        /*
         * horizontalInput = Input.GetAxis("Horizontal");

            float slideSpeed = isSlipping ? speed * 0.3f : speed;
            float acceleration = isSlipping ? 5f : 20f;

            float targetVelocityX = horizontalInput * slideSpeed;
            float newVelocityX = Mathf.Lerp(body.velocity.x, targetVelocityX, Time.deltaTime * acceleration);

            body.velocity = new Vector2(newVelocityX, body.velocity.y);

        */

        float baseInput = Input.GetAxis("Horizontal");
        horizontalInput = isSlipping ? baseInput * SlipperySpeedMultiplier : baseInput; 

        body.linearVelocity = new UnityEngine.Vector2(horizontalInput * speed, body.linearVelocityY);

        animator.SetFloat("speed", Mathf.Abs(horizontalInput));

        //flip player when moving left and right (for animations/sprites) + movement
        if(horizontalInput > 0.01f)
            transform.localScale = UnityEngine.Vector3.one;
        else if (horizontalInput < -0.01f)
            transform.localScale = new UnityEngine.Vector3(-1, 1, 1);
        
        body.linearVelocity = new UnityEngine.Vector2(horizontalInput * speed, body.linearVelocityY);

        //coyote time
        if (isGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        //flexible jump height
        isHoldingJump = Input.GetKey(KeyCode.UpArrow);
        if (!isHoldingJump && body.linearVelocity.y > minJumpHeight)
        {
            body.linearVelocity = new UnityEngine.Vector2(body.linearVelocityX, minJumpHeight);
        }
        else if (isHoldingJump && !isGrounded() && !onWall())
        {
            body.gravityScale = jumpHoldMultiplier;
        }
        else{
            body.gravityScale = 5;
        }

        //wall jumping logic
        if(wallJumpCooldown > 0.2f)
        {

            if (onWall() && !isGrounded())
            {
                if (body.linearVelocityY < 0)
                {
                    body.linearVelocity = new UnityEngine.Vector2(body.linearVelocityX, -wallSlideSpeed);
                }
                body.gravityScale = 4;
                body.linearVelocity = UnityEngine.Vector2.zero; 
            }
            else
                body.gravityScale = 5;

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                isJumping = true;
                Jump();
            }

        }
        else
            wallJumpCooldown += Time.deltaTime;

        if (isGrounded()) { }
            isJumping = false;
        animator.SetBool("isJump", !isGrounded());
    }

    //its literally jumping and all its variations duh. its called the jump method for a reason
    private void Jump()
    {
        if(isGrounded())
            body.linearVelocity = new UnityEngine.Vector2(body.linearVelocityX, jumpPower);
        else if (onWall() && !isGrounded())
        {
            float direction = -Mathf.Sign(transform.localScale.x);
            body.AddForce(new UnityEngine.Vector2(direction * wallJumpHorizontalForce, jumpPower), ForceMode2D.Impulse);
            isWallJumping = true;
            Invoke(nameof(ResetWallJump), 0.2f);
        }
    }

    private void ResetWallJump() => isWallJumping = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, UnityEngine.Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        bool left = Physics2D.Raycast(boxCollider.bounds.center, UnityEngine.Vector2.left, boxCollider.bounds.extents.x + 0.1f, wallLayer);
        bool right = Physics2D.Raycast(boxCollider.bounds.center, UnityEngine.Vector2.right, boxCollider.bounds.extents.x + 0.1f, wallLayer);
        return left || right;
    }

    //Slipping
    public void SetSlipping(bool slip)
    {
        isSlipping = slip;
    }
}
